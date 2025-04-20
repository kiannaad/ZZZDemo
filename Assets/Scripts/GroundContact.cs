using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class GroundContact : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;
    private Vector3 verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 检测地面
        isGrounded = CheckGrounded();

        // 应用重力
        if (!isGrounded)
        {
            verticalVelocity += Physics.gravity * Time.deltaTime;
        }
        else
        {
            verticalVelocity.y = 0;
            SnapToGround(); // 强制贴地
        }

        // 移动角色（包括垂直速度）
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    private bool CheckGrounded()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position + controller.center;
        float rayLength = controller.height / 2 + groundCheckDistance;
        
        if (Physics.Raycast(rayStart, Vector3.down, out hit, rayLength, groundLayer))
        {
            return true;
        }
        return controller.isGrounded; // 备用检测
    }

    private void SnapToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance * 2, groundLayer))
        {
            // 将角色位置修正到地面
            controller.Move(Vector3.down * hit.distance);
        }
    }
}