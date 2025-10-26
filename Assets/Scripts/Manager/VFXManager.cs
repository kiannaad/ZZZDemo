
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct VFXData
{
    public Character_Name name;
    public VFXType type;
    public HitType hitType;
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

public class VFXManager : MonoBehaviour
{
    [SerializeField] private VFXPool Anbi_VFXPool;
    [SerializeField] private VFXPool Corin_VFXPool;
    
    private List<IVFXPool> vfxPools = new List<IVFXPool>();
    private List<VFXItem> vfxItems = new List<VFXItem>();

    private Dictionary<Character_Name, Dictionary<VFXType, VFXItem>> Pool = new Dictionary<Character_Name, Dictionary<VFXType, VFXItem>>();
    
    private Dictionary<Character_Name, Dictionary<HitType, GameObject>> OriginHitPool = new Dictionary<Character_Name, Dictionary<HitType, GameObject>>();
    private Dictionary<Character_Name, Dictionary<HitType, Queue<VFXItem>>> hitPool = new Dictionary<Character_Name, Dictionary<HitType, Queue<VFXItem>>>();
    public void Awake()
    {
        vfxPools.Add(Anbi_VFXPool);
        vfxPools.Add(Corin_VFXPool);
        
        Init();
    }

    private void Init()
    {
        foreach (var pool in vfxPools)
        {
            foreach (var vfx in pool.vfxData)
            {
                if (vfx.hitType != HitType.NULL)
                {
                    OriginHitPoolInit(vfx);

                    hitPoolInit(vfx);
                }
                else if (vfx.type != VFXType.NULL)
                {
                    PoolInit(vfx);
                }
                
            }
        }
    }

    private void PoolInit(VFXData vfx)
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

    private void hitPoolInit(VFXData vfx)
    {
        if (!hitPool.ContainsKey(vfx.name))
        {
            hitPool.Add(vfx.name, new Dictionary<HitType, Queue<VFXItem>>());
        }

        if (!hitPool[vfx.name].ContainsKey(vfx.hitType))
        {
            hitPool[vfx.name].Add(vfx.hitType, new Queue<VFXItem>());
        }

        int count = vfx.Count;
        while (count > 0)
        {
            //Debug.Log(vfx.name + ": " + count + " VFX Hit");
            count--;
            var obj = Instantiate(vfx.vfxPrefab, transform);
            hitPool[vfx.name][vfx.hitType].Enqueue(obj.gameObject.GetComponent<VFXItem>());
        }
    }

    private void OriginHitPoolInit(VFXData vfx)
    {
        if (!OriginHitPool.ContainsKey(vfx.name))
        {
            OriginHitPool.Add(vfx.name, new Dictionary<HitType, GameObject>());
        }

        if (!OriginHitPool[vfx.name].ContainsKey(vfx.hitType))
        {
            OriginHitPool[vfx.name].Add(vfx.hitType, null);
        }
                    
        OriginHitPool[vfx.name][vfx.hitType] = vfx.vfxPrefab;
    }

    public VFXItem PlayVFXItem(Character_Name name, VFXType type, Transform parent = null)
    {
        if (Pool.TryGetValue(name, out Dictionary<VFXType, VFXItem> pool))
        {
            if (Pool[name].TryGetValue(type, out VFXItem item))
            {
                item.Spawn(parent);
                return item;
            }
        }

        Debug.LogWarning("VFX pool doesn't exist");
        return null;
    }

    public VFXItem PlayHitVFXItem(Character_Name name, HitType type, Transform parent = null)
    {
        if (hitPool.TryGetValue(name, out Dictionary<HitType, Queue<VFXItem>> pool))
        {
            if (pool.TryGetValue(type, out Queue<VFXItem> queue))
            {
                if (queue.Count > 0)
                {
                    var newitem = queue.Dequeue();
                    newitem.Spawn(parent);
                    newitem.OnFinished += () => CheckPushHitPool(new vfxInfo(name, type, newitem.gameObject));
                    return newitem;
                }
            }
        }
            
        var obj = Instantiate(OriginHitPool[name][type], gameObject.transform);
        var item = obj.GetComponent<VFXItem>();
        item.Spawn(parent);
        item.OnFinished += () => CheckPushHitPool(new vfxInfo(name, type, item.gameObject));
        return obj.GetComponent<VFXItem>();
    }

    struct vfxInfo
    {
        public vfxInfo(Character_Name name, HitType type, GameObject vfxPrefab)
        {
            this.name = name;
            this.hitType = type;
            this.vfx = vfxPrefab;
        }
        
        public Character_Name name;
        public HitType hitType;
        public GameObject vfx;
    }

    private void CheckPushHitPool(vfxInfo info)
    {
        if (hitPool.Count > 10)
        {
            Destroy(info.vfx);
            return;
        }

        hitPool[info.name][info.hitType].Enqueue(info.vfx.GetComponent<VFXItem>());
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
