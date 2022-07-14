using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject hitVFX;//�����Ӿ���Ч
    [SerializeField] AudioData[] hitSFX;
    [SerializeField] float damage;//�ӵ��˺�
    [SerializeField] protected float moveSpeed = 10f;//�ƶ��ٶ�
    [SerializeField] protected Vector2 moveDirection;//�ƶ���ά����

    protected GameObject target;

   protected virtual void OnEnable()
    {
        StartCoroutine(MoveDirectly());//����Э��
    }
    IEnumerator MoveDirectly()
    {
        // �ƶ�Э��
        while (gameObject.activeSelf)
        {
            Move();

            yield return null;//����ֱ����һ֡
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
