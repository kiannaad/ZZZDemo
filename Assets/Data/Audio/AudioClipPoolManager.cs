using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioClipType
{
    foot,
    footback,
    Wind,
    Attack1,
    Attack2,
    Attack3,
    Attack4,
    Attack5,
    
    安比入鞘,
    安比一阶段说话,
    安比二阶段说话,
    安比三阶段说话,
    安比四阶段说话,
    安比技能
}

public enum PoolType
{
    AnBi_AudioPool
}

public class AudioClipPoolManager : MonoSigleton<AudioClipPoolManager>
{
    [SerializeField]private AnBi_AudioPool AnBi_AudioPool;

    private List<IAudioPool> _audioPools = new List<IAudioPool>();
    private Dictionary<string, Dictionary<AudioClipType, Queue<SoundItem>>> Pool = new Dictionary<string, Dictionary<AudioClipType, Queue<SoundItem>>>();

    private void Awake()
    {
        _audioPools.Add(AnBi_AudioPool);
        
        Init();
    }

    private void Start()
    {
       
    }

    private void Init()
    {
        foreach (var item in _audioPools)
        {
            string name = item.GetType().Name;
            if (!Pool.ContainsKey(name))
            {
                Pool.Add(name, new Dictionary<AudioClipType, Queue<SoundItem>>());
            }
            foreach (var st in item.clipItem)
            {
                if (!Pool[name].ContainsKey(st.audioType))
                {
                    Pool[name].Add(st.audioType, new Queue<SoundItem>());
                }
                
                var obj = Instantiate(st.gameObject, transform);
                
                obj.SetActive(false);
                Pool[name][st.audioType].Enqueue(obj.transform.GetComponent<SoundItem>());
               // Debug.Log(name + " : " + st.audioType + " pool");
            }
        }
    }

    public SoundItem PlayAudioClip(PoolType poolType, AudioClipType clipType)
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

        var st = Pool[name][clipType].Dequeue();
        st.gameObject.SetActive(true);
        
        Pool[name][clipType].Enqueue(st);
        return st;
    }
}
