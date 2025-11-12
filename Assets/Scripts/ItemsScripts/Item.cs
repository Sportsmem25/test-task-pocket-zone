using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public int maxStack = 99;
    public GameObject uiSlotPrefab;
    public abstract void Use(GameObject user);
}
