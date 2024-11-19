using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Room Prefabs")]
    public GameObject startingRoomPrefab;
    public GameObject[] normalRoomPrefabs;  // 1x1 rooms
    public GameObject[] largeRoomPrefabs;   // 2x2 rooms
    public GameObject[] longRoomPrefabs;    // 1x2 rooms

    [Header("Generation Settings")]
    public int numberOfNormalRooms = 5;
    public int numberOfLargeRooms = 1;
    public int numberOfLongRooms = 2;
    public float roomSpacing = 18f;

    private HashSet<Vector2Int> occupiedTiles = new HashSet<Vector2Int>();
    private List<GameObject> placedRooms = new List<GameObject>();

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        // Place starting room
        PlaceRoom(startingRoomPrefab, Vector2Int.zero, RoomSize.Normal);

        // Place large rooms first (since they need more space)
        for (int i = 0; i < numberOfLargeRooms; i++)
        {
            TryPlaceLargeRoom();
        }

        // Place long rooms
        for (int i = 0; i < numberOfLongRooms; i++)
        {
            TryPlaceLongRoom();
        }

        // Fill in with normal rooms
        for (int i = 0; i < numberOfNormalRooms; i++)
        {
            TryPlaceNormalRoom();
        }
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

    private void PlaceRoom(GameObject prefab, Vector2Int gridPos, RoomSize size)
    {
        Vector3 worldPos = new Vector3(gridPos.x * roomSpacing, 0, gridPos.y * roomSpacing);
        GameObject room = Instantiate(prefab, worldPos, Quaternion.identity);
        room.transform.SetParent(transform);
        placedRooms.Add(room);
        OccupyTiles(gridPos, size);
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Draw grid
        Gizmos.color = Color.gray;
        float gridSize = roomSpacing;
        int range = 10;

        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector3 pos = new Vector3(x * gridSize, 0, y * gridSize);
                if (occupiedTiles.Contains(new Vector2Int(x, y)))
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(pos, new Vector3(gridSize - 0.1f, 0.1f, gridSize - 0.1f));
                    Gizmos.color = Color.gray;
                }
                else
                {
                    Gizmos.DrawWireCube(pos, new Vector3(gridSize - 0.1f, 0.1f, gridSize - 0.1f));
                }
            }
        }
    }
}