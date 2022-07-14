using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]//取得2D刚体组件
public class Player : Character
{

    [SerializeField] Status_Bar_HDR status_Bar_HDR;
    [SerializeField] bool regenerateHealth = true;//方便启用玩家再生功能
    [SerializeField] float healthRegenerateTime;//生命值再生时间
    [SerializeField,Range(0f,1f)] float healthRegeneratePercent;//生命值回复百分比

    [Header("....INPUT....")]
    [SerializeField] PlayInput input;//声明玩家输入类

    [Header("....MOVE....")]
    [SerializeField] float movespeed = 10f;//玩家移动速度
    [SerializeField] float accelerationTime = 2f;//玩家加速时间
    [SerializeField] float deccelerationTime = 2f;//玩家减速时间
    float paddingx = 0.8f;//机体偏移x保证飞机不会出背景
    float paddingy = 0.22f;//机体偏移y保证飞机不会出背景
    [SerializeField] float moveRotationAngle = 50f; //飞机在上下移动时的转动角度

    [Header("....FIRE....")]
    [SerializeField, Range(0, 2)] int weaponPower = 0; //武器威力
    [SerializeField] GameObject projectile1; //玩家子弹1
    [SerializeField] GameObject projectile2;//玩家子弹2
    [SerializeField] GameObject projectile3;//玩家子弹3
    [SerializeField] GameObject projectileOverdrive;//能量爆发子弹
    [SerializeField] ParticleSystem muzzleVFX;//枪口特效
    [SerializeField] Transform muzzleMiddle; //中间子弹枪口
    [SerializeField] Transform muzzleTop;//上方子弹枪口
    [SerializeField] Transform muzzleBottom;//下方子弹枪口
    [SerializeField] private float fireInterval = 0.2f;//开火间隔
    [SerializeField] AudioData projectileLuntchSFX;

