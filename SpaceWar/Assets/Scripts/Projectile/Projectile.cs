using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject hitVFX;//命中视觉特效
    [SerializeField] AudioData[] hitSFX;
    [SerializeField] float damage;//子弹伤害
    [SerializeField] protected float moveSpeed = 10f;//移动速度
    [SerializeField] protected Vector2 moveDirection;//移动二维向量

    protected GameObject target;

   protected virtual void OnEnable()
    {
        StartCoroutine(MoveDirectly());//启用协程
    }
    IEnumerator MoveDirectly()
    {
        // 移动协程
        while (gameObject.activeSelf)
        {
            Move();

            yield return null;//挂起直到下一帧
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent<Character>(out Character character))
        {
            character.TakeDamage(damage);
            PoolManager.Release(hitVFX, collision.GetContact(0).point, Quaternion.LookRotation(collision.GetContact(0).normal));
            AudioManager.Instance.PlayRandomSFX(hitSFX);
            gameObject.SetActive(false);
        }
    }

    protected void SetTarget(GameObject target) => this.target = target;

    public void Move()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
}
