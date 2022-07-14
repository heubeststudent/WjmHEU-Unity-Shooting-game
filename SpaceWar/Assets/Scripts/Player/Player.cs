using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]//ȡ��2D�������
public class Player : Character
{

    [SerializeField] Status_Bar_HDR status_Bar_HDR;
    [SerializeField] bool regenerateHealth = true;//�������������������
    [SerializeField] float healthRegenerateTime;//����ֵ����ʱ��
    [SerializeField,Range(0f,1f)] float healthRegeneratePercent;//����ֵ�ظ��ٷֱ�

    [Header("....INPUT....")]
    [SerializeField] PlayInput input;//�������������

    [Header("....MOVE....")]
    [SerializeField] float movespeed = 10f;//����ƶ��ٶ�
    [SerializeField] float accelerationTime = 2f;//��Ҽ���ʱ��
    [SerializeField] float deccelerationTime = 2f;//��Ҽ���ʱ��
    float paddingx = 0.8f;//����ƫ��x��֤�ɻ����������
    float paddingy = 0.22f;//����ƫ��y��֤�ɻ����������
    [SerializeField] float moveRotationAngle = 50f; //�ɻ��������ƶ�ʱ��ת���Ƕ�

    [Header("....FIRE....")]
    [SerializeField, Range(0, 2)] int weaponPower = 0; //��������
    [SerializeField] GameObject projectile1; //����ӵ�1
    [SerializeField] GameObject projectile2;//����ӵ�2
    [SerializeField] GameObject projectile3;//����ӵ�3
    [SerializeField] GameObject projectileOverdrive;//���������ӵ�
    [SerializeField] ParticleSystem muzzleVFX;//ǹ����Ч
    [SerializeField] Transform muzzleMiddle; //�м��ӵ�ǹ��
    [SerializeField] Transform muzzleTop;//�Ϸ��ӵ�ǹ��
    [SerializeField] Transform muzzleBottom;//�·��ӵ�ǹ��
    [SerializeField] private float fireInterval = 0.2f;//������
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

    WaitForSeconds waitforfireInterval;//WaitForSeconds���͵ĵȴ�����
    WaitForSeconds waitforOverdriveInterval;//��������������
    WaitForSeconds waitHealthRegenerateTime;//�ȴ�����ֵ����ʱ��
    WaitForSeconds waitDecelerationTime;//�ȴ�����ʱ��
    WaitForSeconds waitInvincibleTime;//�����ײ�ӵ��󻺳�ʱ��

    new Rigidbody2D rigidbody; //����һ��2d�������

    Coroutine moveCoroutine;

    Coroutine healthRegenerateCoroutine;

    new Collider2D collider;

    MissileSystrm missile; 

    void Awake()
    {
        //��ȡ2D�������
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        missile = GetComponent<MissileSystrm>();
        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingx = size.x / 2f;
        paddingy = size.y / 2f;
        dodgeDuration = maxRoll / rollSpeed;
        rigidbody.gravityScale = 0f;//���������Ҳ������ҷɻ���������Ϊ0��������һῪʼʱ����Ϊ����������

        waitforfireInterval = new WaitForSeconds(fireInterval);
        waitforOverdriveInterval = new WaitForSeconds(fireInterval /= overdriveSpeedFactor);
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime);
        waitDecelerationTime = new WaitForSeconds(deccelerationTime);
        waitInvincibleTime = new WaitForSeconds(InvincibleTime);

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        input.onMove += Move;//����ƶ�
        input.onstopMove += StopMove;//���ֹͣ�ƶ�
        input.onFire += Fire;//��ҿ���
        input.onStopFire += StopFire;//���ֹͣ����
        input.switchweap += weap;//���ת����������
        input.onDodge += Dodge;
        input.onOverdrive += Overdrive;
        input.onLaunchMissile += LaunchMissile;

        PlayerOverdrive.on += OverdriveOn;
        PlayerOverdrive.off += OverdriveOff;


        
    }

    void OnDisable()
    {
        //��OnEnable�෴
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
        //����һ����Ԫ���ȡ�ɻ��ƶ�ʱת���ĽǶ�
        Quaternion moveRotation = Quaternion.AngleAxis(moveRotationAngle * moveInput.y, Vector3.right);
        if(moveCoroutine!=null)
        {
            StopCoroutine(moveCoroutine);
        }
        //moveCoroutineȡ��Э��
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
        //�ɻ��ƶ���Э�̣�Ϊ���÷ɻ����ٺͼ��ٳ�����ʹ��Э��ʵ��
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
        //��֤�ɻ��Ļ��岻�ᳬ������
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
        //���ﺯ���������봫�����Ʋ�Ȼ��ʱ�ᱨ��
        StopCoroutine(nameof(FireCoroutine));
    }

    IEnumerator FireCoroutine() 
    {
        //�ɻ�����Э��
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
        //����������������ת��
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
        //��Ұ�x�ᷭת������
    }

    IEnumerator DodgeCoroutine()
    {

        isDodging = true;
        AudioManager.Instance.PlayRandomSFX(dodgeSFX);
        //����ֵ����
        PlayerEnergy.Instance.Use(dodgeEnergyCost);

        //�������ʱ����޵�
        collider.isTrigger = true;

        //��Ұ�x�ᷭת������
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
