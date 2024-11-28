using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject player;
    
    [Header("Room Prefabs")]
    public GameObject startingRoomPrefab;
    public GameObject stairRoomPrefab;
    public GameObject[] normalRoomPrefabs;
    public GameObject[] largeRoomPrefabs;
    public GameObject[] longRoomPrefabs;

    [Header("Generation Mode")]
    public bool useRandomCounts = false;

    [Header("Fixed Generation Settings")]
    [Tooltip("Only used when Random Generation is disabled")]
    public int numberOfNormalRooms = 5;
    public int numberOfLargeRooms = 1;
    public int numberOfLongRooms = 2;

    [Header("Random Generation Settings")]
    [Tooltip("Only used when Random Generation is enabled")]
    public Vector2Int normalRoomRange = new Vector2Int(3, 8);
    public Vector2Int largeRoomRange = new Vector2Int(0, 2);
    public Vector2Int longRoomRange = new Vector2Int(1, 3);

    [Header("Common Settings")]
    public float roomSpacing = 18f;
    public int minimumDistanceFromStart = 3;

    private HashSet<Vector2Int> occupiedTiles = new HashSet<Vector2Int>();
    private List<GameObject> generatedRooms = new List<GameObject>();
    private Vector2Int startRoomPosition = Vector2Int.zero;

    private EnemySpawner enemySpawner;


    void Start()
    {
        enemySpawner = GetComponent<EnemySpawner>();

        if (stairRoomPrefab == null)
        {
            Debug.LogError("Stair Room Prefab is not assigned!");
            return;
        }
        StartCoroutine(GenerateDungeonSequence());
    }

    private void DetermineRoomCounts(out int normalCount, out int largeCount, out int longCount)
    {
        if (useRandomCounts)
        {
            normalCount = Random.Range(normalRoomRange.x, normalRoomRange.y + 1);
            largeCount = Random.Range(largeRoomRange.x, largeRoomRange.y + 1);
            longCount = Random.Range(longRoomRange.x, longRoomRange.y + 1);

            Debug.Log($"Random generation selected: {normalCount} normal rooms, {largeCount} large rooms, {longCount} long rooms");
        }
        else
        {
            normalCount = numberOfNormalRooms;
            largeCount = numberOfLargeRooms;
            longCount = numberOfLongRooms;
        }
    }

    IEnumerator GenerateDungeonSequence()
    {
        bool successfulGeneration = false;
        int maxAttempts = 3;
        int currentAttempt = 0;

        while (!successfulGeneration && currentAttempt < maxAttempts)
        {
            currentAttempt++;
            Debug.Log($"Attempting dungeon generation (Attempt {currentAttempt}/{maxAttempts})...");

            // Determine room counts
            int normalRooms, largeRooms, longRooms;
            DetermineRoomCounts(out normalRooms, out largeRooms, out longRooms);

            // Clear existing rooms
            foreach (var room in generatedRooms)
            {
                if (room != null) Destroy(room);
            }
            generatedRooms.Clear();
            occupiedTiles.Clear();

            // Place starting room
            PlaceRoom(startingRoomPrefab, Vector2Int.zero, RoomSize.Normal, enemiesAllowed: false);
            startRoomPosition = Vector2Int.zero;
            player.transform.position = new Vector3(0, -5, 0); // move player to starting room
            yield return null;

            // Place large rooms first
            for (int i = 0; i < largeRooms; i++)
            {
                TryPlaceLargeRoom();
                yield return null;
            }

            // Place long rooms
            for (int i = 0; i < longRooms; i++)
            {
                TryPlaceLongRoom();
                yield return null;
            }

            // Place some normal rooms before stair room
            int halfNormal = normalRooms / 2;
            for (int i = 0; i < halfNormal; i++)
            {
                TryPlaceNormalRoom();
                yield return null;
            }

            // Try to place stair room with progressively shorter distances if needed
            int currentMinDistance = minimumDistanceFromStart;
            bool stairPlaced = false;

            while (currentMinDistance >= 2 && !stairPlaced)
            {
                Debug.Log($"Attempting to place stair room (minimum distance: {currentMinDistance})...");
                stairPlaced = TryPlaceStairRoom(currentMinDistance);
                if (!stairPlaced)
                {
                    currentMinDistance--;
                    yield return null;
                }
            }

            if (stairPlaced)
            {
                // Place remaining normal rooms
                for (int i = halfNormal; i < normalRooms; i++)
                {
                    TryPlaceNormalRoom();
                    yield return null;
                }

                successfulGeneration = true;
                Debug.Log("Dungeon generation successful!");
            }
            else
            {
                Debug.Log("Failed to place stair room, retrying generation...");
                yield return new WaitForSeconds(0.1f);
                continue;
            }
        }

        if (!successfulGeneration)
        {
            Debug.LogError($"Failed to generate valid dungeon after {maxAttempts} attempts!");
            yield break;
        }

        yield return new WaitForSeconds(0.2f);

        Debug.Log("Resetting all doors...");
        ResetAllDoors();

        DungeonConnectionHandler.Instance.ProcessConnections();
    }

    // void GenerateDungeon()
    // {
    //     // Place starting room
    //     PlaceRoom(startingRoomPrefab, Vector2Int.zero, RoomSize.Normal);

    //     // Place large rooms first (since they need more space)
    //     for (int i = 0; i < numberOfLargeRooms; i++)
    //     {
    //         TryPlaceLargeRoom();
    //     }

    //     // Place long rooms
    //     for (int i = 0; i < numberOfLongRooms; i++)
    //     {
    //         TryPlaceLongRoom();
    //     }

    //     // Fill in with normal rooms
    //     for (int i = 0; i < numberOfNormalRooms; i++)
    //     {
    //         TryPlaceNormalRoom();
    //     }

    //     // ProcessDoorConnections();
    //     Invoke("ForceDoorCheck", 0.5f);

    // }

    enum RoomSize { Normal, Large, Long }

    private bool CanPlaceRoomAt(Vector2Int position, RoomSize size)
    {
        switch (size)
        {
            case RoomSize.Normal:
                return !occupiedTiles.Contains(position);

            case RoomSize.Large:
                // Check 2x2 area
                for (int x = 0; x < 2; x++)
                    for (int y = 0; y < 2; y++)
                        if (occupiedTiles.Contains(position + new Vector2Int(x, y)))
                            return false;
                return true;

            case RoomSize.Long:
                // Check 1x2 area
                return !occupiedTiles.Contains(position) &&
                       !occupiedTiles.Contains(position + Vector2Int.up);

            default:
                return false;
        }
    }

    private void OccupyTiles(Vector2Int position, RoomSize size)
    {
        switch (size)
        {
            case RoomSize.Normal:
                occupiedTiles.Add(position);
                break;

            case RoomSize.Large:
                for (int x = 0; x < 2; x++)
                    for (int y = 0; y < 2; y++)
                        occupiedTiles.Add(position + new Vector2Int(x, y));
                break;

            case RoomSize.Long:
                occupiedTiles.Add(position);
                occupiedTiles.Add(position + Vector2Int.up);
                break;
        }
    }

    private List<Vector2Int> GetValidPositionsForSize(RoomSize size)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();
        HashSet<Vector2Int> checkedPositions = new HashSet<Vector2Int>();

        // Get positions adjacent to existing rooms
        foreach (Vector2Int occupied in occupiedTiles)
        {
            CheckAdjacentPosition(occupied + Vector2Int.up, size, validPositions, checkedPositions);
            CheckAdjacentPosition(occupied + Vector2Int.down, size, validPositions, checkedPositions);
            CheckAdjacentPosition(occupied + Vector2Int.left, size, validPositions, checkedPositions);
            CheckAdjacentPosition(occupied + Vector2Int.right, size, validPositions, checkedPositions);
        }

        return validPositions;
    }

    private void CheckAdjacentPosition(Vector2Int pos, RoomSize size, List<Vector2Int> validPositions, HashSet<Vector2Int> checkedPositions)
    {
        if (checkedPositions.Contains(pos)) return;
        checkedPositions.Add(pos);

        if (CanPlaceRoomAt(pos, size) && HasAdjacentRoom(pos))
        {
            validPositions.Add(pos);
        }
    }

    private bool HasAdjacentRoom(Vector2Int pos)
    {
        return occupiedTiles.Contains(pos + Vector2Int.up) ||
               occupiedTiles.Contains(pos + Vector2Int.down) ||
               occupiedTiles.Contains(pos + Vector2Int.left) ||
               occupiedTiles.Contains(pos + Vector2Int.right);
    }


    private void TryPlaceNormalRoom()
    {
        if (normalRoomPrefabs == null || normalRoomPrefabs.Length == 0) return;

        var validPositions = GetValidPositionsForSize(RoomSize.Normal);
        if (validPositions.Count > 0)
        {
            Vector2Int pos = validPositions[Random.Range(0, validPositions.Count)];
            GameObject prefab = normalRoomPrefabs[Random.Range(0, normalRoomPrefabs.Length)];
            PlaceRoom(prefab, pos, RoomSize.Normal);
        }
    }

    private void TryPlaceLargeRoom()
    {
        if (largeRoomPrefabs == null || largeRoomPrefabs.Length == 0) return;

        var validPositions = GetValidPositionsForSize(RoomSize.Large);
        if (validPositions.Count > 0)
        {
            Vector2Int pos = validPositions[Random.Range(0, validPositions.Count)];
            GameObject prefab = largeRoomPrefabs[Random.Range(0, largeRoomPrefabs.Length)];
            PlaceRoom(prefab, pos, RoomSize.Large);
        }
    }

    private void TryPlaceLongRoom()
    {
        if (longRoomPrefabs == null || longRoomPrefabs.Length == 0) return;

        var validPositions = GetValidPositionsForSize(RoomSize.Long);
        if (validPositions.Count > 0)
        {
            Vector2Int pos = validPositions[Random.Range(0, validPositions.Count)];
            GameObject prefab = longRoomPrefabs[Random.Range(0, longRoomPrefabs.Length)];
            PlaceRoom(prefab, pos, RoomSize.Long);
        }
    }

    private bool TryPlaceStairRoom(int minDistance)
    {
        var validPositions = GetValidPositionsForStairs(minDistance);
        Debug.Log($"Found {validPositions.Count} potential positions for stair room (min distance: {minDistance})");

        if (validPositions.Count > 0)
        {
            Vector2Int pos = validPositions[Random.Range(0, validPositions.Count)];
            Debug.Log($"Placing stair room at position: {pos}");
            PlaceRoom(stairRoomPrefab, pos, RoomSize.Normal, enemiesAllowed: false);
            return true;
        }
        return false;
    }

    private void PlaceRoom(GameObject prefab, Vector2Int gridPos, RoomSize size, bool enemiesAllowed = true)
    {
        Vector3 worldPos = new Vector3(gridPos.x * roomSpacing, 0, gridPos.y * roomSpacing);
        GameObject room = Instantiate(prefab, worldPos, Quaternion.identity);
        room.transform.SetParent(transform);
        generatedRooms.Add(room);
        OccupyTiles(gridPos, size);

        RoomBehaviour roomBehaviour = room.GetComponent<RoomBehaviour>();
        if (roomBehaviour != null)
        {
            // Start with walls showing and doorways hidden
            for (int i = 0; i < roomBehaviour.wallSections.Length; i++)
            {
                roomBehaviour.ShowWall(i);
            }
        }

        // start logic to spawn enemies
        if(enemySpawner != null && enemiesAllowed) {
            enemySpawner.SpawnEnemy(worldPos);
        }        
    }

    private int GetDoorIndex(string doorName)
    {
        switch (doorName)
        {
            case "NorthPoint": return 0;
            case "SouthPoint": return 1;
            case "EastPoint": return 2;
            case "WestPoint": return 3;
            default: return -1;
        }
    }

    void ForceDoorCheck()
    {
        DoorPoint[] allDoors = FindObjectsByType<DoorPoint>(FindObjectsSortMode.None);
        Debug.Log($"Checking {allDoors.Length} doors");

        // First, make sure all doorways are closed
        foreach (DoorPoint door in allDoors)
        {
            RoomBehaviour room = door.GetComponentInParent<RoomBehaviour>();
            int index = GetDoorIndex(door.gameObject.name);

            if (room != null && room.wallSections != null && index >= 0)
            {
                var wallSection = room.wallSections[index];
                if (wallSection.completeWall != null)
                {
                    wallSection.completeWall.SetActive(true);
                    Debug.Log($"Setting wall active for {door.gameObject.name}");
                }
                if (wallSection.doorwayOpen != null)
                {
                    wallSection.doorwayOpen.SetActive(false);
                    Debug.Log($"Setting doorway inactive for {door.gameObject.name}");
                }
            }
        }

        // Then check for connections
        foreach (DoorPoint door in allDoors)
        {
            Collider[] nearby = Physics.OverlapSphere(door.transform.position, 1f);
            foreach (Collider other in nearby)
            {
                DoorPoint otherDoor = other.GetComponent<DoorPoint>();
                if (otherDoor != null && otherDoor != door)
                {
                    Debug.Log($"Processing connection: {door.gameObject.name} and {otherDoor.gameObject.name}");

                    RoomBehaviour room1 = door.GetComponentInParent<RoomBehaviour>();
                    RoomBehaviour room2 = otherDoor.GetComponentInParent<RoomBehaviour>();

                    int index1 = GetDoorIndex(door.gameObject.name);
                    int index2 = GetDoorIndex(otherDoor.gameObject.name);

                    // Explicitly toggle both sides
                    if (room1 != null && index1 >= 0)
                    {
                        var wallSection = room1.wallSections[index1];
                        if (wallSection.completeWall != null)
                        {
                            wallSection.completeWall.SetActive(false);
                            Debug.Log($"Deactivating wall for {door.gameObject.name}");
                        }
                        if (wallSection.doorwayOpen != null)
                        {
                            wallSection.doorwayOpen.SetActive(true);
                            Debug.Log($"Activating doorway for {door.gameObject.name}");
                        }
                    }

                    if (room2 != null && index2 >= 0)
                    {
                        var wallSection = room2.wallSections[index2];
                        if (wallSection.completeWall != null)
                        {
                            wallSection.completeWall.SetActive(false);
                            Debug.Log($"Deactivating wall for {otherDoor.gameObject.name}");
                        }
                        if (wallSection.doorwayOpen != null)
                        {
                            wallSection.doorwayOpen.SetActive(true);
                            Debug.Log($"Activating doorway for {otherDoor.gameObject.name}");
                        }
                    }
                }
            }
        }
    }

    private bool HasValidStairConnection(Vector2Int pos)
    {
        // Check North connection
        if (occupiedTiles.Contains(pos + Vector2Int.up))
        {
            Debug.Log($"Valid north connection found at {pos}");
            return true;
        }

        // Check East connection
        if (occupiedTiles.Contains(pos + Vector2Int.right))
        {
            Debug.Log($"Valid east connection found at {pos}");
            return true;
        }

        // Check West connection
        if (occupiedTiles.Contains(pos + Vector2Int.left))
        {
            Debug.Log($"Valid west connection found at {pos}");
            return true;
        }

        Debug.Log($"No valid connections found at position {pos}");
        return false;
    }

    void ProcessDoorConnections()
    {
        DoorPoint[] allDoors = FindObjectsByType<DoorPoint>(FindObjectsSortMode.None);
        Debug.Log($"Processing {allDoors.Length} door points");

        foreach (var door1 in allDoors)
        {
            foreach (var door2 in allDoors)
            {
                if (door1 != door2 && Vector3.Distance(door1.transform.position, door2.transform.position) < 2f)
                {
                    RoomBehaviour room1 = door1.GetComponentInParent<RoomBehaviour>();
                    RoomBehaviour room2 = door2.GetComponentInParent<RoomBehaviour>();

                    if (room1 != null && room2 != null)
                    {
                        int index1 = GetDoorIndex(door1.gameObject.name);
                        int index2 = GetDoorIndex(door2.gameObject.name);

                        Debug.Log($"Found connection between {door1.name} and {door2.name}");

                        // Force both sides to show doorways
                        if (index1 >= 0 && index2 >= 0)
                        {
                            room1.wallSections[index1].completeWall.SetActive(false);
                            room1.wallSections[index1].doorwayOpen.SetActive(true);

                            room2.wallSections[index2].completeWall.SetActive(false);
                            room2.wallSections[index2].doorwayOpen.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    private List<Vector2Int> GetValidPositionsForStairs(int minDistance)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();
        HashSet<Vector2Int> checkedPositions = new HashSet<Vector2Int>();

        foreach (Vector2Int occupied in occupiedTiles)
        {
            CheckAdjacentPositionForStairs(occupied + Vector2Int.up, validPositions, checkedPositions, minDistance);
            CheckAdjacentPositionForStairs(occupied + Vector2Int.down, validPositions, checkedPositions, minDistance);
            CheckAdjacentPositionForStairs(occupied + Vector2Int.left, validPositions, checkedPositions, minDistance);
            CheckAdjacentPositionForStairs(occupied + Vector2Int.right, validPositions, checkedPositions, minDistance);
        }

        // Filter for valid connections
        validPositions.RemoveAll(pos =>
            !occupiedTiles.Contains(pos + Vector2Int.up) &&
            !occupiedTiles.Contains(pos + Vector2Int.right) &&
            !occupiedTiles.Contains(pos + Vector2Int.left));

        return validPositions;
    }

    private void CheckAdjacentPositionForStairs(Vector2Int pos, List<Vector2Int> validPositions, HashSet<Vector2Int> checkedPositions, int minDistance)
    {
        if (checkedPositions.Contains(pos)) return;
        checkedPositions.Add(pos);

        if (CanPlaceRoomAt(pos, RoomSize.Normal) && HasAdjacentRoom(pos))
        {
            int distance = Mathf.Abs(pos.x - startRoomPosition.x) + Mathf.Abs(pos.y - startRoomPosition.y);

            if (distance >= minDistance)
            {
                validPositions.Add(pos);
                Debug.Log($"Added position {pos} as valid stair position (distance: {distance})");
            }
        }
    }

    private void CheckAdjacentPositionForStairs(Vector2Int pos, List<Vector2Int> validPositions, HashSet<Vector2Int> checkedPositions)
    {
        if (checkedPositions.Contains(pos)) return;
        checkedPositions.Add(pos);

        if (CanPlaceRoomAt(pos, RoomSize.Normal) && HasAdjacentRoom(pos))
        {
            int distance = Mathf.Abs(pos.x - startRoomPosition.x) + Mathf.Abs(pos.y - startRoomPosition.y);
            Debug.Log($"Checking position {pos} - Distance from start: {distance}, Minimum required: {minimumDistanceFromStart}");

            if (distance >= minimumDistanceFromStart)
            {
                validPositions.Add(pos);
                Debug.Log($"Added position {pos} as valid stair position");
            }
        }
    }

    void ResetAllDoors()
    {
        var allRooms = FindObjectsByType<RoomBehaviour>(FindObjectsSortMode.None);
        foreach (var room in allRooms)
        {
            for (int i = 0; i < room.wallSections.Length; i++)
            {
                if (room.wallSections[i].completeWall != null)
                {
                    room.wallSections[i].completeWall.SetActive(true);
                }
                if (room.wallSections[i].doorwayOpen != null)
                {
                    room.wallSections[i].doorwayOpen.SetActive(false);
                }
            }
        }
    }
}