using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using DG.Tweening;

public class ChatUI : NetworkBehaviour
{
    [Header("UI Components")]
    public InputField inputField;
    public Transform messageContainer;
    public GameObject messagePrefab;
    public ScrollRect scrollRect;
    public CanvasGroup scrollCanvasGroup;

    [Header("Animation Settings")]
    public float fadeTime = 1f;
    public float fadeDelay = 3f;
    private bool isDelayActive = true;

    private void Start()
    {
        inputField.gameObject.SetActive(false);
        inputField.onEndEdit.AddListener(OnInputEndEdit);
    }

    private void Update()
    {
        HandleUIAnimation();
        HandleInputActivation();
    }

    private void HandleUIAnimation()
    {
        if (!inputField.gameObject.activeSelf && !isDelayActive)
        {
            scrollCanvasGroup.DOFade(0, fadeTime).SetDelay(fadeDelay);
            isDelayActive = true;
        }
        else if (inputField.gameObject.activeSelf && !isDelayActive)
        {
            scrollCanvasGroup.alpha = 1;
        }
    }

    private void HandleInputActivation()
    {
        // 按T激活输入框
        if (!inputField.gameObject.activeSelf && Input.GetKeyDown(KeyCode.T))
        {
            ActivateChatInput();
        }
    }

    public void ActivateChatInput()
    {
        isDelayActive = false;
        inputField.gameObject.SetActive(true);
        inputField.ActivateInputField();
    }

    private void OnInputEndEdit(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            string msg = inputField.text.Trim();
            if (!string.IsNullOrEmpty(msg))
            {
                SubmitMessage(msg);
            }
            
            // 清空内容并关闭输入框
            inputField.text = "";
            inputField.gameObject.SetActive(false);
        }
    }

    public void SubmitMessage(string msg)
    {
        if (IsClient)
        {
            // 添加玩家名称前缀
            string playerName = $"Player {NetworkManager.LocalClientId}";
            string formattedMsg = $"[{playerName}]: {msg}";
            
            /*// 本地立即显示消息（减少延迟感）
            DisplayLocalMessage(formattedMsg);*/
            
            // 发送到服务器广播
            SendMessageToServerRpc(formattedMsg);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendMessageToServerRpc(string formattedMsg, ServerRpcParams rpcParams = default)
    {
        // 验证消息长度
        if (formattedMsg.Length > 100)
        {
            Debug.LogWarning("消息过长，已截断");
            formattedMsg = formattedMsg.Substring(0, 100) + "...";
        }
        
        // 广播给所有客户端
        BroadcastMessageClientRpc(formattedMsg);
    }

    [ClientRpc]
    private void BroadcastMessageClientRpc(string formattedMsg)
    {
        DisplayMessage(formattedMsg);
    }

    private void DisplayLocalMessage(string msg)
    {
        // 本地立即显示，减少延迟感
        CreateMessageObject(msg);
    }

    public void DisplayMessage(string msg)
    {
        // 确保只在客户端执行
        if (!IsClient) return;
        
        CreateMessageObject(msg);
    }

    private void CreateMessageObject(string msg)
    {
        GameObject msgObj = Instantiate(messagePrefab, messageContainer);
        Text text = msgObj.GetComponent<Text>();
        text.text = msg;

        // 自动滚动到底部
        StartCoroutine(ScrollToBottom());
    }

    private System.Collections.IEnumerator ScrollToBottom()
    {
        // 等待一帧让UI更新
        yield return null;

        scrollCanvasGroup.alpha = 1;
        
        isDelayActive = false;
        
        // 强制布局重建
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)messageContainer);
        
        // 滚动到底部
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
