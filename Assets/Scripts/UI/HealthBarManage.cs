using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UiSwitch
{
    public List<Character_Name> getNames{get;set;}
    public List<Action> getActions{get;set;}
}

public class HealthBarManage : MonoBehaviour
{
    private List<HealthBar> healthBars;
    private EventManager.UnRegisterOnDisableAndDestroyTrigger m_UnregisterTrigger;
    [SerializeField] private GameObject healthBar;

    private void Awake()
    {
        healthBars = new List<HealthBar>(GetComponentsInChildren<HealthBar>());
        // 必须通过 AddComponent 创建 MonoBehaviour
        m_UnregisterTrigger = gameObject.AddComponent<EventManager.UnRegisterOnDisableAndDestroyTrigger>();
    }
    

    private void UpdateUI(UiSwitch uiSwitch)
    {
        for (int i = 0; i < healthBars.Count; i++)
        {
            if (healthBars[i] == null) continue;
            if (!healthBars[i].gameObject.activeInHierarchy) break;
            
            Character_Name name = uiSwitch.getNames[i];
            if (i != 0 && (name == uiSwitch.getNames[i - 1] || name == uiSwitch.getNames[(i + 1) % 3]))
            {
                healthBars[i].SetForActive(false);
            }
            else
            {
                healthBars[i].SetName(uiSwitch.getNames[i]);
            }
            
            uiSwitch.getActions[i]?.Invoke();
        }
    }

    private void OnEnable()
    {
        m_UnregisterTrigger.AddUnRegister(EventManager.Instance.RegisterEvent<UiSwitch>(UpdateUI));
        m_UnregisterTrigger.AddUnRegister(EventManager.Instance.RegisterEvent<OnQTEStart>(OnQTEStartHandler));
        m_UnregisterTrigger.AddUnRegister(EventManager.Instance.RegisterEvent<OnQTEEnd>(OnQTEEndHandler));
        
    }

// 具名事件处理方法
    private void OnQTEStartHandler(OnQTEStart start) => healthBar.SetActive(false);
    private void OnQTEEndHandler(OnQTEEnd end) => healthBar.SetActive(true);
}
