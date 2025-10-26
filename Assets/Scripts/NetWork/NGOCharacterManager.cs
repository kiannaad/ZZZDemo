using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NGOCharacterManager : NetworkManager
{
    private SpawnPointsSetting _spawnPointsSetting;
    
    enum GameState
    {
        Menu,
        Start,
        Playing,
        End,
    }
    
    GameState gameState = GameState.Menu;
    private bool isSceneLoading = false;
    private bool hasSpawnedPlayers = false;
    
    private void Awake()
    {
        _spawnPointsSetting = GetComponent<SpawnPointsSetting>();
        
        // 一次性注册客户端连接事件
        OnClientConnectedCallback += OnClientConnected;
    }
    
    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"客户端连接: {clientId}");
        
        // 只在特定条件下加载场景
        if (ShouldLoadStartScene())
        {
            LoadStartScene();
        }
    }
    
    private bool ShouldLoadStartScene()
    {
        // 检查所有条件
        return gameState == GameState.Menu && 
               IsServer && 
               ConnectedClientsIds.Count >= 2 &&
               !isSceneLoading &&
               !hasSpawnedPlayers;
    }
    
    private void LoadStartScene()
    {
        if (isSceneLoading)
        {
            Debug.LogWarning("场景正在加载中，跳过重复加载");
            return;
        }
        
        Debug.Log("开始加载场景");
        isSceneLoading = true;
        gameState = GameState.Start;
        
        // 一次性注册场景事件（避免重复注册）
        SceneManager.OnSceneEvent -= OnSceneEvent; // 先取消之前的注册
        SceneManager.OnSceneEvent += OnSceneEvent;
        
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
    
    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        Debug.Log($"场景事件: {sceneEvent.SceneEventType} - {sceneEvent.SceneName}");
        
        // 只处理加载完成事件
        if (sceneEvent.SceneEventType == SceneEventType.LoadComplete && 
            sceneEvent.SceneName == "SampleScene")
        {
            Debug.Log($"场景加载完成: {sceneEvent.SceneName}");
            
            if (IsServer)
            {
                // 延迟生成玩家，确保场景完全加载
                StartCoroutine(SpawnPlayersAfterSceneReady());
            }
        }
    }
    
    private IEnumerator SpawnPlayersAfterSceneReady()
    {
        // 等待场景完全稳定
        yield return new WaitForSeconds(0.5f);
        
        SpawnPlayers();
        
        // 重置加载状态
        isSceneLoading = false;
        gameState = GameState.Playing;
        
        // 取消事件订阅，避免重复触发
        SceneManager.OnSceneEvent -= OnSceneEvent;
    }
    
    private void SpawnPlayers()
    {
        if (hasSpawnedPlayers)
        {
            Debug.LogWarning("玩家已生成，跳过重复生成");
            return;
        }
        
        Debug.Log($"开始生成玩家，客户端数量: {ConnectedClientsIds.Count}");
        
        foreach (ulong clientId in ConnectedClientsIds)
        {
            SpawnPlayerForClient(clientId);
        }
        
        hasSpawnedPlayers = true;
        Debug.Log("所有玩家生成完成");
    }
    
    private void SpawnPlayerForClient(ulong clientId)
    {
        try
        {
            Debug.Log($"为客户端 {clientId} 生成玩家");
            
            GameObject playerPrefab = _spawnPointsSetting.player.gameObject;
            if (playerPrefab == null)
            {
                Debug.LogError("玩家预制体为空");
                return;
            }
            
            // 实例化玩家
            GameObject playerInstance = Instantiate(playerPrefab);
            NetworkObject netObj = playerInstance.GetComponent<NetworkObject>();
            
            if (netObj == null)
            {
                Debug.LogError("玩家预制体缺少 NetworkObject 组件");
                Destroy(playerInstance);
                return;
            }
            
            // 生成玩家对象
            netObj.SpawnAsPlayerObject(clientId);
            netObj.gameObject.transform.position = _spawnPointsSetting.GetPoint();
            Debug.Log($"玩家生成成功: {clientId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"生成玩家失败: {e.Message}");
        }
    }
    
    
    
}
