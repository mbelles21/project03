using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, IDataPersistence
{
    public Transform enemyParent;
    public List<GameObject> enemyPrefabs;

    [Header("Spawn Chance Variables")]
    public float baseSpawnChance = 0.2f;
    public float incrementPerFloor = 0.05f;
    public float maxSpawnChance = 1f;

    public void LoadData(GameData data)
    {
        StartCoroutine(DelayLoad(data));
    }

    IEnumerator DelayLoad(GameData data)
    {
        yield return new WaitForSeconds(0.1f); // to delay spawning so enemies don't fall through the floor when loaded

        for(int i = 0; i < data.enemyPositions.Count; i++) {
            int eTypeIndex = data.enemyTypes[i];
            Vector3 ePos = data.enemyPositions[i];

            GameObject enemy = Instantiate(enemyPrefabs[eTypeIndex], ePos, Quaternion.identity);
            enemy.transform.SetParent(enemyParent);
        }
    }

    public void SaveData(ref GameData data)
    {
        if(data.enemyPositions == null || data.enemyTypes == null) {
            data.enemyPositions = new List<Vector3>();
            data.enemyTypes = new List<int>();
        }
        else {
            // clear existing data before saving
            Debug.Log("clearing enemy data");
            data.enemyPositions.Clear();
            data.enemyTypes.Clear();
        }

        if(enemyParent != null) {
            // save each enemy 
            foreach(Transform eTransform in enemyParent.transform) {
                data.enemyPositions.Add(eTransform.position);
                
                // also save enemy type
                int type = eTransform.GetComponent<EnemyID>().GetEnemyID();
                data.enemyTypes.Add(type);
            }
        }
    }

    public void SpawnEnemy(Vector3 roomPos, int roomSize)
    {
        // TODO: will also need to parent enemies to same obj as room

        // roomSize 1 means large room --> spawn 2 enemies
        int spawnCount = roomSize == 1 ? 2 : 1;

        for(int i = 0; i < spawnCount; i++) {
            bool canSpawn = CheckSpawnChance();
            int index = Random.Range(0, enemyPrefabs.Count);

            if(canSpawn) {
                Debug.Log("enemy spawned at " + roomPos);
                Vector3 spawnPos = new Vector3(roomPos.x + i * 18, -5f, roomPos.z + i * 18); // to offset second enemy pos (if applicable)
                GameObject newEnemy = Instantiate(enemyPrefabs[index], spawnPos, Quaternion.identity);
                if(enemyParent != null) {
                    Debug.Log("setting parent");
                    newEnemy.transform.SetParent(enemyParent);
                }
                else {
                    Debug.Log("no parent");
                }
            }
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
