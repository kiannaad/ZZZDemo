using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraHitfeel : MonoSigleton<CameraHitfeel>
{
    [SerializeField] private List<Animator> playerAnimator;
    [SerializeField] private List<Animator> EnemyAnimator;
    private Coroutine pauseCoroutine;
    public CinemachineImpulseSource impulseSource;
    
    private bool DontPauseEnemy { get; set; }
    public void EnableDontPauseEnemy() => DontPauseEnemy = true;
    public void DisableDontPauseEnemy() => DontPauseEnemy = false;

    public void AddAni_Enemy(Animator animator) => EnemyAnimator.Add(animator);
    public void AddAni_Player(Animator animator) => playerAnimator.Add(animator);

    private void Start()
    {
        DontPauseEnemy = false;
    }

    public void SetAllEnemy(float time)
    {
       // Debug.Log("set enemy animation");
        EnemyAnimator.RemoveAll((Animator a) =>
        {
            if (a == null) return true;
            else
            {
                a.speed = time;
                return false;
            }
        });
    }

    public void SetAllPlayer(float time)
    {
        playerAnimator.RemoveAll((ani =>
        {
            if (ani == null) return true;
            else
            {
                ani.speed = time;
                return false;
            }
        }));

    }

    public void PS(float time)
    {
        if (playerAnimator == null)
        {
            //Debug.Log("PlayerAnimator is null");
            return;
        }

        if (EnemyAnimator == null)
        {
            //Debug.Log("EnemyAnimator is null");
            return;
        }

        if (pauseCoroutine != null)
        {
            StopCoroutine(pauseCoroutine);
        }

        pauseCoroutine = StartCoroutine(pauseTime(time));
    }
    
    IEnumerator pauseTime(float time)
    {
        if (!DontPauseEnemy)
            SetAllEnemy(0f);
        
        SetAllPlayer(0f);
        VFXManager.Instance.paseVFX();
        yield return new WaitForSeconds(time);
        
        if (!DontPauseEnemy)
            SetAllEnemy(1f);
        
        SetAllPlayer(1f);
        VFXManager.Instance.resetVFX(1f);
    }

    public void slowTime(float time)
    {
        Time.timeScale = time;
    }

    public void slowTimeEnd()
    {
        Time.timeScale = 1f;   
    }

    public void ShakeCamera(float shakeForce)
    {
       // Debug.Log("Shaking camera " + shakeForce);
        impulseSource.GenerateImpulseWithForce(shakeForce); 
    }
}
