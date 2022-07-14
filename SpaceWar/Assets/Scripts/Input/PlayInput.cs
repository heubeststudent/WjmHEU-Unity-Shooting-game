using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Input Player")]
public class PlayInput : 
    ScriptableObject, 
    InputActions.IGamePlayActions,
    InputActions.IPauseMenuActions,
    InputActions.IGameOverScreenActions
{
    InputActions inputActions;//声明一个InputActions对象
    public event UnityAction<Vector2> onMove = delegate { };//移动事件二维向量
    public event UnityAction onstopMove = delegate { };//停止移动事件
    public event UnityAction onFire = delegate { };//开火发射子弹事件
    public event UnityAction onStopFire = delegate { };//停火事件
    public event UnityAction switchweap = delegate { };//转换武器威力事件
    public event UnityAction onDodge = delegate { };//玩家闪避事件
    public event UnityAction onOverdrive = delegate { };//玩家能量爆发事件
    public event UnityAction onPause = delegate { };//玩家暂停事件
    public event UnityAction onUnpause = delegate { };//玩家取消暂停事件
    public event UnityAction onLaunchMissile = delegate { };//玩家发射导弹事件
    public event UnityAction onConfirmGameOver = delegate { };//游戏结束事件

    void OnEnable()
    {
        //unity所有的流程都需要onenbale函数
       
        inputActions = new InputActions();
        inputActions.GamePlay.SetCallbacks(this);
        inputActions.PauseMenu.SetCallbacks(this);
        inputActions.GameOverScreen.SetCallbacks(this);

    }

    void OnDisable()
    {
        DisableAllInputs();
    }

    public void SwitchToDynamicUpdateMode()
    {
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
    }

    public void SwitchToFixedUpdateMode()
    {
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
    }

    void SwitchActionMap(InputActionMap actionMap,bool isUIInput)
    {
        inputActions.Disable();
        actionMap.Enable();

        if(isUIInput)
        {
            Cursor.visible = true;//游戏启用鼠标指针
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;//游戏启用将鼠标指针取消
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void DisableAllInputs()
    {
        inputActions.Disable();
    }

    public void EnableGamePlayInput()
    {
        SwitchActionMap(inputActions.GamePlay,false);
    }

    public void EnablePauseMenuInput()
    {
        SwitchActionMap(inputActions.PauseMenu,true);
    }

    public void EnableGameOverScreenInput()
    {
        SwitchActionMap(inputActions.GameOverScreen, false);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //如果摁下绑定好的按键onmove执行，如果取消按键onstopmove执行
        if(context.performed)
        {
            onMove.Invoke(context.ReadValue<Vector2>());
        }
        if(context.canceled)
        {
            onstopMove.Invoke();
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        //与移动模块相一致
        if (context.performed)
        {
            onFire.Invoke();
        }
        if (context.performed)
        {
            onStopFire.Invoke();
        }
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switchweap.Invoke();
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            onDodge.Invoke();
        }
    }

    public void OnOverdrive(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onOverdrive.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onPause.Invoke();
        }
    }

    public void OnUnpause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onUnpause.Invoke();
        }
    }

    public void OnLaunchMissile(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onLaunchMissile.Invoke();
        }
    }

    public void OnConfirmGameOver(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onConfirmGameOver.Invoke();
        }
    }

   
}
