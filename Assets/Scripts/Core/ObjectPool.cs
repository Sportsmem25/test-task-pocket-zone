using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [System.Serializable]
    public class PoolEntry { public string key; public GameObject prefab; public int initial = 10; };

    [SerializeField] private PoolEntry[] entries;

    private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
        
        foreach (var e in entries)
        {
            var q = new Queue<GameObject>();
            var parent = new GameObject(e.key + "_pool");
            parent.transform.SetParent(transform);
            for (int i = 0; i < e.initial; i++)
            {
                var go = Instantiate(e.prefab, parent.transform);
                go.SetActive(false);
                q.Enqueue(go);
            }
            pools[e.key] = q;
        }
    }

    public GameObject Get(string key)
    {
        if (!pools.ContainsKey(key))
        {
            Debug.LogWarning("Pool with key not found: " + key);
            return null;
        }
        var q = pools[key];
        if (q.Count == 0)
        {
            var prefab = System.Array.Find(entries, x => x.key ==
            key)?.prefab;
            if (prefab == null) return null;
            var go = Instantiate(prefab, transform);
            go.SetActive(false);
            return go;
        }
        var obj = q.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public void Release(string key, GameObject obj)
    {
        if (!pools.ContainsKey(key))
        {
            Destroy(obj); return;
        }
        obj.SetActive(false);
        pools[key].Enqueue(obj);
        obj.transform.SetParent(transform);
    }
}