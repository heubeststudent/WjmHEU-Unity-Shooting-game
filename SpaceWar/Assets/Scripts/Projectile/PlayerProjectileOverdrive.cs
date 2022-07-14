using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileOverdrive : Player_Projectile
{
    [SerializeField] ProjectileGudanceSystem gudanceSystem;
    protected override void OnEnable()
    {
        SetTarget(EnemyManager.Instance.RandomEnemy);
        transform.rotation = Quaternion.identity;
        if(target == null)
        {
            base.OnEnable();
        }
        else
        {
            StartCoroutine(gudanceSystem.HomingCoroutine(target));
        }
       

    }
}
