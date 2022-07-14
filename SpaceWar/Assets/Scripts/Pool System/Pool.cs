using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class Pool
{

    public GameObject Prefab => prefab;

    public int Size => size;
    public int RuntimeSize => queue.Count;

    [SerializeField]GameObject prefab;

    [SerializeField]int size = 1;

    Queue<GameObject> queue;

    Transform parent;



    public void Initalize(Transform parent)
    {
        this.parent = parent;
        queue = new Queue<GameObject>();
        for(var i =0;i<size;i++)
        {
            queue.Enqueue(Copy());
        }
    }

    GameObject Copy()
    {
        var copy = GameObject.Instantiate(prefab,parent);

        copy.SetActive(false);

        return copy;
    }

    GameObject  AvailableObject()
    {
        GameObject availableObject = null;
        if(queue.Count > 0 && queue.Peek().activeSelf)
        {
            availableObject = queue.Dequeue();
        }
        else
        {
            availableObject = Copy();

        }
        queue.Enqueue(availableObject);
        return availableObject;
    }

    public GameObject PrepareObject()
    {
        GameObject prepareObject = AvailableObject();

        prepareObject.SetActive(true);

        return prepareObject;
    }

    public GameObject PrepareObject(Vector3 position)
    {
        GameObject prepareObject = AvailableObject();
        prepareObject.transform.position = position;

        prepareObject.SetActive(true);

        return prepareObject;
    }

    public GameObject PrepareObject( Vector3 position,Quaternion roation)
    {
        GameObject prepareObject = AvailableObject();

        prepareObject.SetActive(true);
        prepareObject.transform.position = position;
        prepareObject.transform.rotation = roation;

        return prepareObject;
    }

    public GameObject PrepareObject(Vector3 position, Quaternion roation,Vector3 localScale )
    {
        GameObject prepareObject = AvailableObject();

        prepareObject.SetActive(true);
        prepareObject.transform.position = position;
        prepareObject.transform.rotation = roation;
        prepareObject.transform.localScale = localScale;

        return prepareObject;
    }
}
