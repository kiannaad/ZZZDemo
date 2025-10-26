using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private NetworkManager networkManager;
    [SerializeField] private TMP_InputField _ip;
    [SerializeField] private TMP_InputField _port;
    [SerializeField] private Button _server;
    [SerializeField] private Button _client;

    private void Awake()
    {
        networkManager = NetworkManager.Singleton;
    }

    private void Start()
    {
        _ip.text = "";
        _port.text = "";
        EventManager.Instance.RegisterEvent<WaitUIData>(OnWaitUIOpen);
        _server.onClick.AddListener(OnServerBtnClick);
        _client.onClick.AddListener(OnClientBtnClick);
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnRegisterEvent<WaitUIData>(OnWaitUIOpen);
    }

    private void OnServerBtnClick()
    {
        networkManager.StartServer();
        //networkManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        EventManager.Instance.SendEvent<WaitUIData>(new WaitUIData("WaitForClient...", true));
    }

    private void OnClientBtnClick()
    {
        if (_ip.text != "" && _port.text != "")
        {
            if (networkManager.NetworkConfig.NetworkTransport is UnityTransport unityTransport)
            {
                unityTransport.SetConnectionData(_ip.text, ushort.Parse(_port.text));
            }
        }
        networkManager.StartClient();
        EventManager.Instance.SendEvent<WaitUIData>(new WaitUIData("WaitForServer...", true));
        //networkManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    private void OnWaitUIOpen(WaitUIData data)
    {
        if (data.isOpen)
        {
            gameObject.SetActive(false);
        }
    }
}
