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
        if (dungeonGeneratorPrefab == null)
        {
            Debug.LogError("Cannot generate floor: DungeonGenerator prefab is null!");
            yield break;
        }

        Debug.Log($"Starting generation of floor {floorLevel}");

        // Show loading screen if available
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
        CopyGeneratorSettings(generator);

        // Handle floor management
        if (floors.ContainsKey(floorLevel))
        {
            Debug.Log($"Removing existing floor {floorLevel}");
            Destroy(floors[floorLevel]);
        }
        floors[floorLevel] = newFloorContainer;

        // Wait for dungeon generation to complete
        yield return new WaitUntil(() => generator.IsGenerationComplete);

        // Hide previous floor if it exists
        if (floors.ContainsKey(currentFloorLevel) && currentFloorLevel != floorLevel)
        {
            floors[currentFloorLevel].SetActive(false);
            Debug.Log($"Deactivated floor {currentFloorLevel}");
        }

        // Handle player teleportation
        yield return StartCoroutine(TeleportPlayerToNewFloor(newFloorContainer));

        currentFloorLevel = floorLevel;
        Debug.Log($"Floor {floorLevel} generation complete");

        // Hide loading screen
        if (loadingScreenUI != null)
        {
            loadingScreenUI.SetActive(false);
        }
    }

    private IEnumerator TeleportPlayerToNewFloor(GameObject newFloor)
    {
        // Wait for floor to be ready but before physics update
        yield return new WaitForEndOfFrame();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found!");
            yield break;
        }

        // Find spawn point
        PlayerSpawnPoint spawnPointComponent = newFloor.GetComponentInChildren<PlayerSpawnPoint>(true);
        if (spawnPointComponent != null)
        {
            // Disable physics components temporarily
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

            // Set position instantly
            player.transform.position = spawnPointComponent.transform.position;

            // Re-enable physics components
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

    private Transform FindChildRecursively(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                Debug.Log($"Found child '{childName}' at position {child.position}");
                return child;
            }

            Transform result = FindChildRecursively(child, childName);
            if (result != null)
                return result;
        }
        return null;
    }

    private void CopyGeneratorSettings(DungeonGenerator generator)
    {
        if (dungeonGeneratorPrefab == null || generator == null) return;

        // Copy all room prefabs
        generator.startingRoomPrefab = dungeonGeneratorPrefab.startingRoomPrefab;
        generator.stairRoomPrefab = dungeonGeneratorPrefab.stairRoomPrefab;
        generator.normalRoomPrefabs = dungeonGeneratorPrefab.normalRoomPrefabs;
        generator.largeRoomPrefabs = dungeonGeneratorPrefab.largeRoomPrefabs;
        generator.longRoomPrefabs = dungeonGeneratorPrefab.longRoomPrefabs;

        // Copy generation settings
        generator.useRandomCounts = dungeonGeneratorPrefab.useRandomCounts;
        generator.numberOfNormalRooms = dungeonGeneratorPrefab.numberOfNormalRooms;
        generator.numberOfLargeRooms = dungeonGeneratorPrefab.numberOfLargeRooms;
        generator.numberOfLongRooms = dungeonGeneratorPrefab.numberOfLongRooms;

        // Copy random generation ranges
        generator.normalRoomRange = dungeonGeneratorPrefab.normalRoomRange;
        generator.largeRoomRange = dungeonGeneratorPrefab.largeRoomRange;
        generator.longRoomRange = dungeonGeneratorPrefab.longRoomRange;

        generator.roomSpacing = dungeonGeneratorPrefab.roomSpacing;
        generator.minimumDistanceFromStart = dungeonGeneratorPrefab.minimumDistanceFromStart;
    }
    public void MoveToNextFloor(Vector3 stairPosition)
    {
        int nextFloorLevel = currentFloorLevel + 1;
        if (nextFloorLevel <= maxFloors)
        {
            // Store the stair position for the current floor
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
            // Hide current floor
            if (floors.ContainsKey(currentFloorLevel))
            {
                floors[currentFloorLevel].SetActive(false);
            }

            // Show previous floor
            floors[previousFloor].SetActive(true);

            // Reprocess the door connections for the previous floor
            StartCoroutine(ReprocessFloorConnections(previousFloor));
        }
    }

    private IEnumerator ReprocessFloorConnections(int floorLevel)
    {
        if (floors.ContainsKey(floorLevel))
        {
            GameObject floor = floors[floorLevel];

            // Reset all doors first
            var roomBehaviours = floor.GetComponentsInChildren<RoomBehaviour>();
            foreach (var room in roomBehaviours)
            {
                for (int i = 0; i < room.wallSections.Length; i++)
                {
                    room.ShowWall(i);
                }
            }

            yield return new WaitForSeconds(0.1f);

            // Reprocess connections
            DungeonConnectionHandler.Instance.ProcessConnections();

            // Teleport player to the stored stair position
            if (lastStairPositions.ContainsKey(floorLevel))
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    CharacterController cc = player.GetComponent<CharacterController>();
                    if (cc != null) cc.enabled = false;

                    player.transform.position = lastStairPositions[floorLevel];

                    if (cc != null) cc.enabled = true;
                }
            }

            currentFloorLevel = floorLevel;
        }
    }
}