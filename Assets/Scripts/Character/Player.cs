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

    public PoolType poolType;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        PlayerActions = InputActions.Instance._playerInputActions;
        controller = new PlayerController(content, this);
    }
    
    private void Update()
    {
        controller.Update();
    }

    private void FixedUpdate()
    {
       controller.FixedUpdate();
    }

    
}
