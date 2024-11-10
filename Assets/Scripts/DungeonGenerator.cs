using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Room Prefabs")]
    public GameObject basicRoomPrefab;
    public GameObject lShapeRoomPrefab;
    public GameObject lShapeFlippedPrefab;
    public GameObject stairRoomPrefab;

    [Header("Generation Settings")]
    public bool generateOnStart = true;
    public int minRooms = 5;
    public int maxRooms = 8;
    public float roomSpacing = 18f;  // Adjusted for 1.5 scale (12 * 1.5)
    public Vector3 roomScale = new Vector3(1.5f, 1.5f, 1.5f);
    public KeyCode generateKey = KeyCode.G;

    private class RoomData
    {
        public GameObject instance;
        public Vector2Int gridPosition;
        public List<DoorPoint> doors = new List<DoorPoint>();
        public bool isStairRoom;

        public RoomData(GameObject roomObj, Vector2Int pos, bool isStair = false)
        {
            instance = roomObj;
            gridPosition = pos;
            isStairRoom = isStair;

            // Find all door points in the room
            doors.AddRange(roomObj.GetComponentsInChildren<DoorPoint>());
        }
    }

    private List<RoomData> placedRooms = new List<RoomData>();
    private HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();

    private void Start()
    {
        if (generateOnStart)
        {
            GenerateDungeon();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(generateKey))
        {
            ClearExistingDungeon();
            GenerateDungeon();
        }
    }

    private void ValidateSetup()
    {
        if (basicRoomPrefab == null)
            Debug.LogError("Basic Room Prefab is not assigned!");
        if (lShapeRoomPrefab == null)
            Debug.LogError("L-Shape Room Prefab is not assigned!");
        if (lShapeFlippedPrefab == null)
            Debug.LogError("L-Shape Flipped Prefab is not assigned!");
        if (stairRoomPrefab == null)
            Debug.LogError("Stair Room Prefab is not assigned!");
    }

    public void GenerateDungeon()
    {
        Debug.Log("Starting dungeon generation...");
        ValidateSetup();

        ClearExistingDungeon();
        PlaceFirstRoom();

        int roomsToPlace = Random.Range(minRooms, maxRooms + 1);
        int attempts = 0;
        int maxAttempts = 100;

        while (placedRooms.Count < roomsToPlace && attempts < maxAttempts)
        {
            if (TryPlaceNextRoom())
            {
                attempts = 0;
                Debug.Log($"Placed room {placedRooms.Count} of {roomsToPlace}");
            }
            else
            {
                attempts++;
                Debug.Log($"Failed attempt {attempts} of {maxAttempts}");
            }
        }

        PlaceStairRoom();
        SealUnusedDoors();
        Debug.Log("Dungeon generation complete!");
    }

    private void PlaceFirstRoom()
    {
        Debug.Log("Placing first room...");
        Vector2Int startPos = Vector2Int.zero;
        GameObject roomInstance = Instantiate(basicRoomPrefab, Vector3ToWorld(startPos), Quaternion.identity, transform);
        roomInstance.transform.localScale = roomScale;

        RoomData newRoom = new RoomData(roomInstance, startPos);
        placedRooms.Add(newRoom);
        occupiedPositions.Add(startPos);
        Debug.Log("First room placed successfully");
    }

    private bool TryPlaceNextRoom()
    {
        if (placedRooms.Count == 0)
        {
            Debug.LogError("No rooms placed yet!");
            return false;
        }

        // Get a random placed room
        RoomData sourceRoom = placedRooms[Random.Range(0, placedRooms.Count)];

        // Get unconnected doors from this room
        List<DoorPoint> availableDoors = new List<DoorPoint>();
        foreach (DoorPoint door in sourceRoom.doors)
        {
            if (!door.isConnected)
                availableDoors.Add(door);
        }

        if (availableDoors.Count == 0) return false;

        // Try each available door
        foreach (DoorPoint door in availableDoors)
        {
            Vector2Int newPos = GetPositionFromDoor(sourceRoom.gridPosition, door.direction);

            if (!occupiedPositions.Contains(newPos))
            {
                if (TryPlaceConnectingRoom(newPos, door))
                {
                    door.isConnected = true;
                    return true;
                }
            }
        }

        return false;
    }

    private bool TryPlaceConnectingRoom(Vector2Int position, DoorPoint connectingDoor)
    {
        GameObject[] roomPrefabs = { basicRoomPrefab, lShapeRoomPrefab, lShapeFlippedPrefab };
        List<GameObject> shuffledPrefabs = new List<GameObject>(roomPrefabs);

        // Try each prefab
        foreach (GameObject prefab in shuffledPrefabs)
        {
            GameObject tempRoom = Instantiate(prefab, Vector3ToWorld(position), Quaternion.identity);
            tempRoom.transform.localScale = roomScale;
            DoorPoint[] tempDoors = tempRoom.GetComponentsInChildren<DoorPoint>();

            // Check if this room has a matching door
            DoorDirection neededDirection = GetOppositeDirection(connectingDoor.direction);
            bool hasMatchingDoor = false;

            foreach (DoorPoint tempDoor in tempDoors)
            {
                if (tempDoor.direction == neededDirection)
                {
                    hasMatchingDoor = true;
                    break;
                }
            }

            if (hasMatchingDoor)
            {
                RoomData newRoom = new RoomData(tempRoom, position);
                placedRooms.Add(newRoom);
                occupiedPositions.Add(position);

                foreach (DoorPoint door in newRoom.doors)
                {
                    if (door.direction == neededDirection)
                    {
                        door.isConnected = true;
                        break;
                    }
                }

                return true;
            }

            Destroy(tempRoom);
        }

        return false;
    }

    private void PlaceStairRoom()
    {
        Debug.Log("Attempting to place stair room...");
        foreach (RoomData room in placedRooms)
        {
            foreach (DoorPoint door in room.doors)
            {
                if (!door.isConnected)
                {
                    Vector2Int stairPos = GetPositionFromDoor(room.gridPosition, door.direction);
                    if (!occupiedPositions.Contains(stairPos))
                    {
                        GameObject stairInstance = Instantiate(stairRoomPrefab, Vector3ToWorld(stairPos), Quaternion.identity, transform);
                        stairInstance.transform.localScale = roomScale;
                        RoomData stairRoom = new RoomData(stairInstance, stairPos, true);

                        foreach (DoorPoint stairDoor in stairRoom.doors)
                        {
                            if (stairDoor.direction == GetOppositeDirection(door.direction))
                            {
                                stairDoor.isConnected = true;
                                door.isConnected = true;
                                break;
                            }
                        }

                        placedRooms.Add(stairRoom);
                        Debug.Log("Stair room placed successfully");
                        return;
                    }
                }
            }
        }
        Debug.LogWarning("Could not place stair room!");
    }

    private void SealUnusedDoors()
    {
        foreach (RoomData room in placedRooms)
        {
            foreach (DoorPoint door in room.doors)
            {
                if (!door.isConnected)
                {
                    door.SealDoorway();
                }
            }
        }
    }

    private Vector2Int GetPositionFromDoor(Vector2Int roomPos, DoorDirection direction)
    {
        switch (direction)
        {
            case DoorDirection.North: return roomPos + Vector2Int.up;
            case DoorDirection.South: return roomPos + Vector2Int.down;
            case DoorDirection.East: return roomPos + Vector2Int.right;
            case DoorDirection.West: return roomPos + Vector2Int.left;
            default: return roomPos;
        }
    }

    private DoorDirection GetOppositeDirection(DoorDirection direction)
    {
        switch (direction)
        {
            case DoorDirection.North: return DoorDirection.South;
            case DoorDirection.South: return DoorDirection.North;
            case DoorDirection.East: return DoorDirection.West;
            case DoorDirection.West: return DoorDirection.East;
            default: return direction;
        }
    }

    private Vector3 Vector3ToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * roomSpacing, 0, gridPos.y * roomSpacing);
    }

    private void ClearExistingDungeon()
    {
        foreach (RoomData room in placedRooms)
        {
            if (room.instance != null)
            {
                Destroy(room.instance);
            }
        }
        placedRooms.Clear();
        occupiedPositions.Clear();
    }
}