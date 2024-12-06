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
}
