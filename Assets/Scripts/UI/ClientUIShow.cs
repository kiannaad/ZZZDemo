using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct ClientUIData
{
   public Sprite _sprite;
   public float _hp;
   public ulong _id;

   public ClientUIData(Sprite _sprite, float _hp, ulong _id)
   {
      this._sprite = _sprite;
      this._hp = _hp;
      this._id = _id;
   }
}

public class ClientUIShow : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI _hp;
   [SerializeField] private Image _head;
   private ulong _id;

   private void Awake()
   {
      EventManager.Instance.RegisterEvent<ClientUIData>(ShowClientUI);
   }

   private void Start()
   {
      gameObject.SetActive(false);
   }

   private void OnDestroy()
   {
      EventManager.Instance.UnRegisterEvent<ClientUIData>(ShowClientUI);
   }

   private void ShowClientUI(ClientUIData data)
   {
      Debug.Log($"ShowClientUI called {data._sprite.name}, UpdateHealth {data._hp}");
      gameObject.SetActive(true);
      _id = data._id;
      _hp.text = $"HP : {data._hp}";
      _head.sprite = data._sprite;
   }
}
