using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjevtile_Aiming : Projectile
{
    private void Awake()
    {
        SetTarget(GameObject.FindGameObjectWithTag("Player"));
    }

    protected override void OnEnable()
    {
        StartCoroutine(nameof(MoveDirectionCorountine));
        base.OnEnable();
    }

    IEnumerator MoveDirectionCorountine()
    {
        yield return null;
        if (target.activeSelf)
        {
            moveDirection = (target.transform.position - transform.position).normalized;

        }
    }
}
