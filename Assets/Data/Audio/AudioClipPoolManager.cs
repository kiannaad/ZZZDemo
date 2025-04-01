using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 采用接口统一池子
/// </summary>
public interface IAudioPool
{
    public List<GameObject> clipItem{get;set;}
    public Characterlist poolType { get; set; }
}

/// <summary>
/// 分池来区分不同角色的音频
/// </summary>
[System.Serializable]
public class AudioPool : IAudioPool
{
    ///public List<SoundItem> soundItems = new List<SoundItem>();
    [field:SerializeField] public List<GameObject> clipItem { get; set; }
    [field:SerializeField] public Characterlist poolType { get; set; }
}

/// <summary>
/// 缓存并且来使用
/// </summary>
public class AudioClipPoolManager : MonoSigleton<AudioClipPoolManager>
{ 
    [SerializeField]private AudioPool AnBi_Pool;

    private List<IAudioPool> _audioPools = new List<IAudioPool>();
    private Dictionary<string, Dictionary<AudioClipType, SoundItem>> Pool = new Dictionary<string, Dictionary<AudioClipType, SoundItem>>();

    public override void Awake()
    {
        base.Awake();
        _audioPools.Add(AnBi_Pool);
        
        Init();
    }

    private void Init()
    {
        foreach (var item in _audioPools)
        {
            string name = item.poolType.ToString();
            if (!Pool.ContainsKey(name))
            {
                Pool.Add(name, new Dictionary<AudioClipType, SoundItem>());
            }
            foreach (var obj in item.clipItem)
            {
                var st = obj.GetComponent<SoundItem>();
                if (!Pool[name].ContainsKey(st.audioType))
                {
                    Pool[name].Add(st.audioType, null);
                }
                
                var obj2 = Instantiate(obj, transform);
                
                Pool[name][st.audioType] = obj2.GetComponent<SoundItem>();
                obj2.SetActive(false);
                Debug.Log(name + " : " + st.audioType + " pool");
            }
        }
    }

    public SoundItem PlayAudioClip(Characterlist poolType, AudioClipType clipType)
    {
        string name = poolType.ToString();
        if (!Pool.ContainsKey(name))
        {
            Debug.LogWarning("Pool Type: " + name + " Not Found");
            return null;
        }

        if (!Pool[name].ContainsKey(clipType))
        {
            Debug.LogWarning("Pool Type: " + name + " Not Found");
            return null;
        }

        var st = Pool[name][clipType];
        st.gameObject.SetActive(true);
        
        return st;
    }
}
