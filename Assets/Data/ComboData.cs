using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum comboType
{
    LightCombo,
    SkillCombo
}

[System.Serializable]
public class ComboData
{
    public string comboName;
    public comboType comboType;
    public float coldTime;
    public float attackDistance;
    public float damage;

     public AudioClipType sweaponSound;
     public AudioClipType CharacterSounds;
}
