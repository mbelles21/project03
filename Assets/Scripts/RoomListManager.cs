using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListManager : MonoBehaviour
{
    public GameObject[] roomPrefabs;

    public GameObject GetRoomPrefab(int index)
    {
        return roomPrefabs[index];
    }

    private void OnValidate()
    {
        // Ensure room IDs match array indices
        if (roomPrefabs != null)
        {
            for (int i = 0; i < roomPrefabs.Length; i++)
            {
                if (roomPrefabs[i] != null)
                {
                    RoomID roomID = roomPrefabs[i].GetComponent<RoomID>();
                    if (roomID != null && roomID.roomID != i)
                    {
                        Debug.LogWarning($"Room prefab at index {i} has mismatched roomID: {roomID.roomID}");
                    }
                }
            }
        }
    }
}
