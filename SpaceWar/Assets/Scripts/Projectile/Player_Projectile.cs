using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Projectile : Projectile
{
    TrailRenderer trail;//����һ���ӵ�β��

    protected virtual void Awake()
    {
        //��ȡTrailRenderer�����
        trail = GetComponentInChildren<TrailRenderer>();

        if (moveDirection != Vector2.right)
        {
            transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector2.right, moveDirection);
        }
    }

    void OnDestroy()
    {
        //�ӵ�����ʱ���β��
        trail.Clear();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        PlayerEnergy.Instance.Obtain(PlayerEnergy.PERCENT);

    }
}
