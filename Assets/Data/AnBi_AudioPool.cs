using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnBi_AudioPool : IAudioPool
{
    ///public List<SoundItem> soundItems = new List<SoundItem>();
    [field:SerializeField] public List<SoundItem> clipItem { get; set; }
}
