using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    [System.Serializable]
    public class EnemySave
    {
        public string PrefabName; 
        public float X, Y;
        public bool IsAlive;
        public int Health;
    }

    [System.Serializable]
    public class SaveData
    {
        public float PlayerX, PlayerY;
        public int PlayerHealth;
        public List<InventorySlot> Inventory;
        public List<EnemySave> Enemies;
    }


    static string PathSave => Path.Combine(Application.persistentDataPath, "save.json");
    static string PathSaveTemp => PathCombineTemp(PathSave);

    public static string PathCombineTemp(string original)
    {
        return original + ".tmp";
    }

    public static bool HasSave() { return File.Exists(PathSave); }

    
    /// <summary>
    /// Сохранение данных об игроке, предметах, врагов
    /// </summary>
    public static void Save()
    {
        try
        {
            GameObject playerGO = GameObject.FindWithTag("Player");
            if (playerGO == null)
            {
                Debug.LogWarning("Player not found by tag");
                return;
            }
            Inventory inventory = playerGO.GetComponent<Inventory>();
            Health health = playerGO.GetComponent<Health>();

            SaveData sd = new SaveData();
            sd.PlayerX = playerGO.transform.position.x;
            sd.PlayerY = playerGO.transform.position.y;
            sd.PlayerHealth = health != null ? health.CurrentHealth : 0;
            sd.Inventory = inventory != null ? new List<InventorySlot>(inventory.Slots) : new List<InventorySlot>();
            sd.Enemies = new List<EnemySave>();
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            
            foreach (var e in enemies)
            {
                EnemySave es = new EnemySave();
                es.PrefabName = e.name.Replace("(Clone)", "").Trim();
                es.X = e.transform.position.x;
                es.Y = e.transform.position.y;
                es.IsAlive = e.activeSelf;
                var h = e.GetComponent<Health>();
                es.Health = h != null ? h.CurrentHealth : 0;
                sd.Enemies.Add(es);
            }

            string json = JsonUtility.ToJson(sd, true);
            string dir = Path.GetDirectoryName(PathSave);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(PathSaveTemp, json);

            if (File.Exists(PathSaveTemp))
            {
                if (File.Exists(PathSave))
                    File.Delete(PathSave);
                File.Move(PathSaveTemp, PathSave);
            }
            else
                File.WriteAllText(PathSave, json);

            Debug.Log("Saved to " + PathSave);
        }
        catch (Exception ex)
        {
            Debug.LogError("Save error" + ex);
            try
            {
                File.WriteAllText(PathSave, JsonUtility.ToJson(new SaveData(), true));
            }
            catch (Exception inner)
            {
                Debug.LogError("Fallback save also failed: " + inner);
            }
        }
    }

    /// <summary>
    /// Загрузка данных об игроке, предметах, врагов
    /// </summary>
    public static void Load()
    {
        try
        {
            if (!HasSave())
            {
                Debug.Log("No save file" + PathSave);
                return;
            }

            string json = File.ReadAllText(PathSave);
            SaveData sd = JsonUtility.FromJson<SaveData>(json);

            // Загрузка данных игрока
            GameObject playerGO = GameObject.FindWithTag("Player");
            playerGO.transform.position = new Vector3(sd.PlayerX, sd.PlayerY, 0f);
            Health health = playerGO.GetComponent<Health>();
            if (health != null)
            {
                health.CurrentHealth = Mathf.Clamp(sd.PlayerHealth, 0, health.MaxHealth);
                if (health.SliderHP != null)
                    health.SliderHP.value = health.CurrentHealth;
            }
            Inventory inventory = playerGO.GetComponent<Inventory>();
            if (inventory != null)
            {
                inventory.Slots = sd.Inventory != null ? new List<InventorySlot>(sd.Inventory) : new List<InventorySlot>();
                if (inventory.Ui == null)
                    inventory.Ui = GameObject.FindObjectOfType<InventoryUI>();
                inventory.Ui?.RefreshUI();
            }
            else
                Debug.LogWarning("Player not found by tag 'Player'");


            // Удаление текущих врагов и создание новых из сохранения
            foreach (var e in GameObject.FindGameObjectsWithTag("Enemy"))
                GameObject.Destroy(e);

            if (sd.Enemies != null)
            {
                foreach (var es in sd.Enemies)
                {
                    GameObject prefab = Resources.Load<GameObject>("Enemies/" + es.PrefabName);
                    if (prefab != null)
                    {
                        GameObject go = GameObject.Instantiate(prefab, new Vector3(es.X, es.Y, 0f), Quaternion.identity);
                        go.SetActive(es.IsAlive);
                        Health h = go.GetComponent<Health>();
                        if (h != null)
                        {
                            if (!es.IsAlive)
                                h.CurrentHealth = 0;
                            else
                                h.CurrentHealth = h.MaxHealth;
                            if (h.SliderHP != null)
                                h.SliderHP.value = h.CurrentHealth;
                        }
                    }
                    else
                        Debug.LogWarning("Prefab not found in Resources/Enemies: " + es.PrefabName);
                }
            }
            Debug.Log("Loaded save from " + PathSave);
        }
        catch (Exception ex)
        {
            Debug.LogError("Load error: " + ex);
        }  
    }
}