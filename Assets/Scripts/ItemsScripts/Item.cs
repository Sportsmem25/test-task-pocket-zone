using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string Id;
    public string DisplayName;
    public Sprite Icon;
    public int MaxStack = 99;
    public GameObject UiSlotPrefab;
    public abstract void Use(GameObject user);
}
