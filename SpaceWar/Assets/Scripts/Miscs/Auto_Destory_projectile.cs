using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auto_Destory_projectile : MonoBehaviour
{
    [SerializeField]bool destroyGameObeject;
    [SerializeField]float lifetime = 2f;
    WaitForSeconds waitLifeTime;

    private void Awake()
    {
        waitLifeTime = new WaitForSeconds(lifetime);
    }
    void OnEnable()
    {
        StartCoroutine(DeactiveCorountine());
    }

    IEnumerator DeactiveCorountine()
    {
        yield return waitLifeTime;
        if(destroyGameObeject)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
