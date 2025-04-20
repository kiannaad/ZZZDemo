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

public class CharacterList : MonoSigleton<CharacterList>
{
    [SerializeField] private Player[] characters;
    public CinemachineVirtualCamera vcam;
    //public bool isSwitching = false;
    private int index = 0;
    public Action<GameObject> OnCharacterSelected;
    public float switch_In_Offset;
    
    public GameObject GetCurPlayer() => characters[index].gameObject;
    
    private void Awake()
    {
        characters = GetComponentsInChildren<Player>();
    }

    private void Start()
    {
        Init();
    }

    public void ChangeCharacter(InputAction.CallbackContext context)
    {
        if (characters.Length < 2) return;

        int nextIndex = GetNextCharacter(index);
        Debug.Log($"Index: {index} Next: {nextIndex}");
        if (nextIndex == -1) return;
        
        Vector3 targetPos = characters[index].transform.position;
        Vector3 targetRotation = characters[index].transform.rotation.eulerAngles;
        
        ISwitch switch1 = characters[index].GetComponent<ISwitch>();
        ISwitch switch2 = characters[nextIndex].GetComponent<ISwitch>();
        
        switch1.Switch_Out(targetPos, targetRotation);
        
        switch2.Switch_In(targetPos, targetRotation, characters[index].transform.right * switch_In_Offset);
        
        
        ChangeCinemachineCamera(nextIndex);
        OnCharacterSelected?.Invoke(characters[nextIndex].gameObject);
        
        index = nextIndex;
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
    }
}
