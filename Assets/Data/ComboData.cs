using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum comboType
{
    LightCombo,
    SkillCombo,
    FinishSkillCombo
}

public enum nameType
{
    AnBi
}

[System.Serializable]
public class ComboData
{
    public nameType nameType;
    public string comboName;
    public comboType comboType;
    public float coldTime;
    public float attackDistance;
    public float damage;

     public AudioClipType sweaponSound;
     public AudioClipType CharacterSounds;

     public float ShakeForce;
}
