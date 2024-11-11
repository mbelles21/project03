using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Room Prefabs")]
    public GameObject basicRoomPrefab;
    public GameObject lShapeRoomPrefab;
    public GameObject lShapeFlippedPrefab;
    public GameObject stairRoomPrefab;
    public GameObject corridorPrefab;  // For connecting rooms

    [Header("Generation Settings")]
    public bool generateOnStart = true;
    public int minRooms = 5;
    public int maxRooms = 8;
    public float roomSpacing = 18f;
    public Vector3 floorHeight = new Vector3(0, -5f, 0);
    public int totalFloors = 3;
    public KeyCode generateKey = KeyCode.G;

    private List<List<RoomData>> floors = new List<List<RoomData>>();  // Rooms organized by floor
    private HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int>();
    private Dictionary<Vector3Int, RoomData> roomGrid = new Dictionary<Vector3Int, RoomData>();

    private class RoomData
    {
        public GameObject instance;
        public Vector3Int position;
        public List<DoorPoint> doors = new List<DoorPoint>();
        public bool isStairRoom;
        public HashSet<RoomData> connectedRooms = new HashSet<RoomData>();

        public RoomData(GameObject roomObj, Vector3Int pos, bool isStair = false)
        {
            instance = roomObj;
            position = pos;
            isStairRoom = isStair;
            doors.AddRange(roomObj.GetComponentsInChildren<DoorPoint>());
        }

        public bool IsConnectedTo(RoomData other)
        {
            return connectedRooms.Contains(other);
        }
    }

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

    public void GenerateDungeon()
    {
        Debug.Log("Starting dungeon generation...");

        // Initialize floors
        for (int i = 0; i < totalFloors; i++)
        {
            floors.Add(new List<RoomData>());
        }

        // Generate each floor
        for (int floor = 0; floor < totalFloors; floor++)
        {
            GenerateFloor(floor);
        }

        // Connect floors with stairs
        ConnectFloors();

        // Final pass to ensure all doors are either connected or sealed
        SealUnusedDoors();

        Debug.Log("Dungeon generation complete!");
    }

    private void GenerateFloor(int floorNumber)
    {
        int roomsToPlace = Random.Range(minRooms, maxRooms + 1);
        List<RoomData> floorRooms = floors[floorNumber];

        // Place first room at center
        PlaceRoom(Vector3Int.zero + Vector3Int.up * floorNumber);

        // Place remaining rooms
        for (int i = 1; i < roomsToPlace; i++)
        {
            TryPlaceConnectedRoom(floorNumber);
        }

        // Ensure rooms are connected
        ConnectRoomsOnFloor(floorNumber);
    }

    private void TryPlaceConnectedRoom(int floor)
    {
        // Get a random existing room on this floor
        List<RoomData> floorRooms = floors[floor];
        if (floorRooms.Count == 0) return;

        RoomData sourceRoom = floorRooms[Random.Range(0, floorRooms.Count)];

        // Try to place room connected to an available door
        foreach (DoorPoint door in sourceRoom.doors.Where(d => !d.isConnected))
        {
            Vector3Int neighborPos = GetNeighborPosition(sourceRoom.position, door.direction);

            if (!occupiedPositions.Contains(neighborPos))
            {
                // Try to place a room that can connect to this door
                if (TryPlaceConnectingRoom(neighborPos, door))
                {
                    return;
                }
            }
        }
    }

    private bool TryPlaceConnectingRoom(Vector3Int position, DoorPoint connectingDoor)
    {
        // Try different room prefabs
        GameObject[] prefabs = { basicRoomPrefab, lShapeRoomPrefab, lShapeFlippedPrefab };

        foreach (GameObject prefab in prefabs)
        {
            // Try different rotations
            for (int rotation = 0; rotation < 4; rotation++)
            {
                Quaternion rot = Quaternion.Euler(0, rotation * 90, 0);
                GameObject roomInstance = Instantiate(prefab, GetWorldPosition(position), rot, transform);

                RoomData newRoom = new RoomData(roomInstance, position);

                // Check if any door aligns with the connecting door
                if (HasCompatibleDoor(newRoom, connectingDoor))
                {
                    // Place room
                    floors[position.y].Add(newRoom);
                    occupiedPositions.Add(position);
                    roomGrid[position] = newRoom;
                    return true;
                }

                Destroy(roomInstance);
            }
        }

        return false;
    }

    private bool HasCompatibleDoor(RoomData room, DoorPoint targetDoor)
    {
        DoorDirection neededDirection = GetOppositeDirection(targetDoor.direction);
        return room.doors.Any(d => d.direction == neededDirection && !d.isConnected);
    }

    private void ConnectRoomsOnFloor(int floor)
    {
        List<RoomData> floorRooms = floors[floor];

        foreach (RoomData room in floorRooms)
        {
            foreach (DoorPoint door in room.doors.Where(d => !d.isConnected))
            {
                Vector3Int neighborPos = GetNeighborPosition(room.position, door.direction);

                if (roomGrid.TryGetValue(neighborPos, out RoomData neighbor))
                {
                    // Find matching door on neighbor
                    DoorPoint matchingDoor = neighbor.doors.FirstOrDefault(d =>
                        d.direction == GetOppositeDirection(door.direction) && !d.isConnected);

                    if (matchingDoor != null)
                    {
                        ConnectDoors(door, matchingDoor);
                        room.connectedRooms.Add(neighbor);
                        neighbor.connectedRooms.Add(room);
                    }
                    else if (!door.isConnected && !door.optional)
                    {
                        // Place corridor if needed
                        PlaceCorridor(room.position, neighborPos);
                    }
                }
            }
        }
    }

    private void ConnectFloors()
    {
        for (int floor = 0; floor < totalFloors - 1; floor++)
        {
            // Place stair room
            Vector3Int stairPos = GetValidStairPosition(floor);
            GameObject stairInstance = Instantiate(stairRoomPrefab, GetWorldPosition(stairPos), Quaternion.identity, transform);
            RoomData stairRoom = new RoomData(stairInstance, stairPos, true);

            floors[floor].Add(stairRoom);
            occupiedPositions.Add(stairPos);
            roomGrid[stairPos] = stairRoom;

            // Connect to rooms on both floors
            ConnectStairRoom(stairRoom, floor);
        }
    }
    private void ConnectDoors(DoorPoint door1, DoorPoint door2)
    {
        // Mark both doors as connected
        door1.isConnected = true;
        door2.isConnected = true;

        // Open the doorways
        door1.OpenDoorway();
        door2.OpenDoorway();

        // You could add additional connection logic here
        // For example:
        // - Create a physical connection between rooms
        // - Add triggers for room transitions
        // - Update navigation meshes
    }
    private bool CanConnectDoors(DoorPoint door1, DoorPoint door2)
    {
        // Check if doors are already connected
        if (door1.isConnected || door2.isConnected)
            return false;

        // Check if doors face each other
        if (door1.direction != GetOppositeDirection(door2.direction))
            return false;

        // Additional validation could be added here
        return true;
    }
    private Vector3Int GetValidStairPosition(int floor)
    {
        List<Vector3Int> validPositions = new List<Vector3Int>();

        // Find positions next to existing rooms
        foreach (RoomData room in floors[floor])
        {
            foreach (DoorPoint door in room.doors.Where(d => !d.isConnected))
            {
                Vector3Int neighborPos = GetNeighborPosition(room.position, door.direction);
                if (!occupiedPositions.Contains(neighborPos))
                {
                    validPositions.Add(neighborPos);
                }
            }
        }

        return validPositions[Random.Range(0, validPositions.Count)];
    }

    private void ConnectStairRoom(RoomData stairRoom, int floor)
    {
        // Connect to current floor
        ConnectRoomsOnFloor(floor);

        // Connect to floor above (if exists)
        if (floor < totalFloors - 1)
        {
            Vector3Int upperPos = stairRoom.position + Vector3Int.up;
            if (!occupiedPositions.Contains(upperPos))
            {
                // Place connection point or special room on upper floor
                PlaceRoom(upperPos);
            }
        }
    }

    private void PlaceCorridor(Vector3Int from, Vector3Int to)
    {
        Vector3 corridorPos = GetWorldPosition((from + to) / 2);
        Vector3 direction = GetWorldPosition(to) - GetWorldPosition(from);

        GameObject corridor = Instantiate(corridorPrefab, corridorPos, Quaternion.LookRotation(direction), transform);
        // Scale corridor to fit between rooms if needed
        corridor.transform.localScale = new Vector3(1, 1, direction.magnitude / roomSpacing);
    }

    private Vector3 GetWorldPosition(Vector3Int gridPos)
    {
        return new Vector3(
            gridPos.x * roomSpacing,
            gridPos.y * floorHeight.y,
            gridPos.z * roomSpacing
        );
    }

    private Vector3Int GetNeighborPosition(Vector3Int pos, DoorDirection dir)
    {
        switch (dir)
        {
            case DoorDirection.North: return pos + Vector3Int.forward;
            case DoorDirection.South: return pos + Vector3Int.back;
            case DoorDirection.East: return pos + Vector3Int.right;
            case DoorDirection.West: return pos + Vector3Int.left;
            default: return pos;
        }
    }

    private DoorDirection GetOppositeDirection(DoorDirection dir)
    {
        switch (dir)
        {
            case DoorDirection.North: return DoorDirection.South;
            case DoorDirection.South: return DoorDirection.North;
            case DoorDirection.East: return DoorDirection.West;
            case DoorDirection.West: return DoorDirection.East;
            default: return dir;
        }
    }

    private void PlaceRoom(Vector3Int position)
    {
        GameObject prefab = basicRoomPrefab; // Could randomly select from available prefabs
        GameObject instance = Instantiate(prefab, GetWorldPosition(position), Quaternion.identity, transform);
        RoomData room = new RoomData(instance, position);

        floors[position.y].Add(room);
        occupiedPositions.Add(position);
        roomGrid[position] = room;
    }

    private void SealUnusedDoors()
    {
        foreach (var floorRooms in floors)
        {
            foreach (RoomData room in floorRooms)
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
    }

    private void ClearExistingDungeon()
    {
        foreach (var floorRooms in floors)
        {
            foreach (RoomData room in floorRooms)
            {
                if (room.instance != null)
                {
                    Destroy(room.instance);
                }
            }
        }

        floors.Clear();
        occupiedPositions.Clear();
        roomGrid.Clear();
    }
}