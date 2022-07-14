using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Projectile : Projectile
{
    TrailRenderer trail;//声明一个子弹尾迹

    protected virtual void Awake()
    {
        //获取TrailRenderer的组件
        trail = GetComponentInChildren<TrailRenderer>();

        if (moveDirection != Vector2.right)
        {
            transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector2.right, moveDirection);
        }
    }

    void OnDestroy()
    {
        //子弹销毁时清空尾迹
        trail.Clear();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        PlayerEnergy.Instance.Obtain(PlayerEnergy.PERCENT);

    }
}
