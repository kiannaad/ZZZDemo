using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class Corin_AttackSystem : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerController _playerController;

    public string Run_ATK;

    public float RunATK_ColdTime;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _playerController = _player.controller;
        _player.inputActions.Skill.performed += ctx =>
        {
            if (ctx.interaction is HoldInteraction)
            {
                Loop_ATK();
            }
        };

        _player.inputActions.Skill.canceled += context =>
        {
            if (context.interaction is HoldInteraction)
            {
                Release_loop();
            }
        };

        _player.inputActions.LeftMouse.performed += ctx =>
        {
            if (_player.controller.ResuableDataAttack.comboCount == 3 && ctx.interaction is HoldInteraction)
            {
                Loop_ATK();
            }
        };
        
        _player.inputActions.LeftMouse.canceled += ctx =>
        {
            if (ctx.interaction is HoldInteraction)
            {
                Release_loop();
            }
        };
    }

    private void Update()
    {
        if (_playerController != null && (_playerController.stateMachine.State == StateAction.run || _playerController.stateMachine.State == StateAction.dash))
        {
            _playerController.ChangeATKAction(RunATK, RunATK_ColdTime, 1, AudioClipType.可琳攻击受击语音6, HitType.Corin_Hit);
        }
    }

    private void RunATK()
    {
        _animator.CrossFade(Run_ATK, 0.14f);

        //AudioClipPoolManager.Instance.PlayAudioClip(Character_Name.Corin, AudioClipType.可琳攻击5);
        
        _player.controller.ATKSet();
    }

    private void Loop_ATK()
    {
        _animator.SetBool("loop", true);
    }
    
    private void Release_loop() => _animator.SetBool("loop", false);
}
