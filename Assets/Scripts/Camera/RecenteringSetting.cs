using Cinemachine;
using UnityEngine;

public class RecenteringSetting
{
        private CinemachinePOV _cinemachinePov;
        
        public RecenteringSetting(CinemachineVirtualCamera cinemachineVirtualCamera)
        {
                if (cinemachineVirtualCamera == null) Debug.LogError("cinemachineVirtualCamera is null");
                _cinemachinePov = cinemachineVirtualCamera.GetCinemachineComponent<CinemachinePOV>();
        }

        private float defaultWaitTime = 0f;
        private float defaultRecenterTime = 0.5f;

        private float cancelMaxAngle = 10f;
        private float cancelMinAngle = 0f;
        
        public void EnableHorizontalRecentering() => _cinemachinePov.m_HorizontalRecentering.m_enabled = true;
        public void DisableForHorizontalRecentering() => _cinemachinePov.m_HorizontalRecentering.m_enabled = false;
        
        public void EnableVerticalRecentering() => _cinemachinePov.m_VerticalRecentering.m_enabled = true;
        
        public void DisableForVerticalRecentering() => _cinemachinePov.m_VerticalRecentering.m_enabled = false;

        private void SetForHorizontal(float waitTime, float recenterTime)
        {
                _cinemachinePov.m_HorizontalRecentering.m_WaitTime = waitTime;
                _cinemachinePov.m_VerticalRecentering.m_WaitTime = recenterTime;
        }

        private void SetForVertical(float waitTime, float recenterTime)
        {
                _cinemachinePov.m_VerticalRecentering.m_WaitTime = waitTime;
                _cinemachinePov.m_HorizontalRecentering.m_WaitTime = recenterTime;
        }

        public bool SetForCancelHorizontal(float angle)
        {
                if (angle <= cancelMinAngle || angle >= cancelMaxAngle)
                {
                        DisableForHorizontalRecentering();
                        return true;
                }

                return false;
        }

        public bool SetForCancelVertical(float angle)
        {
                if (angle >= cancelMinAngle && angle <= cancelMaxAngle)
                {
                        DisableForVerticalRecentering();
                        return true;
                }

                return false;
        }
        

        public void SetForHorizontalRecentering(float waitTime = -1f, float recenterTime = -1f)
        { 
                EnableHorizontalRecentering();
                
                if (waitTime == -1f)
                {
                        waitTime = defaultWaitTime;
                }

                if (recenterTime == -1f)
                {
                        recenterTime = defaultRecenterTime;
                }
                
                SetForHorizontal(waitTime, recenterTime);
        }

        public void SetForVerticalRecentering(float waitTime = -1f, float recenterTime = -1f)
        {
                EnableVerticalRecentering();
                 
                if (waitTime == -1f)
                {
                        waitTime = defaultWaitTime;
                }

                if (recenterTime == -1f)
                {
                        recenterTime = defaultRecenterTime;
                }
                
                SetForVertical(waitTime, recenterTime);
        }

        
        
}

