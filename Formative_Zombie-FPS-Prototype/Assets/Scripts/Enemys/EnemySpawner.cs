using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject zombiePrefab;

    [SerializeField] private GameObject target;
    [SerializeField] private float currentDamage;
    [SerializeField] private float currentMoveSpeed;
    [SerializeField] private float currentHealth;

    [SerializeField] private int zombiesSpawned;
    
    [SerializeField] private List<Transform> spawnPositionsList = new List<Transform>(); 
 
    private float spawnTimer;
    private List<GameObject> enemies = new List<GameObject>();


    void Start()
    {
        
    }
    
    void LateUpdate()
    {
        if (zombiesSpawned < 10)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        int spawnPos = Random.Range(0, spawnPositionsList.Count); // choose a random spawn pos
        GameObject newZombie = Instantiate(zombiePrefab, spawnPositionsList[spawnPos].transform.position, Quaternion.identity);// spawns a zombie there
        enemies.Add(newZombie);

        spawnPositionsList.Remove(spawnPositionsList[spawnPos]);// removes the last spawn pos, so that every pos can spawn only one enemy
        zombiesSpawned += 1;
        
        //GameObject zombie = newZombie;
        
        newZombie.GetComponent<ZombieAi>().Target = target;
        newZombie.GetComponent<ZombieAi>().Damage = currentDamage;
        newZombie.GetComponent<NavMeshAgent>().speed = currentMoveSpeed;
        newZombie.GetComponent<HealthManager>().SetHealth(currentHealth);
    }
}