    [Header("....DODGE....")]
    [SerializeField,Range(0,100)] int dodgeEnergyCost = 25;
    [SerializeField] float maxRoll = 720f;
    [SerializeField] float rollSpeed = 360f;
    [SerializeField] Vector3 dodgeScale = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] AudioData dodgeSFX;

    [Header("....OVERDRIVE....")]
    [SerializeField] int overdriveDodgeFactor = 2;
    [SerializeField] float overdriveSpeedFactor = 1.2f;


    bool isDodging = false;
    bool isOverdriving = false;

    readonly float SlowMotionDuration = 1f;
    readonly float InvincibleTime = 1f;

    float currentRoll;

    float dodgeDuration;
    float t;
    Vector2 moveDirection;
    Vector2 previousVelocity;
    Quaternion previousRotation;

    WaitForSeconds waitforfireInterval;//WaitForSeconds类型的等待开火
    WaitForSeconds waitforOverdriveInterval;//能量爆发开火间隔
    WaitForSeconds waitHealthRegenerateTime;//等待生命值再生时间
    WaitForSeconds waitDecelerationTime;//等待减速时间
    WaitForSeconds waitInvincibleTime;//玩家碰撞子弹后缓冲时间

    new Rigidbody2D rigidbody; //声明一个2d刚体组件

    Coroutine moveCoroutine;

    Coroutine healthRegenerateCoroutine;

    new Collider2D collider;

    MissileSystrm missile; 

    void Awake()
    {
        //获取2D刚体组件
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        missile = GetComponent<MissileSystrm>();
        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingx = size.x / 2f;
        paddingy = size.y / 2f;
        dodgeDuration = maxRoll / rollSpeed;
        rigidbody.gravityScale = 0f;//将刚体对象也就是玩家飞机的重力设为0，否则玩家会开始时就因为重力而下落

        waitforfireInterval = new WaitForSeconds(fireInterval);
        waitforOverdriveInterval = new WaitForSeconds(fireInterval /= overdriveSpeedFactor);
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime);
        waitDecelerationTime = new WaitForSeconds(deccelerationTime);
        waitInvincibleTime = new WaitForSeconds(InvincibleTime);

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        input.onMove += Move;//玩家移动
        input.onstopMove += StopMove;//玩家停止移动
        input.onFire += Fire;//玩家开火
        input.onStopFire += StopFire;//玩家停止开火
        input.switchweap += weap;//玩家转换武器威力
        input.onDodge += Dodge;
        input.onOverdrive += Overdrive;
        input.onLaunchMissile += LaunchMissile;

        PlayerOverdrive.on += OverdriveOn;
        PlayerOverdrive.off += OverdriveOff;


        
    }

    void OnDisable()
    {
        //与OnEnable相反
        input.onMove -= Move;
        input.onstopMove -= StopMove;
        input.onFire -= Fire;
        input.onStopFire -= StopFire;
        input.switchweap -= weap;
        input.onDodge -= Dodge;
        input.onOverdrive -= Overdrive;
        input.onLaunchMissile -= LaunchMissile;

        PlayerOverdrive.on -= OverdriveOn;
        PlayerOverdrive.off -= OverdriveOff;
    }

    void Start()
    {
        status_Bar_HDR.Initialize(health,maxHealth);

        input.EnableGamePlayInput();

    }

    #region HEALTH
    
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        status_Bar_HDR.UpdateStatus(health, maxHealth);
        TimeController.Instance.BulleTime(SlowMotionDuration);
        if (gameObject.activeSelf)
        {
            Move(moveDirection);
            StartCoroutine(InvincibleCoroutine());
            if (regenerateHealth)
            {
                if(healthRegenerateCoroutine!=null)
                {
                    StopCoroutine(healthRegenerateCoroutine);
                }
                 healthRegenerateCoroutine = StartCoroutine(HealthRegenerateCoroutine(waitHealthRegenerateTime, healthRegeneratePercent));
            }
        }
    }

    public override void RestoreHealth(float value)
    {
        base.RestoreHealth(value);
        status_Bar_HDR.UpdateStatus(health, maxHealth);
    }

    public override void Die()
    {
        GameManager.onGameOver?.Invoke();
        GameManager.GameState = GameState.GameOver;
        status_Bar_HDR.UpdateStatus(0f,maxHealth);
        base.Die();

    }

    IEnumerator InvincibleCoroutine()
    {
        collider.isTrigger = true;

        yield return waitInvincibleTime;

        collider.isTrigger = false;
    }
    #endregion

    #region Move
    void Move(Vector2 moveInput)
    {
        //Vector2 moveAmount = moveInput * movespeed;
        //rigidbody.velocity = moveAmount;
        //声明一个四元组获取飞机移动时转动的角度
        Quaternion moveRotation = Quaternion.AngleAxis(moveRotationAngle * moveInput.y, Vector3.right);
        if(moveCoroutine!=null)
        {
            StopCoroutine(moveCoroutine);
        }
        //moveCoroutine取得协程
        moveDirection = moveInput.normalized;
        moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime,moveInput.normalized * movespeed,moveRotation));
        StopCoroutine(nameof(DeccelerationCoroutine));
        StartCoroutine(nameof(MoveRangeLimationCoroutine));
    }

    void StopMove()
    {
        //rigidbody.velocity = Vector2.zero;
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveDirection = Vector2.zero;
        moveCoroutine = StartCoroutine(MoveCoroutine(deccelerationTime, moveDirection, Quaternion.identity));
        StartCoroutine(nameof(DeccelerationCoroutine));

    }

    IEnumerator MoveCoroutine(float time,Vector2 moveVelocity,Quaternion moveRotation)
    {
        //飞机移动的协程，为了让飞机加速和减速呈线性使用协程实现
        t = 0f;
        previousVelocity = rigidbody.velocity;
        previousRotation = transform.rotation;
        while (t< 1f)
        {
            t += Time.fixedDeltaTime/ time;
            rigidbody.velocity = Vector2.Lerp(previousVelocity, moveVelocity, t);
            transform.rotation = Quaternion.Lerp(previousRotation, moveRotation, t);
            yield return new WaitForFixedUpdate();
        }
        
    }

    IEnumerator MoveRangeLimationCoroutine()
    {
        //保证飞机的机体不会超出背景
        while(true)
        {
            transform.position = Viewport.Instance.PlayerMoveablePosition(transform.position,paddingx,paddingy);
            yield return null;
        }
    }

    IEnumerator DeccelerationCoroutine()
    {
        yield return waitDecelerationTime;
        StopCoroutine(nameof(MoveRangeLimationCoroutine));
    }
    #endregion

    #region Fire

    void Fire()
    {
        muzzleVFX.Play();
        StartCoroutine(nameof(FireCoroutine));
        
    }

    void StopFire()
    {
        muzzleVFX.Stop();
        //StopCoroutine("FireCoroutine");
        //这里函数变量必须传入名称不然有时会报错
        StopCoroutine(nameof(FireCoroutine));
    }

    IEnumerator FireCoroutine() 
    {
        //飞机开火协程
        while (true)
        {
            switch (weaponPower)
            {
                case 0:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleMiddle.position);
                    break;
                case 1:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleTop.position);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleBottom.position);
                    break;
                case 2:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleMiddle.position);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile2, muzzleTop.position);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile3, muzzleBottom.position);
                    break;
                default:
                    break;
            }
            AudioManager.Instance.PlayRandomSFX(projectileLuntchSFX);
            if(isOverdriving)
            {
                yield return waitforOverdriveInterval;
            }
            else
            {
                yield return waitforfireInterval;
            }
        }
        
    }
    #endregion

    #region Weapon
    void weap()
    {
        //对武器的威力进行转换
        if( weaponPower<=2)
        {
            weaponPower++;
        }
        if( weaponPower > 2 )
        {
            weaponPower--;
            weaponPower--;
            weaponPower--;
        }
    }
    #endregion

    #region DODGE

    void Dodge()
    {
        if(isDodging || !PlayerEnergy.Instance.IsEnough(dodgeEnergyCost))
        {
            return;
        }
        StartCoroutine(nameof(DodgeCoroutine));
        //玩家按x轴翻转且伸缩
    }

    IEnumerator DodgeCoroutine()
    {

        isDodging = true;
        AudioManager.Instance.PlayRandomSFX(dodgeSFX);
        //能量值消耗
        PlayerEnergy.Instance.Use(dodgeEnergyCost);

        //玩家闪避时玩家无敌
        collider.isTrigger = true;

        //玩家按x轴翻转且伸缩
        currentRoll = 0f;
        TimeController.Instance.BulleTime(SlowMotionDuration,SlowMotionDuration);
        var scale = transform.localScale;
        while(currentRoll<maxRoll)
        {
            currentRoll += rollSpeed*Time.deltaTime;
            transform.rotation =  Quaternion.AngleAxis(currentRoll, Vector3.right);

            if(currentRoll < maxRoll/2f)
            {
                //scale -= (Time.deltaTime * dodgeDuration)*Vector3.one;
                scale.x = Mathf.Clamp(scale.x - Time.deltaTime / dodgeDuration, dodgeScale.x,1f);
                scale.y = Mathf.Clamp(scale.y - Time.deltaTime / dodgeDuration, dodgeScale.y, 1f);
                scale.z = Mathf.Clamp(scale.z - Time.deltaTime / dodgeDuration, dodgeScale.z, 1f);
            }
            else
            {
                //scale += (Time.deltaTime * dodgeDuration) * Vector3.one;
                scale.x = Mathf.Clamp(scale.x + Time.deltaTime / dodgeDuration, dodgeScale.x, 1f);
                scale.y = Mathf.Clamp(scale.y + Time.deltaTime / dodgeDuration, dodgeScale.y, 1f);
                scale.z = Mathf.Clamp(scale.z + Time.deltaTime / dodgeDuration, dodgeScale.z, 1f);
            }
            transform.localScale = scale;
            yield return null;
        }
        collider.isTrigger = false;

        isDodging = false;

    }
    #endregion

    #region OVERDRIVE

    void Overdrive()
    {
        if(!PlayerEnergy.Instance.IsEnough(PlayerEnergy.MAX))
        {
            return;
        }
        PlayerOverdrive.on.Invoke();
    }

    void OverdriveOn()
    {
        isOverdriving = true;
        dodgeEnergyCost *= overdriveDodgeFactor;
        movespeed *= overdriveSpeedFactor;
        TimeController.Instance.BulleTime(SlowMotionDuration, SlowMotionDuration);
    }

    void OverdriveOff()
    {
        isOverdriving = false;
        dodgeEnergyCost /= overdriveDodgeFactor;
        movespeed /= overdriveSpeedFactor;
    }

    #endregion

    void LaunchMissile()
    {
        missile.Launch(muzzleMiddle);
    }
}
