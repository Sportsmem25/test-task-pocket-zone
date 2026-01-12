using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }
    public Item[] Items;

    private Dictionary<string, Item> byId = new Dictionary<string, Item>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        foreach(var i in Items)
        {
            if (i != null && !string.IsNullOrEmpty(i.Id))
                byId[i.Id] = i;
        }
    }

    public Item Get(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        return byId.ContainsKey(id) ? byId[id] : null;
    }
}
