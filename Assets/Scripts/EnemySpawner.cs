using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;

    [Header("Spawn Chance Variables")]
    public float baseSpawnChance = 0.2f;
    public float incrementPerFloor = 0.05f;
    public float maxSpawnChance = 1f;
    
    private static int EnemyCount = 0; // mostly for debugging purposes

    public void SpawnEnemy(Vector3 roomPos)
    {
        // TODO: will also need to parent enemies to same obj as room

        bool canSpawn = CheckSpawnChance();
        int index = Random.Range(0, enemyPrefabs.Count);

        if(canSpawn) {
            Debug.Log("enemy spawned at " + roomPos);
            Vector3 spawnPos = new Vector3(roomPos.x, -5f, roomPos.z);
            Instantiate(enemyPrefabs[index], spawnPos, Quaternion.identity);

            EnemyCount++;
            Debug.Log("enemies: " + EnemyCount);
        }
        
    }

    bool CheckSpawnChance()
    {
        float spawnChance = Mathf.Min(baseSpawnChance + incrementPerFloor * LevelManager.CurrentFloor, maxSpawnChance);
        // Debug.Log("spawn chance = " + spawnChance);
        
        // Random.value returns a random float from 0 to 1
        if(Random.value < spawnChance) {
            return true;
        }
        else {
            return false;
        }
    }
}
