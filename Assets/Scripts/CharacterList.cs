using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public enum SwitchType
{
    Wait,
    Next,
    immediate,
}

public struct OnCharacterSwitch
{
    public GameObject curCharacter;
}

public struct UpdateHeadSprite
{
    public Sprite leftHead;
    public Sprite rightHead;
}

public interface ICharacterList
{
    public VFXManager fx { get; set; }
    public void ChangeCharacter(InputAction.CallbackContext context);
}

public class CharacterList : NetworkBehaviour, ICharacterList
{
    [SerializeField] private IPlayer[] characters;
    public CinemachineVirtualCamera vcam;
    public Camera cam;
    public GameObject track;
    public VFXManager VFXManager;
    public VFXManager fx{ get => VFXManager; set{}}
    private int index = 0;
    public float switch_In_Offset;
    
    public IPlayer GetCurPlayer() => characters[index];
    public Transform GetPlayerTra(int index) => characters[index].owner.transform;
    
    private void Awake()
    {
        characters = GetComponentsInChildren<IPlayer>();
        foreach (var character in characters)
        {
            character.OnInit(this);
            character.OnAwake();
        }
        //Debug.Log($"{transform.name}: OnAwake");
    }
    
    protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
    {
        base.OnNetworkPreSpawn(ref networkManager);
        //Debug.Log($"{transform.name}: OnNetworkPreSpawn");
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        foreach (var character in characters)
        {
            character.OnStartLocalPlayer();
        }
        //EventManager.Instance.RegisterEvent<QTESwitch>(isNext  => QTEChangeCharacter(isNext.isNext));
    }

    protected override void OnNetworkPostSpawn()
    {
        base.OnNetworkPostSpawn();
        //Debug.Log($"{transform.name}: OnNetworkPostSpawn");
    }

    private void Start()
    {
        foreach (var character in characters)
        {
            character.OnStart();
        }
        Init();
        //Debug.Log($"{transform.name}: OnStart");
    }

    private void Update()
    {
        if (!IsLocalPlayer) return;
        GetCurPlayer().OnUpdate();
    }

    private void FixedUpdate()
    {
        if (!IsLocalPlayer) return;
        GetCurPlayer().OnFixedUpdate();
    }

    private void Init()
    {
        foreach (var character in characters)
        {
            if (character != characters[0])
            {
                character.owner.gameObject.SetActive(false);
                character.DisableInput();
            }
        }
        
        if (IsLocalPlayer)
        {
            ChangeCinemachineCamera(index);
            InvokeCharacterSwitch();
            InvokeUISwitch();
        }
        else
        {
            vcam.gameObject.SetActive(false);
            cam.gameObject.SetActive(false);
            track.SetActive(false);
            fx.gameObject.SetActive(false);
            GetCurPlayer().InvokeClientNoLocalHealthChange();
        }
    }

  #region Change

    public void ChangeCharacter(InputAction.CallbackContext context)
    {
        if (characters.Length < 2) return;

        int nextIndex = GetNextCharacter(index);
        //Debug.Log($"Index: {index} Next: {nextIndex}");
        if (nextIndex == -1) return;
        
        SwapCharacter(nextIndex);
        
    }
    
    public void QTEChangeCharacter(bool isNext)
    {
        if (characters.Length < 2) return;
        var nextIndex = isNext ? ((index + 1) % characters.Length): ((index - 1) < 0 ? characters.Length - 1 : index - 1); 
        SwapCharacter(nextIndex, true);
        QTETimeManager.Instance.EndQTETime();
    }

