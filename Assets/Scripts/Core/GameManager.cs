using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private GameObject enemyPrefab;

    private int enemyCount = 3;
    private Vector2 spawnMin = new Vector2(-5f, -10f);
    private Vector2 spawnMax = new Vector2(25f, 10f);

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
         SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 pos = new Vector2(Random.Range(spawnMin.x, spawnMax.x),
            Random.Range(spawnMin.y, spawnMax.y));
            Instantiate(enemyPrefab, pos, Quaternion.identity);
        }
    }
}