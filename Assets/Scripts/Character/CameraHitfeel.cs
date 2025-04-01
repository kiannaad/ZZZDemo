using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraHitfeel : MonoSigleton<CameraHitfeel>
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator EnemyAnimator;
    private Coroutine pauseCoroutine;
    public CinemachineImpulseSource impulseSource;

    public override void Awake()
    {
        base.Awake();
    }

    private void Init()
    {
        
    }

    public void PS(float time)
    {
        if (playerAnimator == null)
        {
            Debug.Log("PlayerAnimator is null");
            return;
        }

        if (EnemyAnimator == null)
        {
            Debug.Log("EnemyAnimator is null");
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
        playerAnimator.speed = 0f;
        EnemyAnimator.speed = 0f;
        VFXManager.Instance.paseVFX();
        yield return new WaitForSeconds(time);
        playerAnimator.speed = 1f;
        EnemyAnimator.speed = 1f;
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
