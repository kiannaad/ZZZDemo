using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OnTimerCount
{
    public float time;
}

public struct OnQTEStart
{
    public Sprite leftHead;
    public Sprite rightHead;
}

public struct OnQTEEnd
{
    
}

public struct QTESwitch
{
    public bool isNext;
}

public class QTETimeManager : MonoSigleton<QTETimeManager>
{
    private GameInput.QTEInputActions inputActions;
    [SerializeField] private float timeToWait;
    [SerializeField] private float SlowTime;

    private float timer = 0f;
    
    private bool isQTERunning = false;
    
    private Sprite LeftHead;
    private Sprite RightHead;

    private float Timer
    {
        get
        {
            return timer;
        }
        set
        {
            if (value < 0f) return;
            timer = value;
            EventManager.Instance.SendEvent<OnTimerCount>(new OnTimerCount{ time = timer });
        }
    }

    public override void Awake()
    {
        base.Awake();
        var input = new GameInput();
        inputActions = input.QTEInput;
        inputActions.leftMouse.performed += ctx => EventManager.Instance.SendEvent<QTESwitch>(new QTESwitch{ isNext = false });
        inputActions.rightMouse.performed += ctx => EventManager.Instance.SendEvent<QTESwitch>(new QTESwitch{ isNext = true });
        EventManager.Instance.RegisterEvent<UpdateHeadSprite>(UpdateSprite);   
    }

    private void Update()
    {
        if (Timer > 0.1f)
        {
            Timer -= Time.deltaTime; 
        }

        if (Timer <= 0.1f && isQTERunning)
        {
            EndQTETime();
        }
    }

    public void StartQTETime()
    {
        isQTERunning = true;
        
        CameraHitfeel.Instance.SetAllEnemy(SlowTime);
        CameraHitfeel.Instance.SetAllPlayer(SlowTime);
        EventManager.Instance.SendEvent<OnQTEStart>(new OnQTEStart
        {
            leftHead = LeftHead,
            rightHead = RightHead
        });
        
        Timer = timeToWait;
        inputActions.Enable();
    }

    public void EndQTETime()
    {
        isQTERunning = false;
        
        inputActions.Disable();
        EventManager.Instance.SendEvent<OnQTEEnd>();
        CameraHitfeel.Instance.SetAllPlayer(1f);
        CameraHitfeel.Instance.SetAllEnemy(1f);
    }

    private void UpdateSprite(UpdateHeadSprite updateHeadSprite)
    {
        LeftHead = updateHeadSprite.leftHead;
        RightHead = updateHeadSprite.rightHead;
    }
    
    
}
