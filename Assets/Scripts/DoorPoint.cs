using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorDirection
{
    North,
    South,
    East,
    West
}

public class DoorPoint : MonoBehaviour
{
    public DoorDirection direction;
    public bool isConnected = false;
    public GameObject doorBlockerPrefab; // Assign a wall prefab to block unused doors
    private GameObject spawmedBlocker;

    public void SealDoorway()
    {
        if (doorBlockerPrefab != null && !isConnected)
        {
            // Spawn the blocker with the correct rotation based on direction
            Quaternion rotation = Quaternion.identity;
            switch (direction)
            {
                case DoorDirection.North:
                    rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case DoorDirection.South:
                    rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case DoorDirection.East:
                    rotation = Quaternion.Euler(0, 90, 0);
                    break;
                case DoorDirection.West:
                    rotation = Quaternion.Euler(0, 270, 0);
                    break;
            }

            spawmedBlocker = Instantiate(doorBlockerPrefab, transform.position, rotation, transform);
        }
    }

    public void UnSealDoorway()
    {
        if (spawmedBlocker != null)
        {
            Destroy(spawmedBlocker);
        }
    }
}