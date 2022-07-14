    using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PoolManager : MonoBehaviour
{
    [SerializeField] Pool[] enemyPools;//����һ�����˵ĳ�����

    [SerializeField]Pool[] playerProjectilePools;//����һ������ӵ��ĳ�����

    [SerializeField] Pool[] enemyProjectilePools;//����һ�������ӵ��ĳ�����

    [SerializeField] Pool[] vFXPools;//��Ч���������

    static Dictionary<GameObject, Pool> dictionary;//�ֵ�ʹ�þ�̬

    void Awake()
    {
        dictionary = new Dictionary<GameObject, Pool>();
        Initalize(enemyPools);
        Initalize(playerProjectilePools);
        Initalize(enemyProjectilePools);
        Initalize(vFXPools);
    }

    #if UNITY_EDITOR
    void OnDestroy()
    {
        CheckPoolSize(enemyPools);
        CheckPoolSize(playerProjectilePools);
        CheckPoolSize(enemyProjectilePools);
        CheckPoolSize(vFXPools);
    }
    #endif
    void CheckPoolSize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
            if(pool.RuntimeSize>pool.Size)
            {
                //Debug.LogWarning(string.Format("Pool: {0} has a runtime size {1} bigger than its initial size {2}!",
                //    pool.Prefab.name,
                //    pool.RuntimeSize,
                //    pool.Size));
            }
        }
    }
    void Initalize(Pool[] pools)
    {
        //����pool����
        foreach(var pool in pools)
        {
#if UNITY_EDITOR
            if (dictionary.ContainsKey(pool.Prefab))
            {
                Debug.LogError("Same prefab in multiple pools! Prefab: " + pool.Prefab.name);

                continue;
            }
#endif
            dictionary.Add(pool.Prefab, pool);

            Transform poolparent =  new GameObject("Pool: " +pool.Prefab.name).transform;

            poolparent.parent = transform;
            pool.Initalize(poolparent);
        }
    }

    /// <summary>
    /// <para>Release a specified prepared gameObject in the pool at specified position.</para>
    /// <para>���ݴ����prefab��������position����λ���ͷŶ������Ԥ���õ���Ϸ����</para> 
    /// </summary>
    /// <param name="prefab">
    /// <para>Specified gameObject prefab.</para>
    /// <para>ָ������Ϸ����Ԥ���塣</para>
    /// </param>
    /// <param name="position">
    /// <para>Specified release position.</para>
    /// <para>ָ���ͷ�λ�á�</para>
    /// </param>
    /// <returns></returns>
    public static GameObject Release(GameObject prefab)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);
            return null;
        }
#endif
        return dictionary[prefab].PrepareObject();
    }

    /// <summary>
    /// <para>Release a specified prepared gameObject in the pool at specified position.</para>
    /// <para>���ݴ����prefab��������position����λ���ͷŶ������Ԥ���õ���Ϸ����</para> 
    /// </summary>
    /// <param name="prefab">
    /// <para>Specified gameObject prefab.</para>
    /// <para>ָ������Ϸ����Ԥ���塣</para>
    /// </param>
    /// <param name="position">
    /// <para>Specified release position.</para>
    /// <para>ָ���ͷ�λ�á�</para>
    /// </param>
    /// <returns></returns>
    /// 
    public static GameObject Release(GameObject prefab,Vector3 position)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);
            return null;
        }
#endif
        return dictionary[prefab].PrepareObject(position);
    }

    /// <summary>
    /// <para>Release a specified prepared gameObject in the pool at specified position and rotation.</para>
    /// <para>���ݴ����prefab������rotation��������position����λ���ͷŶ������Ԥ���õ���Ϸ����</para> 
    /// </summary>
    /// <param name="prefab">
    /// <para>Specified gameObject prefab.</para>
    /// <para>ָ������Ϸ����Ԥ���塣</para>
    /// </param>
    /// <param name="position">
    /// <para>Specified release position.</para>
    /// <para>ָ���ͷ�λ�á�</para>
    /// </param>
    /// <param name="rotation">
    /// <para>Specified rotation.</para>
    /// <para>ָ������תֵ��</para>
    /// </param>
    /// <returns></returns>
    public static GameObject Release(GameObject prefab,Vector3 position, Quaternion roation)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);
            return null;
        }
#endif
        return dictionary[prefab].PrepareObject(position, roation);
    }

    /// <summary>
    /// <para>Release a specified prepared gameObject in the pool at specified position, rotation and scale.</para>
    /// <para>���ݴ����prefab����, rotation������localScale��������position����λ���ͷŶ������Ԥ���õ���Ϸ����</para> 
    /// </summary>
    /// <param name="prefab">
    /// <para>Specified gameObject prefab.</para>
    /// <para>ָ������Ϸ����Ԥ���塣</para>
    /// </param>
    /// <param name="position">
    /// <para>Specified release position.</para>
    /// <para>ָ���ͷ�λ�á�</para>
    /// </param>
    /// <param name="rotation">
    /// <para>Specified rotation.</para>
    /// <para>ָ������תֵ��</para>
    /// </param>
    /// <param name="localScale">
    /// <para>Specified scale.</para>
    /// <para>ָ��������ֵ��</para>
    /// </param>
    /// <returns></returns>
    public static GameObject Release(GameObject prefab,Vector3 position, Quaternion roation, Vector3 localScale)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);
            return null;
        }
#endif
        return dictionary[prefab].PrepareObject(position, roation, localScale);
    }

}
