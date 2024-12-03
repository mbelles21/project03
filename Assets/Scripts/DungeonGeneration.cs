using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
<<<<<<< Updated upstream
=======
    public GameObject player;

>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
=======
    private EnemySpawner enemySpawner;

    public bool IsGenerationComplete { get; private set; } = false;
    public bool IsFinalFloor { get; private set; } = false;

>>>>>>> Stashed changes

    void Start()
    {
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
    private bool ValidateRoomCounts()
    {
        int normalRoomCount = 0;
        int largeRoomCount = 0;

        foreach (GameObject room in generatedRooms)
        {
            // Check if the room's name matches any of the prefab names
            string roomName = room.name.Replace("(Clone)", "");

            bool isNormalRoom = false;
            bool isLargeRoom = false;

            // Check normal room prefabs
            foreach (GameObject prefab in normalRoomPrefabs)
            {
                if (roomName == prefab.name)
                {
                    isNormalRoom = true;
                    break;
                }
            }

            // Check large room prefabs
            foreach (GameObject prefab in largeRoomPrefabs)
            {
                if (roomName == prefab.name)
                {
                    isLargeRoom = true;
                    break;
                }
            }

            if (isNormalRoom) normalRoomCount++;
            if (isLargeRoom) largeRoomCount++;
        }

        if (useRandomCounts)
        {
            return normalRoomCount >= normalRoomRange.x &&
                   largeRoomCount >= largeRoomRange.x;
        }
        else
        {
            return normalRoomCount >= numberOfNormalRooms &&
                   largeRoomCount >= numberOfLargeRooms;
        }
    }

    IEnumerator GenerateDungeonSequence()
    {
        IsGenerationComplete = false;
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
            PlaceRoom(startingRoomPrefab, Vector2Int.zero, RoomSize.Normal);
            startRoomPosition = Vector2Int.zero;
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

            if (stairPlaced && ValidateRoomCounts())
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
                Debug.Log("Failed to meet room requirements or place stair room, retrying generation...");
                yield return new WaitForSeconds(0.1f);
                continue;
            }
        }

        if (!successfulGeneration)
        {
            Debug.LogError($"Failed to generate valid dungeon after {maxAttempts} attempts!");
            IsGenerationComplete = true; // Set to true even on failure so we don't hang
            yield break;
        }

        yield return new WaitForSeconds(0.2f);

        Debug.Log("Resetting all doors...");
        ResetAllDoors();

        DungeonConnectionHandler.Instance.ProcessConnections();

        IsGenerationComplete = true; // Set to true when generation is successful
    }


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
        Debug.Log($"OccupyTiles: Starting occupation at position ({position.x}, {position.y}) for size {size}");
        int beforeCount = occupiedTiles.Count;

        switch (size)
        {
            case RoomSize.Normal:
                occupiedTiles.Add(position);
                Debug.Log($"OccupyTiles: Added normal room tile at ({position.x}, {position.y})");
                break;

            case RoomSize.Large:
                for (int x = 0; x < 2; x++)
                    for (int y = 0; y < 2; y++)
                    {
                        Vector2Int pos = position + new Vector2Int(x, y);
                        occupiedTiles.Add(pos);
                        Debug.Log($"OccupyTiles: Added large room tile at ({pos.x}, {pos.y})");
                    }
                break;

            case RoomSize.Long:
                occupiedTiles.Add(position);
                occupiedTiles.Add(position + Vector2Int.up);
                Debug.Log($"OccupyTiles: Added long room tiles at ({position.x}, {position.y}) and ({position.x}, {position.y + 1})");
                break;
        }

        Debug.Log($"OccupyTiles: Tiles before: {beforeCount}, after: {occupiedTiles.Count}");
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

    private bool TryPlaceNormalRoom()
    {
        if (normalRoomPrefabs == null || normalRoomPrefabs.Length == 0) return false;

        int maxAttempts = 10;
        while (maxAttempts > 0)
        {
            var validPositions = GetValidPositionsForSize(RoomSize.Normal);
            if (validPositions.Count > 0)
            {
                Vector2Int pos = validPositions[Random.Range(0, validPositions.Count)];
                GameObject prefab = normalRoomPrefabs[Random.Range(0, normalRoomPrefabs.Length)];
                PlaceRoom(prefab, pos, RoomSize.Normal);
                return true;
            }
            maxAttempts--;
        }
        return false;
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
            PlaceRoom(stairRoomPrefab, pos, RoomSize.Normal);
            return true;
        }
        return false;
    }

    private void PlaceRoom(GameObject prefab, Vector2Int gridPos, RoomSize size)
    {
        Debug.Log($"PlaceRoom: Placing {prefab.name} at position ({gridPos.x}, {gridPos.y})");

        float heightOffset = 1f;
        Vector3 worldPos = new Vector3(gridPos.x * roomSpacing, heightOffset, gridPos.y * roomSpacing);
        GameObject room = Instantiate(prefab, worldPos, Quaternion.identity);
        room.transform.SetParent(transform);
        generatedRooms.Add(room);

        Debug.Log($"PlaceRoom: Before OccupyTiles, current occupied count: {occupiedTiles.Count}");
        OccupyTiles(gridPos, size);
        Debug.Log($"PlaceRoom: After OccupyTiles, new occupied count: {occupiedTiles.Count}");

        RoomBehaviour roomBehaviour = room.GetComponent<RoomBehaviour>();
        if (roomBehaviour != null)
        {
            for (int i = 0; i < roomBehaviour.wallSections.Length; i++)
            {
                roomBehaviour.ShowWall(i);
            }
        }
<<<<<<< Updated upstream
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
=======

        // Enemy spawning logic
        if (enemySpawner != null && enemiesAllowed)
        {
            enemySpawner.SpawnEnemy(worldPos, (int)size);
>>>>>>> Stashed changes
        }
    }
    private List<Vector2Int> GetValidPositionsForStairs(int minDistance)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();
        HashSet<Vector2Int> checkedPositions = new HashSet<Vector2Int>();

        foreach (Vector2Int occupied in occupiedTiles)
        {
            // Only check North, East, and West positions since stairs block South
            CheckAdjacentPositionForStairs(occupied + Vector2Int.up, validPositions, checkedPositions, minDistance);    // North
            CheckAdjacentPositionForStairs(occupied + Vector2Int.right, validPositions, checkedPositions, minDistance); // East
            CheckAdjacentPositionForStairs(occupied + Vector2Int.left, validPositions, checkedPositions, minDistance);  // West
        }

        // Filter positions - must have at least one valid connection on North, East, or West
        validPositions.RemoveAll(pos =>
            !(occupiedTiles.Contains(pos + Vector2Int.up) ||    // North connection
              occupiedTiles.Contains(pos + Vector2Int.right) ||  // East connection
              occupiedTiles.Contains(pos + Vector2Int.left)));   // West connection

        return validPositions;
    }
    private void CheckAdjacentPositionForStairs(Vector2Int pos, List<Vector2Int> validPositions, HashSet<Vector2Int> checkedPositions, int minDistance)
    {
        if (checkedPositions.Contains(pos)) return;
        checkedPositions.Add(pos);

        if (CanPlaceRoomAt(pos, RoomSize.Normal))
        {
            // Check if position has at least one valid connection on North, East, or West
            bool hasValidConnection =
                occupiedTiles.Contains(pos + Vector2Int.up) ||
                occupiedTiles.Contains(pos + Vector2Int.right) ||
                occupiedTiles.Contains(pos + Vector2Int.left);

            if (hasValidConnection)
            {
                int distance = Mathf.Abs(pos.x - startRoomPosition.x) + Mathf.Abs(pos.y - startRoomPosition.y);
                if (distance >= minDistance)
                {
                    validPositions.Add(pos);
                    Debug.Log($"Added position {pos} as valid stair position (distance: {distance})");
                }
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