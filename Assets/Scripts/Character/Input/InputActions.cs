using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputActions : MonoSigleton<InputActions>
{
    public GameInput.PlayerInputActions _playerInputActions;

    private void Awake()
    {
        var playerInputActions = new GameInput();
        _playerInputActions = playerInputActions.PlayerInput;
        
    }

    private void Start()
    {
        
    }
    
    public void EnablePlayerInput() => _playerInputActions.Enable();
    
    public void DisablePlayerInput() => _playerInputActions.Disable();
}
