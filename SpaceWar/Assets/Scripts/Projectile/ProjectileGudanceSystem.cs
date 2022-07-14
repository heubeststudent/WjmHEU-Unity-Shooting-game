using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGudanceSystem : MonoBehaviour
{
    [SerializeField] Projectile projectile;
    [SerializeField] float minBallisticAngle = 30f;
    [SerializeField] float maxBallisticAngle = 60f;
    float BallisticAngle;
    Vector3 targetDirection;
    public IEnumerator HomingCoroutine(GameObject target)
    {
        BallisticAngle = Random.Range(minBallisticAngle, maxBallisticAngle);
        while(gameObject.activeSelf)
        {

            if(target.activeSelf)
            {
                targetDirection = target.transform.position - transform.position;
                transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg, Vector3.forward);
                transform.rotation *= Quaternion.Euler(0f, 0f,BallisticAngle);
                projectile.Move();
            }
            else
            {
                projectile.Move();
            }
            yield return null;
        }
    }
}
