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
    InputActions inputActions;//����һ��InputActions����
    public event UnityAction<Vector2> onMove = delegate { };//�ƶ��¼���ά����
    public event UnityAction onstopMove = delegate { };//ֹͣ�ƶ��¼�
    public event UnityAction onFire = delegate { };//�������ӵ��¼�
    public event UnityAction onStopFire = delegate { };//ͣ���¼�
    public event UnityAction switchweap = delegate { };//ת�����������¼�
    public event UnityAction onDodge = delegate { };//��������¼�
    public event UnityAction onOverdrive = delegate { };//������������¼�
    public event UnityAction onPause = delegate { };//�����ͣ�¼�
    public event UnityAction onUnpause = delegate { };//���ȡ����ͣ�¼�
    public event UnityAction onLaunchMissile = delegate { };//��ҷ��䵼���¼�
    public event UnityAction onConfirmGameOver = delegate { };//��Ϸ�����¼�

    void OnEnable()
    {
        //unity���е����̶���Ҫonenbale����
       
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
            Cursor.visible = true;//��Ϸ�������ָ��
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;//��Ϸ���ý����ָ��ȡ��
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
        //������°󶨺õİ���onmoveִ�У����ȡ������onstopmoveִ��
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
        //���ƶ�ģ����һ��
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
