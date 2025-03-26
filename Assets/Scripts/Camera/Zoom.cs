using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Zoom : MonoBehaviour
{
        public float defaultPos;
        public float Min_Pos;
        public float Max_Pos;
        public float smoothValue;
        public float smoothTime;
        [SerializeField]private float velocity;
        
        private CinemachineFramingTransposer transposer;
        private CinemachineInputProvider _inputProvider;

        private void Start()
        { 
                var vca = GetComponent<CinemachineVirtualCamera>();
                transposer = vca.GetCinemachineComponent<CinemachineFramingTransposer>();
                transposer.m_CameraDistance = defaultPos;
                _inputProvider = GetComponent<CinemachineInputProvider>();
        }

        private void Update()
        {
                float inputValue = _inputProvider.GetAxisValue(2);
                
                if (inputValue == 0f) return;
                
                float curPos = transposer.m_CameraDistance;
                float targetPos = curPos + inputValue * smoothValue;

                velocity = 0f;
                float moveToTarget = Mathf.SmoothDamp(curPos, targetPos, ref velocity, smoothTime);
                
                transposer.m_CameraDistance = Mathf.Clamp(moveToTarget, Min_Pos, Max_Pos);
        }
}
