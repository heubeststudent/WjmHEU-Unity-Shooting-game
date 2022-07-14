using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : PersistentSingleton<SceneLoader>
{
    [SerializeField] Image transitionImage;
    [SerializeField] float FadeTime = 3.5f;

    Color color;

    const string GAMEPLAY = "GamePlay";
    const string MAIN_MENU = "MainMenu";
    const string SCORING = "Scoring";

    IEnumerator  LoadCoroutine(string sceneName)
    {
        var loadingOperation =  SceneManager.LoadSceneAsync(sceneName);
        loadingOperation.allowSceneActivation = false;
        transitionImage.gameObject.SetActive(true);
        while(color.a<1f)
        {
            color.a =  Mathf.Clamp01( color.a + Time.unscaledDeltaTime / FadeTime);
            transitionImage.color = color;
            yield return null;
        }
        yield return new WaitUntil(()=>loadingOperation.progress>=0.9f);
        loadingOperation.allowSceneActivation = true;
        while (color.a > 0f)
        {
            color.a = Mathf.Clamp01(color.a - Time.unscaledDeltaTime / FadeTime);
            transitionImage.color = color;
            yield return null;
        }
        transitionImage.gameObject.SetActive(false);

    }

    public void LoadScoringScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadCoroutine(SCORING));

    }

    public void LoadgameplayScene()
    {
        //load("GAMEPLAY");
        StopAllCoroutines();
        StartCoroutine(LoadCoroutine(GAMEPLAY));
    }

    public void LoadMainMenuScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadCoroutine(MAIN_MENU));
    }
}
