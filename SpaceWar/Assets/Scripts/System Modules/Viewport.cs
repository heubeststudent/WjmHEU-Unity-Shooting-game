using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewport : Singleton<Viewport>
{
    float minX;
    float maxX;
    float minY;
    float maxY;
    float middleX;
    public float MaxX => maxX;
    void Start()
    {
        Camera mainCamera = Camera.main;

        Vector2 bottomleft = mainCamera.ViewportToWorldPoint(new Vector3(0f,0f));
        minX = bottomleft.x;
        minY = bottomleft.y;
        Vector2 topright = mainCamera.ViewportToWorldPoint(new Vector3(1f,1f));
        maxX = topright.x;
        maxY = topright.y;
        middleX = mainCamera.ViewportToWorldPoint(new Vector3(0.5f,0f,0f)).x;

    }

    public Vector3 PlayerMoveablePosition(Vector3 playerposition,float paddingx,float paddingy)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Clamp(playerposition.x, minX+paddingx, maxX-paddingx);
        position.y = Mathf.Clamp(playerposition.y, minY+paddingy, maxY-paddingy);

        return position;
    }

    public Vector3 RadomEnemiesPawPosition(float paddingx, float paddingy)
    {
        Vector3 position = Vector3.zero;
        position.x = maxX + paddingx;
        position.y = Random.Range(minY+paddingy,maxY-paddingy);

        return position;
    }

    public Vector3 RandomRightHalfPosition(float paddingx, float paddingy)
    {
        Vector3 position = Vector3.zero;
        position.x = Random.Range(middleX, maxX - paddingx);
        position.y = Random.Range(minY + paddingy, maxY - paddingy);
        return position;
    }
}
