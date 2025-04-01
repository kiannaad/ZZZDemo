using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SwitchCamera : MonoSigleton<SwitchCamera>
{
   private CinemachineBrain brain;

   [System.Serializable]
   public class CharacterCameraList
   {
      public nameType name;
      public comboType combo;
      public CinemachineStateDrivenCamera stateDrivenCamera;
   }
   
   public List<CharacterCameraList> characterCameras = new List<CharacterCameraList>();

   private Dictionary<nameType, Dictionary<comboType, CinemachineStateDrivenCamera>> cinemachinePool;

   public void Awake()
   {
      cinemachinePool = new Dictionary<nameType, Dictionary<comboType, CinemachineStateDrivenCamera>>();
      brain = GetComponent<CinemachineBrain>();
      InitPool();
   }

   /// <summary>
   /// 获得对轨道摄像机的引用。
   /// </summary>
   public void InitPool()
   {
      if (characterCameras.Count == 0) return;

      foreach (var characterCamera in characterCameras)
      {
         if (!cinemachinePool.ContainsKey(characterCamera.name))
         {
            cinemachinePool.Add(characterCamera.name, new Dictionary<comboType, CinemachineStateDrivenCamera>());
         }

         if (!cinemachinePool[characterCamera.name].ContainsKey(characterCamera.combo))
         {
            cinemachinePool[characterCamera.name].Add(characterCamera.combo, characterCamera.stateDrivenCamera);
         }

         //characterCamera.stateDrivenCamera.gameObject.SetActive(false);
         characterCamera.stateDrivenCamera.Priority = 0;
      }
   }

   /// <summary>
   /// 平滑的混合过渡。
   /// </summary>
   /// <param name="name"></param>
   /// <param name="combo"></param>
   public void BlendSwitchToCamera(nameType name, comboType combo)
   {
      if (!cinemachinePool.ContainsKey(name)) return;
      
      CinemachineStateDrivenCamera cameraStateDrivenCamera = cinemachinePool[name][combo];
      
     // cameraStateDrivenCamera.gameObject.SetActive(true);
      cameraStateDrivenCamera.Priority = 20;
   }

   public void UnBlendSwitchToCamera(nameType name, comboType combo)
   {
      if (!cinemachinePool.ContainsKey(name)) return;
      
      CinemachineStateDrivenCamera cameraStateDrivenCamera = cinemachinePool[name][combo];

      //cameraStateDrivenCamera.gameObject.SetActive(false);
      cameraStateDrivenCamera.Priority = 0;
   }

   /// <summary>
   /// 取消混合时间来实现立即过渡。
   /// </summary>
   /// <param name="name"></param>
   /// <param name="combo"></param>
   public void ImmediateSwitchToCamera(nameType name, comboType combo)
   {
      if (!cinemachinePool.ContainsKey(name)) return;
      
      CinemachineStateDrivenCamera cameraStateDrivenCamera = cinemachinePool[name][combo];

      brain.m_DefaultBlend.m_Time = 0f;
      //cameraStateDrivenCamera.gameObject.SetActive(true);
      cameraStateDrivenCamera.Priority = 20;
      
   }

   public void UnImmediateSwitchToCamera(nameType name, comboType combo)
   {
      if (!cinemachinePool.ContainsKey(name)) return;
      
      CinemachineStateDrivenCamera cameraStateDrivenCamera = cinemachinePool[name][combo];
      
      brain.m_DefaultBlend.m_Time = 1f;
      //cameraStateDrivenCamera.gameObject.SetActive(false);
      cameraStateDrivenCamera.Priority = 0;
   }
   
}
