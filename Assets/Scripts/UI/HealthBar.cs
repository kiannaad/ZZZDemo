using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour, EventManager.IUnRegister
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Image Head;
    private Character_Name name;
    
    public void SetName(Character_Name name) => this.name = name;
    public void SetForActive(bool active) => gameObject?.SetActive(active);
    
    private void Start()
    {
        name = Character_Name.None;
    }

    private void OnHealthChanged(HealthChangeEvent e)
    {
        if (e.name != this.name) return;
        if (Head.sprite == null || Head.sprite != e.head) Head.sprite = e.head;
        healthBar.fillAmount = e.curHealthPersent;
    }

    private void OnEnable()
    {
        EventManager.Instance.RegisterEvent<HealthChangeEvent>(OnHealthChanged);
    }

    private void OnDisable()
    {
        UnRegister();
    }

    public void UnRegister()
    {
        EventManager.Instance.UnRegisterEvent<HealthChangeEvent>(OnHealthChanged);
    }
}
