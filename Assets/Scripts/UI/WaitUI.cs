using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct WaitUIData
{
    public string message;
    public bool isOpen;

    public WaitUIData(string message, bool isOpen)
    {
        this.message = message;
        this.isOpen = isOpen;
    }
}

public class WaitUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    
    private void Start()
    {
        EventManager.Instance.RegisterEvent<WaitUIData>(HandleWaitUIData);
        gameObject.SetActive(false);
    }
    
    private void HandleWaitUIData(WaitUIData data)
    {
        Debug.Log($"HandleWaitUIData {data.isOpen} isnull : {this == null}");
        // 添加空检查
        if (this == null) return;
        
        Debug.Log($"HandleWaitUIData {data.isOpen}");
        _text.text = data.message;
        gameObject.SetActive(data.isOpen);
    }
    
    private void OnDestroy()
    {
        Debug.Log("WaitUIData OnDestroy");
        // 额外确保注销
        if (EventManager.Instance != null)
        {
            EventManager.Instance.UnRegisterEvent<WaitUIData>(HandleWaitUIData);
        }
    }
}