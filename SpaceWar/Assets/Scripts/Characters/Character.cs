using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : MonoBehaviour
{

    [SerializeField] GameObject deathVFX;

    [Header("....HEALTH....")]
    [SerializeField] protected float maxHealth;
    [SerializeField] bool showOnHeadHealthBar = true;
    [SerializeField] StatusBar onHeadHealthBar;
    [SerializeField] AudioData[] deathSFX;
    protected float health;

    protected virtual void OnEnable()
    {
        health = maxHealth;
        if(showOnHeadHealthBar)
        {
            ShowOnHeadHealthBar();
        }
        else
        {
            HideOnHeadHealthBar();
        }
    }

    public void ShowOnHeadHealthBar()
    {
        onHeadHealthBar .gameObject.SetActive(true);
        onHeadHealthBar.Initialize(health, maxHealth);
    }

    public void HideOnHeadHealthBar()
    {
        onHeadHealthBar.gameObject.SetActive(false);
    }

    public virtual void TakeDamage(float damage)
    {
        if(health == 0f)
        {
            return;
        }
        health -= damage;
        if(showOnHeadHealthBar)
        {
            onHeadHealthBar.UpdateStatus(health, maxHealth);
        }
        if (health <= 0f)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        health = 0f;
        AudioManager.Instance.PlayRandomSFX(deathSFX);
        PoolManager.Release(deathVFX, transform.position);
        gameObject.SetActive(false);
    }

    public virtual void RestoreHealth(float value)
    {
        if(health == maxHealth)
        {
            return;
        }
        //health += value;
        //health = Mathf.Clamp(health,0f,maxHealth);
        health = Mathf.Clamp(health+value, 0f, maxHealth);
        if (showOnHeadHealthBar)
        {
            onHeadHealthBar.UpdateStatus(health, maxHealth);
        }
    }

    protected IEnumerator HealthRegenerateCoroutine(WaitForSeconds WaitTime,float percent)
    {
        while(health<maxHealth)
        {
            yield return WaitTime;
            RestoreHealth(maxHealth * percent);
        }
    }

    protected IEnumerator DamageOverTimeCoroutine(WaitForSeconds WaitTime, float percent)
    {
        while (health >0f)
        {
            yield return WaitTime;

            TakeDamage(maxHealth * percent);
        }
    }


}
