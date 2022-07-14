using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField]Vector2 scrollVelocity;//����һ����ά�������������ƶ�����
    Material material;//����һ��material���͵ı���������
    void Awake()
    {
        //��ȡ�������
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    IEnumerator Start()
    {
        while (GameManager.GameState != GameState.GameOver)
        {
            material.mainTextureOffset += scrollVelocity * Time.deltaTime;

            yield return null;
        }
    }
}
