using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
    public enum RoomType { Normal, Large, Long }
    public RoomType roomType = RoomType.Normal;

    [Header("Room Properties")]
    public Vector2Int gridSize = new Vector2Int(1, 1);
    public float roomHeight = 6f;
    public bool isStartingRoom = false;

    [Header("Door Points")]
    public Transform[] northDoors;
    public Transform[] southDoors;
    public Transform[] eastDoors;
    public Transform[] westDoors;

    void OnDrawGizmos()
    {
        // Draw room bounds
        Gizmos.color = isStartingRoom ? Color.green : Color.blue;
        Vector3 size = new Vector3(gridSize.x * 18f, roomHeight, gridSize.y * 18f);
        Vector3 center = transform.position + new Vector3(size.x / 2f, roomHeight / 2f, size.z / 2f);
        Gizmos.DrawWireCube(center, size);

        // Draw door positions
        Gizmos.color = Color.yellow;
        if (northDoors != null) foreach (var door in northDoors) DrawDoorGizmo(door);
        if (southDoors != null) foreach (var door in southDoors) DrawDoorGizmo(door);
        if (eastDoors != null) foreach (var door in eastDoors) DrawDoorGizmo(door);
        if (westDoors != null) foreach (var door in westDoors) DrawDoorGizmo(door);
    }

    private void DrawDoorGizmo(Transform door)
    {
        if (door != null)
        {
            Gizmos.DrawSphere(door.position, 0.5f);
            Gizmos.DrawRay(door.position, door.forward * 2f);
        }
    }
}