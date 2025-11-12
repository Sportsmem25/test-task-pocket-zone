using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    [System.Serializable]
    public class EnemySave
    {
        public string prefabName; 
        public float x, y;
        public bool alive;
        public int health;
    }

    [System.Serializable]
    public class SaveData
    {
        public float playerX, playerY;
        public int playerHealth;
        public List<InventorySlot> inventory;
        public List<EnemySave> enemies;
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
            var playerGO = GameObject.FindWithTag("Player");
            if (playerGO == null)
            {
                Debug.LogWarning("Player not found by tag");
                return;
            }
            var inventory = playerGO.GetComponent<Inventory>();
            var health = playerGO.GetComponent<Health>();

            var sd = new SaveData();
            sd.playerX = playerGO.transform.position.x;
            sd.playerY = playerGO.transform.position.y;
            sd.playerHealth = health != null ? health.currentHealth : 0;
            sd.inventory = inventory != null ? new List<InventorySlot>(inventory.slots) : new List<InventorySlot>();
            sd.enemies = new List<EnemySave>();
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var e in enemies)
            {
                var es = new EnemySave();
                es.prefabName = e.name.Replace("(Clone)", "").Trim();
                es.x = e.transform.position.x;
                es.y = e.transform.position.y;
                es.alive = e.activeSelf;
                var h = e.GetComponent<Health>();
                es.health = h != null ? h.currentHealth : 0;
                sd.enemies.Add(es);
            }
            var json = JsonUtility.ToJson(sd, true);
            var dir = Path.GetDirectoryName(PathSave);
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
            var json = File.ReadAllText(PathSave);
            var sd = JsonUtility.FromJson<SaveData>(json);

            // Загрузка данных игрока
            var playerGO = GameObject.FindWithTag("Player");
            playerGO.transform.position = new Vector3(sd.playerX, sd.playerY, 0f);
            var health = playerGO.GetComponent<Health>();
            if (health != null)
            {
                health.currentHealth = Mathf.Clamp(sd.playerHealth, 0, health.maxHealth);
                if (health.sliderHP != null)
                    health.sliderHP.value = health.currentHealth;
            }
            var inventory = playerGO.GetComponent<Inventory>();
            if (inventory != null)
            {
                inventory.slots = sd.inventory != null ? new List<InventorySlot>(sd.inventory) : new List<InventorySlot>();
                if (inventory.ui == null)
                    inventory.ui = GameObject.FindObjectOfType<InventoryUI>();
                inventory.ui?.RefreshUI();
            }
            else
                Debug.LogWarning("Player not found by tag 'Player'");


            // Удаление текущих врагов и создание новых из сохранения
            foreach (var e in GameObject.FindGameObjectsWithTag("Enemy"))
                GameObject.Destroy(e);

            if (sd.enemies != null)
            {
                foreach (var es in sd.enemies)
                {
                    var prefab = Resources.Load<GameObject>("Enemies/" + es.prefabName);
                    if (prefab != null)
                    {
                        var go = GameObject.Instantiate(prefab, new Vector3(es.x, es.y, 0f), Quaternion.identity);
                        go.SetActive(es.alive);
                        var h = go.GetComponent<Health>();
                        if (h != null)
                        {
                            if (!es.alive)
                                h.currentHealth = 0;
                            else
                                h.currentHealth = h.maxHealth;
                            if (h.sliderHP != null)
                                h.sliderHP.value = h.currentHealth;
                        }
                    }
                    else
                        Debug.LogWarning("Prefab not found in Resources/Enemies: " + es.prefabName);
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