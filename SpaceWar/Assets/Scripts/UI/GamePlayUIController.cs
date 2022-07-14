using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GamePlayUIController : MonoBehaviour
{
    [Header("....PLAYER INPUT....")]
    [SerializeField] PlayInput playerInput;
    [Header("....CANVAS....")]
    [SerializeField] Canvas hUDCanvas;
    [SerializeField] Canvas menusCanvas;
    [Header("....PLAYER INPUT....")]
    [SerializeField] Button resumeButton;
    [SerializeField] Button optionButton;
    [SerializeField] Button mainMrnuButton;



    private void OnEnable()
    {
        playerInput.onPause += Pause;
        playerInput.onUnpause += Unpause; 

        //resumeButton.onClick.AddListener(OnResumeButtonClick);
        //optionButton.onClick.AddListener(OnOptionsButtonClick);
        //mainMrnuButton.onClick.AddListener(OnMainMenuButtonClick);

        ButtonPressedBehaviour.buttonFunctionTable.Add(resumeButton.gameObject.name,OnResumeButtonClick);
        ButtonPressedBehaviour.buttonFunctionTable.Add(optionButton.gameObject.name, OnOptionsButtonClick);
        ButtonPressedBehaviour.buttonFunctionTable.Add(mainMrnuButton.gameObject.name, OnMainMenuButtonClick);

    }

    private void OnDisable()
    {
        playerInput.onPause -= Pause;
        playerInput.onUnpause -= Unpause;
        //resumeButton.onClick.RemoveAllListeners();
        //optionButton.onClick.RemoveAllListeners();
        //mainMrnuButton.onClick.RemoveAllListeners();
    }

    private void Pause()
    {
        
        TimeController.Instance.Pause();
        hUDCanvas.enabled = false;
        menusCanvas.enabled = true;
        GameManager.GameState = GameState.Paused;
        playerInput.EnablePauseMenuInput();
        playerInput.SwitchToDynamicUpdateMode();
        UIInput.Instance.SelectUI(resumeButton);
    }

    private void Unpause()
    {
            
        resumeButton.Select();
        resumeButton.animator.SetTrigger("Pressed");
    }

    void OnResumeButtonClick()
    {
        TimeController.Instance.Unpause();
        hUDCanvas.enabled = true;
        menusCanvas.enabled = false;
        GameManager.GameState = GameState.Playing;
        playerInput.EnableGamePlayInput();
        playerInput.SwitchToFixedUpdateMode();
    }

    void OnOptionsButtonClick()
    {
        UIInput.Instance.SelectUI(optionButton);
        playerInput.EnablePauseMenuInput();
    }

    void OnMainMenuButtonClick()
    {
        menusCanvas.enabled = false;
        //加载主菜单场景
        SceneLoader.Instance.LoadMainMenuScene();
    }

}
