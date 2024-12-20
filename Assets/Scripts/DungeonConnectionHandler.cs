using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonConnectionHandler : MonoBehaviour
{
    private static DungeonConnectionHandler instance;
    public static DungeonConnectionHandler Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("DungeonConnectionHandler");
                instance = go.AddComponent<DungeonConnectionHandler>();
            }
            return instance;
        }
    }

    public bool IsProcessingConnections { get; private set; }

    public IEnumerator ProcessConnectionsCoroutine()
    {
        IsProcessingConnections = true;
        Debug.Log("Starting door connection process...");

        // Wait for physics to settle
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.2f);

        DoorPoint[] allDoors = FindObjectsByType<DoorPoint>(FindObjectsSortMode.None);
        Debug.Log($"Found {allDoors.Length} door points in scene");

        HashSet<DoorPoint> processedDoors = new HashSet<DoorPoint>();

        foreach (var door1 in allDoors)
        {
            if (processedDoors.Contains(door1)) continue;

            // Check if door1 is still valid
            if (door1 == null) continue;

            foreach (var door2 in allDoors)
            {
                if (door1 == door2 || processedDoors.Contains(door2)) continue;

                // Check if door2 is still valid
                if (door2 == null) continue;

                if (Vector3.Distance(door1.transform.position, door2.transform.position) < 2f)
                {
                    RoomBehaviour room1 = door1.GetComponentInParent<RoomBehaviour>();
                    RoomBehaviour room2 = door2.GetComponentInParent<RoomBehaviour>();

                    if (room1 != null && room2 != null)
                    {
                        float alignment = Vector3.Dot(door1.transform.forward, -door2.transform.forward);
                        if (alignment > 0.9f)
                        {
                            int index1 = GetDoorIndex(door1.gameObject.name);
                            int index2 = GetDoorIndex(door2.gameObject.name);

                            if (index1 >= 0 && index2 >= 0)
                            {
                                Debug.Log($"Found connection between {door1.name} and {door2.name}");

                                room1.ShowDoorway(index1);
                                room2.ShowDoorway(index2);

                                processedDoors.Add(door1);
                                processedDoors.Add(door2);

                                // Give physics a moment to update
                                yield return new WaitForFixedUpdate();
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Final wait to ensure all physics and rendering is complete
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.1f);

        IsProcessingConnections = false;
        Debug.Log("Door connection process complete");
    }

    private int GetDoorIndex(string doorName)
    {
        switch (doorName)
        {
            // Normal rooms and first set of big room doors
            case "NorthPoint": return 0;
            case "SouthPoint": return 1;
            case "EastPoint": return 2;
            case "WestPoint": return 3;
            // Additional big room doors
            case "NorthPoint (1)": return 4;
            case "SouthPoint (1)": return 5;
            case "EastPoint (1)": return 6;
            case "WestPoint (1)": return 7;
            default:
                Debug.LogWarning($"Unknown door name: {doorName}");
                return -1;
        }
    }

    public void ProcessConnections()
    {
        StartCoroutine(ProcessConnectionsCoroutine());
    }
}