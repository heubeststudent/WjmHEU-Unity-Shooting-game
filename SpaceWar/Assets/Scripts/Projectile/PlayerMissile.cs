using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissile : PlayerProjectileOverdrive
{
    [SerializeField] AudioData targetAcquiredVoice = null;
    
    [Header("....SPEED CHANCE....")]
    [SerializeField] float lowSpeed = 8f;
    [SerializeField] float highSpeed = 25f;
    [SerializeField] float variableSpeedDelay = 0.5f;
    [Header("....EXPLOSION....")]
    [SerializeField] GameObject explosionVFX = null;
    [SerializeField] AudioData explosionSFX = null;

    WaitForSeconds waitvariableSpeedDelay;

    protected override void Awake()
    {
        base.Awake();
        waitvariableSpeedDelay = new WaitForSeconds(variableSpeedDelay);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(nameof(VariableSpeedCorutine));

    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        // µºµØ±¨’®
        PoolManager.Release(explosionVFX, transform.position);
        // ≤•∑≈µºµØ±¨’®“Ù–ß
        AudioManager.Instance.PlayRandomSFX(explosionSFX);
        //µºµØ±¨’®∑∂Œß…À∫¶
        //var colliders = Physics2D.OverlapCircleAll(transform.position, explisionRadius, enemyLayerMask);

        //foreach (var collider in colliders)
        //{
        //    if (collider.TryGetComponent<Enemy>(out Enemy enemy))
        //    {
        //        enemy.TakeDamage(explosionDamage);
        //    }
        //}
    }

    IEnumerator VariableSpeedCorutine()
    {
        moveSpeed = lowSpeed; 

        yield return waitvariableSpeedDelay;

        moveSpeed = highSpeed;

        if(target!=null)
        {
            AudioManager.Instance.PlayRandomSFX(targetAcquiredVoice);
        }
    }

}
