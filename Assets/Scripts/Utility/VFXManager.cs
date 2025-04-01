using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct VFXData
{
    public Characterlist name;
    public VFXType type;
    public GameObject vfxPrefab;
    public Transform parent;
    public int Count;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public Vector3 scaleOffset;
}

public interface IVFXPool
{
     public List<VFXData> vfxData { get; set; }
}

[System.Serializable]
public class VFXPool : IVFXPool
{
   [field : SerializeField] public List<VFXData> vfxData { get; set; }
}

public class VFXManager : MonoSigleton<VFXManager>
{
    [SerializeField] private VFXPool Anbi_VFXPool;
    
    private List<IVFXPool> vfxPools = new List<IVFXPool>();
    private List<VFXItem> vfxItems = new List<VFXItem>();

    private Dictionary<Characterlist, Dictionary<VFXType, VFXItem>> Pool = new Dictionary<Characterlist, Dictionary<VFXType, VFXItem>>();
    public override void Awake()
    {
        base.Awake();
        vfxPools.Add(Anbi_VFXPool);
        
        Init();
    }

    private void Init()
    {
        foreach (var pool in vfxPools)
        {
            foreach (var vfx in pool.vfxData)
            {
                if (!Pool.ContainsKey(vfx.name))
                {
                    Pool.Add(vfx.name, new Dictionary<VFXType, VFXItem>());
                }

                if (!Pool[vfx.name].ContainsKey(vfx.type))
                {
                    Pool[vfx.name].Add(vfx.type, null);
                }
                
                if (vfx.vfxPrefab != null)
                {
                    GameObject obj = Instantiate(vfx.vfxPrefab, Vector3.zero, Quaternion.identity);
                    var item = obj.GetComponent<VFXItem>();
                    obj.SetActive(false);
                    if (vfx.parent != null)
                    {
                        obj.transform.SetParent(vfx.parent, false);
                    }
                    else
                    {
                        obj.transform.SetParent(transform, false);
                    }

                    Pool[vfx.name][vfx.type] = item;
                    vfxItems.Add(item);
                   // Debug.Log(vfx.vfxPrefab.name);
                }
            }
        }
    }

    public VFXItem GetVFXItem(Characterlist name, VFXType type)
    {
        if (Pool.TryGetValue(name, out Dictionary<VFXType, VFXItem> pool))
        {
            if (Pool[name].TryGetValue(type, out VFXItem item))
            {
                item.Spawm();
                return item;
            }
        }

        Debug.LogWarning("VFX pool doesn't exist");
        return null;
    }

    public void paseVFX()
    {
        foreach (var item in vfxItems)
        {
            var main = item.ps.main;
            main.simulationSpeed = 0f;
        }
    }

    public void resetVFX(float speedMult)
    {
        foreach (var item in vfxItems)
        {
            var main = item.ps.main;
            main.simulationSpeed = speedMult;
        }
    }
}
