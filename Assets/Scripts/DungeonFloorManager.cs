using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonFloorManager : MonoBehaviour
{
    [Header("Floor Settings")]
    public DungeonGenerator dungeonGeneratorPrefab;
    public float verticalFloorSpacing = 50f;
    public int maxFloors = 5;
    public float baseHeight = 1f;
    [Header("UI")]
    public GameObject loadingScreenUI;

    private Dictionary<int, GameObject> floors = new Dictionary<int, GameObject>();
    private Dictionary<int, Vector3> lastStairPositions = new Dictionary<int, Vector3>();
    private int currentFloorLevel = 1;
    private bool IsGenerationComplete { get; set; } = false;

    private static DungeonFloorManager instance;
    public static DungeonFloorManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<DungeonFloorManager>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (dungeonGeneratorPrefab == null)
        {
            Debug.LogError("DungeonGenerator prefab not assigned to DungeonFloorManager!");
            return;
        }

        if (loadingScreenUI != null)
        {
            loadingScreenUI.SetActive(false);
        }

        GenerateFirstFloor();
    }

    private void GenerateFirstFloor()
    {
        GenerateNewFloor(1);
    }

    public void GenerateNewFloor(int floorLevel)
    {
        if (floorLevel > maxFloors)
        {
            Debug.LogWarning($"Attempted to generate floor {floorLevel} which exceeds max floors of {maxFloors}");
            return;
        }

        StartCoroutine(GenerateFloorSequence(floorLevel));
    }

    private IEnumerator GenerateFloorSequence(int floorLevel)
    {
        try
        {
            if (dungeonGeneratorPrefab == null)
            {
                Debug.LogError("Cannot generate floor: DungeonGenerator prefab is null!");
                yield break;
            }

            Debug.Log($"Starting generation of floor {floorLevel}");

            if (loadingScreenUI != null)
            {
                loadingScreenUI.SetActive(true);
            }

            // Create new floor container
            GameObject newFloorContainer = new GameObject($"Floor_{floorLevel}");
            float floorHeight = (verticalFloorSpacing * (floorLevel - 1)) + baseHeight;
            newFloorContainer.transform.position = new Vector3(0, floorHeight, 0);

            // Setup new DungeonGenerator
            DungeonGenerator generator = newFloorContainer.AddComponent<DungeonGenerator>();
            if (generator == null)
            {
                Debug.LogError("Failed to add DungeonGenerator component!");
                Destroy(newFloorContainer);
                yield break;
            }

            CopyGeneratorSettings(generator);

            // Handle floor management
            if (floors.ContainsKey(floorLevel))
            {
                Debug.Log($"Removing existing floor {floorLevel}");
                if (floors[floorLevel] != null)
                {
                    Destroy(floors[floorLevel]);
                    yield return null;
                }
            }

            floors[floorLevel] = newFloorContainer;

            // Wait for generation with timeout
            float timeoutDuration = 10f;
            float elapsedTime = 0f;
            while (!generator.IsGenerationComplete && elapsedTime < timeoutDuration)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (!generator.IsGenerationComplete)
            {
                Debug.LogError("Dungeon generation timed out!");
                Destroy(newFloorContainer);
                yield break;
            }

            // Hide previous floor
            if (floors.ContainsKey(currentFloorLevel) && currentFloorLevel != floorLevel)
            {
                if (floors[currentFloorLevel] != null)
                {
                    floors[currentFloorLevel].SetActive(false);
                    Debug.Log($"Deactivated floor {currentFloorLevel}");
                }
            }

            yield return StartCoroutine(TeleportPlayerToNewFloor(newFloorContainer));

            // Wait for physics to settle
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(0.2f);

            currentFloorLevel = floorLevel;
            Debug.Log($"Floor {floorLevel} generation complete");

            Debug.Log("Resetting all doors...");
            generator.ResetAllDoors();  // Use the generator's method

            DungeonConnectionHandler.Instance.ProcessConnections();

            // Wait for door connections with timeout
            elapsedTime = 0f;
            while (DungeonConnectionHandler.Instance.IsProcessingConnections && elapsedTime < timeoutDuration)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (DungeonConnectionHandler.Instance.IsProcessingConnections)
            {
                Debug.LogWarning("Door connection process timed out!");
            }

            IsGenerationComplete = true;
        }
        finally
        {
            if (loadingScreenUI != null)
            {
                loadingScreenUI.SetActive(false);
            }
        }
    }

    private IEnumerator TeleportPlayerToNewFloor(GameObject newFloor)
    {
        yield return new WaitForEndOfFrame();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found!");
            yield break;
        }

        PlayerSpawnPoint spawnPointComponent = newFloor.GetComponentInChildren<PlayerSpawnPoint>(true);
        if (spawnPointComponent != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            CharacterController cc = player.GetComponent<CharacterController>();

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
            }

            if (cc != null)
            {
                cc.enabled = false;
            }

            player.transform.position = spawnPointComponent.transform.position;

            yield return new WaitForFixedUpdate();

            if (cc != null)
            {
                cc.enabled = true;
            }

            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
        else
        {
            Debug.LogError("No spawn point found in new floor!");
        }
    }

    private void CopyGeneratorSettings(DungeonGenerator generator)
    {
        if (dungeonGeneratorPrefab == null || generator == null) return;

        generator.startingRoomPrefab = dungeonGeneratorPrefab.startingRoomPrefab;
        generator.stairRoomPrefab = dungeonGeneratorPrefab.stairRoomPrefab;
        generator.normalRoomPrefabs = dungeonGeneratorPrefab.normalRoomPrefabs;
        generator.largeRoomPrefabs = dungeonGeneratorPrefab.largeRoomPrefabs;
        generator.longRoomPrefabs = dungeonGeneratorPrefab.longRoomPrefabs;

        generator.useRandomCounts = dungeonGeneratorPrefab.useRandomCounts;
        generator.numberOfNormalRooms = dungeonGeneratorPrefab.numberOfNormalRooms;
        generator.numberOfLargeRooms = dungeonGeneratorPrefab.numberOfLargeRooms;
        generator.numberOfLongRooms = dungeonGeneratorPrefab.numberOfLongRooms;

        generator.normalRoomRange = dungeonGeneratorPrefab.normalRoomRange;
        generator.largeRoomRange = dungeonGeneratorPrefab.largeRoomRange;
        generator.longRoomRange = dungeonGeneratorPrefab.longRoomRange;
        generator.roomSpacing = dungeonGeneratorPrefab.roomSpacing;
        generator.minimumDistanceFromStart = dungeonGeneratorPrefab.minimumDistanceFromStart;

        generator.SetupEnemySpawner();

        EnemySpawner originalSpawner = dungeonGeneratorPrefab.GetComponent<EnemySpawner>();
        if (originalSpawner != null)
        {
            EnemySpawner newSpawner = generator.GetComponent<EnemySpawner>();
            newSpawner.enemyPrefabs = originalSpawner.enemyPrefabs;
            newSpawner.baseSpawnChance = originalSpawner.baseSpawnChance;
            newSpawner.incrementPerFloor = originalSpawner.incrementPerFloor;
            newSpawner.maxSpawnChance = originalSpawner.maxSpawnChance;
            Debug.Log($"Enemy spawner set up with {newSpawner.enemyPrefabs.Count} prefabs and base chance {newSpawner.baseSpawnChance}");
        }
        else
        {
            Debug.LogWarning("Original spawner not found on prefab!");
        }
    }

    public void MoveToNextFloor(Vector3 stairPosition)
    {
        int nextFloorLevel = currentFloorLevel + 1;
        if (nextFloorLevel <= maxFloors)
        {
            lastStairPositions[currentFloorLevel] = stairPosition;
            Debug.Log($"Stored stair position for floor {currentFloorLevel}: {stairPosition}");
            GenerateNewFloor(nextFloorLevel);
        }
        else
        {
            Debug.Log("Reached maximum floor level!");
        }
    }

    public void ReturnToPreviousFloor()
    {
        int previousFloor = currentFloorLevel - 1;

        if (previousFloor < 1)
        {
            Debug.Log("Already on first floor!");
            return;
        }

        if (floors.ContainsKey(previousFloor))
        {
            if (floors.ContainsKey(currentFloorLevel))
            {
                floors[currentFloorLevel].SetActive(false);
            }

            floors[previousFloor].SetActive(true);
            StartCoroutine(ReprocessPreviousFloor(previousFloor));
        }
    }

    private IEnumerator ReprocessPreviousFloor(int floorLevel)
    {
        if (!floors.ContainsKey(floorLevel)) yield break;

        GameObject floor = floors[floorLevel];
        if (floor == null)
        {
            Debug.LogError($"Floor {floorLevel} is null!");
            yield break;
        }

        DungeonGenerator generator = floor.GetComponent<DungeonGenerator>();
        if (generator != null)
        {
            generator.ResetAllDoors();
        }

        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.2f);

        DungeonConnectionHandler.Instance.ProcessConnections();

        float timeoutDuration = 5f;
        float elapsedTime = 0f;
        while (DungeonConnectionHandler.Instance.IsProcessingConnections && elapsedTime < timeoutDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (lastStairPositions.ContainsKey(floorLevel))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;

                player.transform.position = lastStairPositions[floorLevel];
                yield return new WaitForFixedUpdate();

                if (cc != null) cc.enabled = true;
            }
        }

        currentFloorLevel = floorLevel;
    }
}