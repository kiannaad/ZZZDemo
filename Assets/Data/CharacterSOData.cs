using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSOData", menuName = "ScriptableObject/CharacterSOData")]
public class CharacterSOData : ScriptableObject
{
    [field : SerializeField] public MoveData moveData{get;private set;}
    
    [field : SerializeField] public AttackData AttackData{get;private set;}
    
    [field : SerializeField] public List<RecenteringData> sideRecenterData {get;private set;}
    
    [field : SerializeField] public List<RecenteringData> backRecenterData {get;private set;}
}
