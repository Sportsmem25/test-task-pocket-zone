using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }
    public Item[] items;
    private Dictionary<string, Item> byId = new Dictionary<string, Item>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        foreach(var i in items)
        {
            if (i != null && !string.IsNullOrEmpty(i.id))
                byId[i.id] = i;
        }
    }

    public Item Get(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        return byId.ContainsKey(id) ? byId[id] : null;
    }
}