    private void SwapCharacter(int nextIndex, bool isQTE = false)
    {
        if (characters.Length == 0)
        {
            Debug.LogWarning("Character list is empty!");
            return;
        }

        if (characters[index] == null)
        {
            Debug.LogError($"Character {index} is null!");
            return;
        }

        try
        {
            Vector3 targetPos = GetCurPlayer().owner.transform.position;
            Vector3 targetRotation = GetCurPlayer().owner.transform.rotation.eulerAngles;
        
            ISwitch switch1 = GetCurPlayer().owner.GetComponent<ISwitch>();
            ISwitch switch2 = GetPlayerTra(nextIndex).GetComponent<ISwitch>();
        
            switch1.Switch_Out(targetPos, targetRotation);
            switch2.Switch_In(targetPos, targetRotation, GetPlayerTra(index).right.normalized * switch_In_Offset, isQTE);
        
            ChangeCinemachineCamera(nextIndex);
           
            index = nextIndex;
        }
        catch (Exception e)
        {
            throw new Exception($"Could not swap character {characters[index].owner.name}", e);
        }

        InvokeCharacterSwitch();
        InvokeUISwitch();

        if (IsLocalPlayer)
        {
            Debug.Log($"{GetCurPlayer().owner.transform.name}: InvokeClientNoLocalHealthChange");
            SwitchHealthNoticeServerRpc(index);
        }
    }

    

    private void ChangeCinemachineCamera(int target)
    {
        vcam.LookAt = characters[target].lookAt.transform;
        vcam.Follow = characters[target].lookAt.transform;
    }

    /*private void ChangeNetWork(int target)
    {
        _netWorkAnimator.animator = characters[target].animator;
        _networkTransformReliable.target = GetPlayerTra(target);
    }*/
    
    private int GetNextCharacter(int _index)
    {
        int nextIndex = (_index + 1) % characters.Length;
        if (nextIndex == _index)
        {
            Debug.Log("nextindex == index");
            return -1;
        }
        if (characters[nextIndex].CanSwitch() == SwitchType.Next) return GetNextCharacter(nextIndex);
        if (characters[nextIndex].CanSwitch() == SwitchType.Wait)
        {
            Debug.Log("SwitchType.Wait");
            return -1;
        }
        if (characters[nextIndex].CanSwitch() == SwitchType.immediate) return nextIndex;
        return -1;
    }
    
    private void InvokeUISwitch()
    {
            EventManager.Instance.SendEvent<UiSwitch>(new UiSwitch
            {
                getNames = GetCharacterName(),
                getActions = GetUiInit()
            });
        
    }

    private void InvokeCharacterSwitch()
    {
        if (!IsLocalPlayer) return;
    
        // 使用协程添加延迟
        StartCoroutine(DelayedCharacterSwitch());
    }

    private IEnumerator DelayedCharacterSwitch()
    {
        yield return new WaitForSeconds(0.2f);
    
        // 发送角色切换事件
        EventManager.Instance.SendEvent<OnCharacterSwitch>(new OnCharacterSwitch
        {
            curCharacter = GetPlayerTra(index).gameObject
        });
    
        // 发送头像更新事件
        EventManager.Instance.SendEvent<UpdateHeadSprite>(new UpdateHeadSprite
        {
            leftHead = GetLeftCharacterSprite(),
            rightHead = GetRightCharacterSprite()
        });
    }
    

    public List<Character_Name> GetCharacterName()
    {
        return new List<Character_Name>
        {
            characters[index].GetName(),
            characters[(index + 1) % characters.Length].GetName(),
            characters[(index + 2) % characters.Length].GetName(),
        };
    }

    public List<Action> GetUiInit()
    {
        return new List<Action>
        {
            characters[index].owner.GetComponent<CharacterStatus>().UIInit,
            characters[(index + 1) % characters.Length].owner.GetComponent<CharacterStatus>().UIInit,
            characters[(index + 2) % characters.Length].owner.GetComponent<CharacterStatus>().UIInit,
        };
    }

    public Sprite GetLeftCharacterSprite()
    {
        var preIndex = (index - 1) < 0 ? characters.Length - 1 : index - 1;
        return characters[preIndex].owner.GetComponent<CharacterStatus>().HealdSprite;
    }

    public Sprite GetRightCharacterSprite()
    {
        var nextIndex = (index + 1) % characters.Length;
        return characters[nextIndex].owner.GetComponent<CharacterStatus>().HealdSprite;
    }

    [ServerRpc]
    public void SwitchHealthNoticeServerRpc(int index) => InvokeClientHealthChangeClientRpc(index);
    
    [ClientRpc]
    public void InvokeClientHealthChangeClientRpc(int index)
    {
        if (IsLocalPlayer) return;
        characters[index].InvokeClientNoLocalHealthChange();
    }

    #endregion
}
