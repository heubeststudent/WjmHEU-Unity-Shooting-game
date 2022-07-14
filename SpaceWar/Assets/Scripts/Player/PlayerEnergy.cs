using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : Singleton<PlayerEnergy>
{
    [SerializeField]EnergyBar energyBar;
    float overdriveInterval = 0.1f;
    public const int MAX = 100;
    public const int PERCENT = 1;
    int energy;
    bool available = true;

    WaitForSeconds waitForOverdriveInterval;

    protected override void Awake()
    {
        base.Awake();
        waitForOverdriveInterval = new WaitForSeconds(overdriveInterval);
    }

    private void OnEnable()
    {
        PlayerOverdrive.on += PlayerOverdriveOn;
        PlayerOverdrive.off += PlayerOverdriveOff;
    }

    private void Start()
    {
        energyBar.Initialize(energy,MAX);
        Obtain(MAX);
    }

    public void Obtain(int value)
    {
        if(energy == MAX || !available ||!gameObject.activeSelf)
        {
            return;
        }
        energy = Mathf.Clamp(energy + value, 0, MAX);
        energyBar.UpdateStatus(energy, MAX);
    }

    public void Use(int value)
    {
        energy -= value;
        energyBar.UpdateStatus(energy, MAX);

        if(energy == 0 && !available)
        {
            PlayerOverdrive.off.Invoke();
        }
    }

    public bool IsEnough(int value)
    {
        return energy >= value;
    }

    void PlayerOverdriveOn()
    {
        available = false;
        StartCoroutine(nameof(KeepingUsingCoroutine));
    }

    void PlayerOverdriveOff()
    {
        available = true;
        StopCoroutine(nameof(KeepingUsingCoroutine));
    }

    IEnumerator KeepingUsingCoroutine()
    {
        while(gameObject.activeSelf &&energy>0)
        {
            yield return waitForOverdriveInterval;

            Use(PERCENT);
        }
    }


}
