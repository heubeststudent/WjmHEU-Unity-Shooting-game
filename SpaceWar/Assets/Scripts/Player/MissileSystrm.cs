using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSystrm : MonoBehaviour
{
    [SerializeField] int defaultAmount = 5;
    [SerializeField] float cooldownTime = 1f;
    [SerializeField] GameObject missilePrefab = null;
    [SerializeField] AudioData launchSFX = null;

    int amount;
    bool isReady = true;
    void Awake()
    {
        amount = defaultAmount;
    }

    void Start()
    {
        MissileDisplay.UpdateAmountText(amount);
    }

    public void Launch(Transform muzzleTransform)
    {
        if (amount == 0 || !isReady)
        {
            return;
        }
        isReady = false;
        //对象池中取出导弹
        PoolManager.Release(missilePrefab, muzzleTransform.position);
        //播放导弹音效
        AudioManager.Instance.PlayRandomSFX(launchSFX);

        amount--;
        MissileDisplay.UpdateAmountText(amount);

        if (amount == 0)
        {
            MissileDisplay.UpdateCooldownImage(1f);
        }
        else
        {
            StartCoroutine(CooldownCoroutine());
        }

    }

    IEnumerator CooldownCoroutine()
    {
        var cooldownValue = cooldownTime;

        while (cooldownValue > 0f)
        {
            MissileDisplay.UpdateCooldownImage(cooldownValue / cooldownTime);
            cooldownValue = Mathf.Max(cooldownValue - Time.deltaTime, 0f);

            yield return null;
        }

        isReady = true;
    }
}
