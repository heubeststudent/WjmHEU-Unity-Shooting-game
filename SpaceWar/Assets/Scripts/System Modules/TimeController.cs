using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : Singleton<TimeController>
{
    [SerializeField,Range(0f,1f)] float bulletTimeScale = 0.1f;
    float defaultFixedDeltaTime;
    float timeScaleBeforePause;
    float t;

    protected override void Awake()
    {
        base.Awake();
        defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void Pause()
    {
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0f;   
    }

    public void Unpause()
    {
        Time.timeScale = timeScaleBeforePause;
        Time.timeScale = 1f;

    }

    public void BulleTime(float duration)
    {
        Time.timeScale = bulletTimeScale;
        //Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
        StartCoroutine(SlowOutCoroutine(duration));
    }

    public void BulleTime(float inDuration, float outDuration)
    {
        Time.timeScale = bulletTimeScale;
        StartCoroutine(SlowInAndOutCoroutine(inDuration, outDuration));
    }

    public void BulleTime(float inDuration, float keepinDuration, float outDuration)
    {
        StartCoroutine(SlowInKeepAndOutCoroutine(inDuration, keepinDuration,outDuration));
    }

    public void SlowIn(float duration)
    {
        StartCoroutine(SlowInCoroutine(duration));
    }

    public void SlowOut(float duration)
    {
        StartCoroutine(SlowOutCoroutine(duration));
    }

    IEnumerator SlowInKeepAndOutCoroutine(float inDuration,float keepinDuration, float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration));
        yield return new WaitForSecondsRealtime(keepinDuration);
        StartCoroutine(SlowOutCoroutine(outDuration));
    }

    IEnumerator SlowInCoroutine(float duration)
    {
        t = 0f;
        while (t < 1f)
        {
            if (GameManager.GameState != GameState.Paused)
            {
                t += Time.unscaledDeltaTime / duration;
                Time.timeScale = Mathf.Lerp(1f,bulletTimeScale, t);
                Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            }
                
            yield return null;
        }
    }

    IEnumerator SlowOutCoroutine(float duration)
    {
        //float starttime = Time.unscaledTime;
        t = 0f;
        while(t<1f)
        {
           if(GameManager.GameState != GameState.Paused)
            {
                t += Time.unscaledDeltaTime / duration;
                Time.timeScale = Mathf.Lerp(bulletTimeScale,1f,t);
                Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            }
            
            yield return null;  
        }
        //Debug.Log("time is "+ (Time.unscaledTime-starttime));
    }

    IEnumerator SlowInAndOutCoroutine(float inDuration, float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration));

        StartCoroutine(SlowOutCoroutine(outDuration));
    }
}
