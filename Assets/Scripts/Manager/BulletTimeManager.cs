using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimeManager : MonoSigleton<BulletTimeManager>
{
    public bool bulletTime { get; private set; }

    [SerializeField] private float slowEnemy;
    [SerializeField] private float slowPlayer;
    [SerializeField] private float slowPlayer_Continue;
    [SerializeField] private float Max_BulletTime;
    private float BulletTimer;

    public Action OnBulletTime_Started;
    public Action OnBulletTime_Ended;
    
    public bool isInBulletTime() => bulletTime;
    
    private void EnablebulletTime()
    {
        BulletTimer = Max_BulletTime;
        bulletTime = true;
        
        CameraHitfeel.Instance.EnableDontPauseEnemy();
    }
    private void DisablebulletTime()
    {
        BulletTimer = 0f;
        bulletTime = false;
        
        CameraHitfeel.Instance.DisableDontPauseEnemy();
    }

    private void Start()
    {
        bulletTime = false;
        BulletTimer = 0f;
    }

    private void Update()
    {
        if (BulletTimer > 0) BulletTimer -= Time.deltaTime;
        if (BulletTimer <= 0 && bulletTime)
        {
            EndBulletTime();
        }
    }

    public void StartBulletTime()
    {
        if (bulletTime) return;
        
        EnablebulletTime();
        try
        {
            CameraHitfeel.Instance.SetAllEnemy(slowEnemy);
        }
        catch (MissingReferenceException  e)
        {
            throw new MissingReferenceException(e.ToString());
        }
        StartCoroutine(SlowPlayerAni());
        OnBulletTime_Started?.Invoke();
    }

    private IEnumerator SlowPlayerAni()
    {
        CameraHitfeel.Instance.SetAllPlayer(slowPlayer);
        yield return new WaitForSeconds(slowPlayer_Continue);
        CameraHitfeel.Instance.SetAllPlayer(1f);
    }

    public void EndBulletTime()
    {
        if (!bulletTime) return;
        
        DisablebulletTime();
        CameraHitfeel.Instance.SetAllEnemy(1f);
        OnBulletTime_Ended?.Invoke();
    }
}
