using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Clip Data", menuName = "Audio Clip Data")]
public class AudioClipData : ScriptableObject
{
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();

    public AudioClip GetRandomAudioClip()
    {
        return audioClips[Random.Range(0, audioClips.Count)];
    }

    public AudioClip GetIndexAudioClip(int index)
    {
        return audioClips[index];
    }
}
