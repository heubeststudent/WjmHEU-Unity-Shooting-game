using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class StatusBar : MonoBehaviour
{
    [SerializeField] protected Image fillImageBack;
    [SerializeField] protected Image fillImageFront;
    [SerializeField] float fillSpeed = 0.5f;
    [SerializeField] float fillDelay = 0.5f;
    [SerializeField] bool delayFill = true;

    protected float currentFillAmount;
    protected float targetFillAmount;
    float previousFillAmount;
    float t;

    Coroutine bufferedFillingCoroutine;

    WaitForSeconds waitForDelayFill;


    private void Awake()
    {  
        if(TryGetComponent<Canvas>(out Canvas canvas))
        {
            canvas.worldCamera = Camera.main;
        }
        waitForDelayFill = new WaitForSeconds(fillDelay);
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public virtual void Initialize(float currentValue,float maxValue)
    {
        currentFillAmount = currentValue / maxValue;
        targetFillAmount =  currentFillAmount;
        fillImageBack.fillAmount  = currentFillAmount;
        fillImageFront.fillAmount = currentFillAmount;
    }

    public void UpdateStatus(float currentValue, float maxValue)
    {
        targetFillAmount = currentValue / maxValue;
        if (bufferedFillingCoroutine != null)
        {
            StopCoroutine(bufferedFillingCoroutine);
        }
        //如果玩家受伤血量减少
        if(currentFillAmount>targetFillAmount)
        {
            //让前面的图片填充直接变为目标值
            fillImageFront.fillAmount = targetFillAmount;
            //让后面的图片填充慢慢减少
            bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageBack));
        }
        //状态增加时比如玩家血量增加
        if(currentFillAmount < targetFillAmount)
        {
            //让后面的图片填充直接变为目标值
            fillImageBack.fillAmount = targetFillAmount;
            //让前面的图片填充慢慢增加
            bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageFront));
        }
 
    }

    protected virtual IEnumerator BufferedFillingCoroutine(Image image)
    {
        if (delayFill)
        {
            yield return waitForDelayFill;
        }

        previousFillAmount = currentFillAmount;
        t = 0f;
        while(t<1f)
        {
            t += fillSpeed*Time.deltaTime;
            currentFillAmount = Mathf.Lerp(previousFillAmount, targetFillAmount, t);
            image.fillAmount = currentFillAmount;

            yield return null;
        }
        
    }
}
