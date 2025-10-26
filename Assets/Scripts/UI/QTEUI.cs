using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class QTEUI : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI ShowTimer;
   [SerializeField] private Image leftHead;
   [SerializeField] private Image rightHead;
   [SerializeField] private GameObject qteObj;

   public void SetActivity(bool active) => qteObj.SetActive(active);
   
   
   
   public void ShowTime(float time)
   {
      //Debug.Log($"Show time: {time}");
    
      // 转换为整数秒（根据需求选择 Floor 或 Round）
      int totalSeconds = Mathf.FloorToInt(time);
      int minutes = totalSeconds / 60;
      int seconds = totalSeconds % 60;

      // 使用 D2 格式符
      ShowTimer.text = string.Format("{0:D2} : {1:D2}", minutes, seconds);
   }

   public void UpdateHead(Sprite leftHead, Sprite rightHead)
   {
      this.leftHead.sprite = leftHead;
      this.rightHead.sprite = rightHead;
   }

   private EventManager.UnRegisterOnDisableAndDestroyTrigger m_UnregisterTrigger;

   private void Awake()
   {
      // 必须通过 AddComponent 创建 MonoBehaviour
      m_UnregisterTrigger = gameObject.AddComponent<EventManager.UnRegisterOnDisableAndDestroyTrigger>();
     /*EventManager.Instance.RegisterEvent<OnQTEStart>(OnQTEStartHandler);
     EventManager.Instance.RegisterEvent<OnQTEEnd>(OnQTEEndHandler);
     EventManager.Instance.RegisterEvent<OnTimerCount>(OnTimerCountHandler);*/
   }

// 具名方法定义 --------------------------------------------------
   private void OnTimerCountHandler(OnTimerCount e)
   {
      ShowTime(e.time);
   }

   private void OnQTEStartHandler(OnQTEStart start)
   {
      SetActivity(true);
      UpdateHead(start.leftHead, start.rightHead);
   }

   private void OnQTEEndHandler(OnQTEEnd end)
   {
      SetActivity(false);
   }

  
}
