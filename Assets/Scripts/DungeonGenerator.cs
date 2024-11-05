using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Room Prefabs")]
    public GameObject basicRoomPrefab;      // Room_Template_1 (3x3 with 4 doors)
    public GameObject lShapeRoomPrefab;     // Room_Template_2 (L-shape)
    public GameObject lShapeFlippedPrefab;  // Room_Template_3 (Flipped L-shape)
    public GameObject stairRoomPrefab;      // Stair_Room

    [Header("Generation Settings")]
    public int minRooms = 5;
    public int maxRooms = 8;
    public float roomSpacing = 12f;  // Adjust based on your room size (3x3 * 4 units)
    public KeyCode generateKey = KeyCode.G;

    private class Room
    {
        public GameObject instance;
        public Vector2Int gridPosition;
        public List<Vector2Int> doorPositions;
        public bool isStairRoom;

        public Room(GameObject roomObj, Vector2Int pos, List<Vector2Int> doors, bool isStair = false)
        {
            instance = roomObj;
            gridPosition = pos;
            doorPositions = doors;
            isStairRoom = isStair;
        }
    }

    private List<Room> placedRooms = new List<Room>();
    private HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> availableDoorPositions = new HashSet<Vector2Int>();

    private void Start()
    {
        ValidateSetup();
        GenerateDungeon();
    }

    private void Update()
    {
        if (Input.GetKeyDown(generateKey))
        {
            RegenerateDungeon();
        }
    }

    private void ValidateSetup()
    {
        if (basicRoomPrefab == null || lShapeRoomPrefab == null ||
            lShapeFlippedPrefab == null || stairRoomPrefab == null)
        {
            Debug.LogError("Please assign all room prefabs!");
        }
    }

    private void RegenerateDungeon()
    {
        // Clear existing dungeon
        foreach (Room room in placedRooms)
        {
            if (room.instance != null)
            {
                Destroy(room.instance);
            }
        }

        placedRooms.Clear();
        occupiedPositions.Clear();
        availableDoorPositions.Clear();

        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        // Start with a basic room at the center
        PlaceFirstRoom();

        // Add random rooms
        int roomsToPlace = Random.Range(minRooms, maxRooms + 1);
        int attempts = 0;
        int maxAttempts = 100;

        while (placedRooms.Count < roomsToPlace && attempts < maxAttempts)
        {
            if (TryPlaceNextRoom())
            {
                attempts = 0;
            }
            else
            {
                attempts++;
            }
        }

        // Place stair room last
        PlaceStairRoom();

        Debug.Log($"Generated dungeon with {placedRooms.Count} rooms");
    }

    private void PlaceFirstRoom()
    {
        Vector2Int startPos = Vector2Int.zero;
        GameObject roomInstance = Instantiate(basicRoomPrefab, Vector3ToWorld(startPos), Quaternion.identity, transform);

        List<Vector2Int> doors = new List<Vector2Int>
        {
            startPos + Vector2Int.up,
            startPos + Vector2Int.right,
            startPos + Vector2Int.down,
            startPos + Vector2Int.left
        };

        Room newRoom = new Room(roomInstance, startPos, doors);
        placedRooms.Add(newRoom);
        occupiedPositions.Add(startPos);

        foreach (Vector2Int door in doors)
        {
            availableDoorPositions.Add(door);
        }
    }

    private bool TryPlaceNextRoom()
    {
        if (availableDoorPositions.Count == 0) return false;

        // Get a random available door position
        Vector2Int[] availableDoors = new Vector2Int[availableDoorPositions.Count];
        availableDoorPositions.CopyTo(availableDoors);
        Vector2Int doorPos = availableDoors[Random.Range(0, availableDoors.Length)]; // Fixed this line

        // Try to place a room connected to this door
        GameObject[] roomPrefabs = { basicRoomPrefab, lShapeRoomPrefab, lShapeFlippedPrefab };
        GameObject selectedPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

        if (TryPlaceRoomAtPosition(doorPos, selectedPrefab))
        {
            availableDoorPositions.Remove(doorPos);
            return true;
        }

        return false;
    }

    private bool TryPlaceRoomAtPosition(Vector2Int position, GameObject roomPrefab)
    {
        // Check if position is already occupied
        if (occupiedPositions.Contains(position)) return false;

        // Place room
        GameObject roomInstance = Instantiate(roomPrefab, Vector3ToWorld(position), Quaternion.identity, transform);

        // Get door positions based on room type
        List<Vector2Int> doors = GetDoorPositionsForRoom(position, roomPrefab);

        // Create new room
        Room newRoom = new Room(roomInstance, position, doors);
        placedRooms.Add(newRoom);
        occupiedPositions.Add(position);

        // Add new door positions
        foreach (Vector2Int door in doors)
        {
            if (!occupiedPositions.Contains(door))
            {
                availableDoorPositions.Add(door);
            }
        }

        return true;
    }

    private void PlaceStairRoom()
    {
        foreach (Vector2Int doorPos in availableDoorPositions)
        {
            if (!occupiedPositions.Contains(doorPos))
            {
                GameObject stairInstance = Instantiate(stairRoomPrefab, Vector3ToWorld(doorPos), Quaternion.identity, transform);
                List<Vector2Int> stairDoors = GetDoorPositionsForRoom(doorPos, stairRoomPrefab);
                Room stairRoom = new Room(stairInstance, doorPos, stairDoors, true);
                placedRooms.Add(stairRoom);
                return;
            }
        }
    }

    private List<Vector2Int> GetDoorPositionsForRoom(Vector2Int roomPos, GameObject roomPrefab)
    {
        List<Vector2Int> doors = new List<Vector2Int>();

        if (roomPrefab == basicRoomPrefab)
        {
            // Add all four doors for basic room
            doors.Add(roomPos + Vector2Int.up);
            doors.Add(roomPos + Vector2Int.right);
            doors.Add(roomPos + Vector2Int.down);
            doors.Add(roomPos + Vector2Int.left);
        }
        else if (roomPrefab == lShapeRoomPrefab)
        {
            // Add doors for L-shape room
            doors.Add(roomPos + Vector2Int.right);
            doors.Add(roomPos + Vector2Int.down);
        }
        else if (roomPrefab == lShapeFlippedPrefab)
        {
            // Add doors for flipped L-shape room
            doors.Add(roomPos + Vector2Int.left);
            doors.Add(roomPos + Vector2Int.down);
        }
        else if (roomPrefab == stairRoomPrefab)
        {
            // Add doors for stair room
            doors.Add(roomPos + Vector2Int.up);
            doors.Add(roomPos + Vector2Int.right);
            doors.Add(roomPos + Vector2Int.down);
        }

        return doors;
    }

    private Vector3 Vector3ToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * roomSpacing, 0, gridPos.y * roomSpacing);
    }
}
