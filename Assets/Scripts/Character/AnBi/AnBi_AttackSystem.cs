using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Serialization;

public class AnBi_AttackSystem : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerController _playerController;

    public string Run_ATK;
    public string Power_ATK;

    public float RunATK_ColdTime;
    public float PowerATK_ColdTime;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _playerController = _player.controller;
        _player.inputActions.LeftMouse.performed += ctx =>
        {
            //Debug.Log(_playerController.ResuableDataAttack.comboCount == 3 && ctx.interaction is HoldInteraction );
            if (_playerController.ResuableDataAttack.comboCount == 3 && ctx.interaction is HoldInteraction)
            {
                //Debug.Log("power atk performed");
                _playerController.ChangeATKAction(PowerATK, PowerATK_ColdTime, 10, AudioClipType.安比攻击受击7, HitType.AnBi_Hit,true);
            }
        };
    }

    private void Update()
    {
        if (_playerController != null && _playerController.stateMachine.State == StateAction.run)
        {
            _playerController.ChangeATKAction(RunATK, RunATK_ColdTime,10, AudioClipType.安比攻击受击7, HitType.AnBi_Hit);
        }
    }

    private void RunATK()
    {
        _animator.CrossFade(Run_ATK, 0.14f);

        AudioClipPoolManager.Instance.PlayAudioClip(Character_Name.AnBi, AudioClipType.安比闪A音效);
        
        _player.controller.ATKSet();
    }

    private void PowerATK()
    {
        _animator.CrossFade(Power_ATK, 0.14f);

        AudioClipPoolManager.Instance.PlayAudioClip(Character_Name.AnBi, AudioClipType.Attack5);
        
        _player.controller.ATKSet();
    }
}
