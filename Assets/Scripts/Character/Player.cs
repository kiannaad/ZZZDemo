using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
   
    public GameInput.PlayerInputActions PlayerActions;
    public Camera cam;
    public CinemachineVirtualCamera vcam;
    public Animator animator;
    public CharacterSOData content;
    public PlayerController controller;

    public Characterlist poolType;
    
    private void Awake()
    {
        var playerInputActions = new GameInput();
        PlayerActions = playerInputActions.PlayerInput;
        EnablePlayerInput();
        animator = GetComponent<Animator>();
        controller = new PlayerController(content, this);
    }

    private void Start()
    {
        controller.stateMachine.State = StateAction.idle;
    }

    private void Update()
    {
        controller.Update();
    }

    private void FixedUpdate()
    {
       controller.FixedUpdate();
    }

    public void EnablePlayerInput() => PlayerActions.Enable();
    
    public void DisablePlayerInput() => PlayerActions.Disable();
}
