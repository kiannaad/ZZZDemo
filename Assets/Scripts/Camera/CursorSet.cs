using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorSet : MonoBehaviour
{
    private CinemachineInputProvider _inputProvider;

    private void Start()
    {
        _inputProvider = GetComponent<CinemachineInputProvider>();
    }

    private void Update()
    {
        if (Keyboard.current.altKey.isPressed && !Cursor.visible) ShowCursor();
        if (Mouse.current.leftButton.isPressed && Cursor.visible) HideCursor();
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _inputProvider.enabled = true;
    }

    private void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _inputProvider.enabled = false;
    }
}
