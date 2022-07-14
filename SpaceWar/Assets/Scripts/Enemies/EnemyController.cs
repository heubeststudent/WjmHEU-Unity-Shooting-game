using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [Header("....MOVE....")]
    float paddingx;
    float paddingy;
    [SerializeField] float movespeed = 2f;
    [SerializeField] float moveRoationAngle = 20f;
    
    [Header("....FIRE....")]
    [SerializeField] protected GameObject[] projectiles;
    [SerializeField] protected AudioData[] projectileLaunchSFX;
    [SerializeField] protected Transform muzzle;//敌人子弹枪口位置
    [SerializeField] protected float minFireInterval;
    [SerializeField] protected float maxFireInterval;
    [SerializeField] protected ParticleSystem muzzleVFX;

    protected float paddingX;
    float paddingY;

    protected Vector3 targetPosition;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    protected virtual void Awake()
    {
        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingx = size.x / 2f;
        paddingy = size.y / 2f;
    }

    protected virtual void OnEnable()
    {
        //每当启用一个对象时onenable都会执行一次
        StartCoroutine(nameof(RandomlyMovingCoroutine));
        StartCoroutine(nameof(RandomFireCoroutine));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator RandomlyMovingCoroutine()
    {
        transform.position = Viewport.Instance.RadomEnemiesPawPosition(paddingx, paddingy);
        Vector3 targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingx, paddingy);

        while(gameObject.activeSelf)
        {
  
            //如果敌人没有目标位置
            if (Vector3.Distance(transform.position, targetPosition)>= movespeed * Time.fixedDeltaTime)
            {
                //我们就赋给他一个目标位置
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, movespeed * Time.fixedDeltaTime);
                //转动敌人x轴取得转动效果
                transform.rotation = Quaternion.AngleAxis((targetPosition - transform.position).normalized.y * moveRoationAngle, Vector3.right);

            }
            else
            {
                //如果敌人已经到达目标位置我们就再给他一个目标位置
                targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingx, paddingy);
            }
            yield return waitForFixedUpdate;
        }
    }

    protected virtual IEnumerator RandomFireCoroutine()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(Random.Range(minFireInterval, maxFireInterval));

            if (GameManager.GameState == GameState.GameOver) yield break;

            foreach (var projectile in projectiles)
            {
                PoolManager.Release(projectile, muzzle.position);
            }
            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);
            muzzleVFX.Play();
        }
    }
}
