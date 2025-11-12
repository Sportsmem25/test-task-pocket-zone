using UnityEngine;

public class SaveLoadButtons : MonoBehaviour
{
    public void SaveGame()
    {
        SaveSystem.Save();
    }

    public void LoadGame()
    {
        SaveSystem.Load();
    }
}