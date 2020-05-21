using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Spawn Manager")] 
    [SerializeField] private float respawnRate = 5.0f;
    [SerializeField] private List<GameObject> spawnPositions = new List<GameObject>();
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private GameObject player;

    [Header("Enemy Status")] 
    [SerializeField] private float startHealth = 100f;
    [SerializeField] private float startSpeed = 1f;
    [SerializeField] private float startDamage = 100f;

    [SerializeField] private float currentHealth;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float currentDamage;

    private float spawnTimer;
    private PrefabManager prefabManager;
    private List<GameObject> enemies = new List<GameObject>();


    void Start()
    {
        currentHealth = startHealth;
        currentSpeed = startSpeed;
        currentDamage = startDamage;
        
        enemies.Add(prefabManager.GetPrefab("Zombie"));
    }
    
    void LateUpdate()
    {
        if(spawnTimer < respawnRate)
        {
            spawnTimer += Time.deltaTime;
        }
        else
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (spawnTimer < respawnRate) return;
        
        
    }
}
