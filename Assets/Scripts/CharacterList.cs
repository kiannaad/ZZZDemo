using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

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

public class CharacterList : MonoBehaviour
{
    [SerializeField] private Player[] characters;
    public CinemachineVirtualCamera vcam;
    //public bool isSwitching = false;
    private int index = 0;
    //public Action<GameObject> OnCharacterSelected;
    public float switch_In_Offset;
    
    public GameObject GetCurPlayer() => characters[index].gameObject;
    
    private void Awake()
    {
        characters = GetComponentsInChildren<Player>();
        EventManager.Instance.RegisterEvent<QTESwitch>(isNext  => QTEChangeCharacter(isNext.isNext));
    }

    private void Start()
    {
        Init();
    }

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
            Vector3 targetPos = characters[index].transform.position;
            Vector3 targetRotation = characters[index].transform.rotation.eulerAngles;
        
            ISwitch switch1 = characters[index].GetComponent<ISwitch>();
            ISwitch switch2 = characters[nextIndex].GetComponent<ISwitch>();
        
            switch1.Switch_Out(targetPos, targetRotation);
        
            switch2.Switch_In(targetPos, targetRotation, characters[index].transform.right * switch_In_Offset, isQTE);
        
            ChangeCinemachineCamera(nextIndex);
            index = nextIndex;
        }
        catch (Exception e)
        {
            throw new Exception($"Could not swap character {characters[index].name}", e);
        }

        InvokeCharacterSwitch();
        InvokeUISwitch();
    }

    

    private void ChangeCinemachineCamera(int target)
    {
        vcam.LookAt = characters[target].lookAt.transform;
        vcam.Follow = characters[target].lookAt.transform;
    }
    
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
    

    private void Init()
    {
        foreach (var character in characters)
        {
            if (character != characters[0])
            {
                character.gameObject.SetActive(false);
                character.DisableInput();
            }
        }
        ChangeCinemachineCamera(index);
        InvokeCharacterSwitch();
        InvokeUISwitch();
    }
    
    private void InvokeUISwitch() => EventManager.Instance.SendEvent<UiSwitch>(new UiSwitch
    {
        getNames = GetCharacterName(),
        getActions = GetUiInit()
    });

    private void InvokeCharacterSwitch()
    {
        EventManager.Instance.SendEvent<OnCharacterSwitch>(new OnCharacterSwitch{curCharacter = characters[index].gameObject});
        EventManager.Instance.SendEvent<UpdateHeadSprite>(new UpdateHeadSprite{leftHead = GetLeftCharacterSprite(), rightHead = GetRightCharacterSprite()});
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
            characters[index].gameObject.GetComponent<CharacterStatus>().UIInit,
            characters[(index + 1) % characters.Length].gameObject.GetComponent<CharacterStatus>().UIInit,
            characters[(index + 2) % characters.Length].gameObject.GetComponent<CharacterStatus>().UIInit,
        };
    }

    public Sprite GetLeftCharacterSprite()
    {
        var preIndex = (index - 1) < 0 ? characters.Length - 1 : index - 1;
        return characters[preIndex].gameObject.GetComponent<CharacterStatus>().HealdSprite;
    }

    public Sprite GetRightCharacterSprite()
    {
        var nextIndex = (index + 1) % characters.Length;
        return characters[nextIndex].gameObject.GetComponent<CharacterStatus>().HealdSprite;
    }
    
}
