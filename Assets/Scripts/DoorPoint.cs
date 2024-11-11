using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPoint : MonoBehaviour
{
    public DoorDirection direction;
    public bool isConnected = false;
    public bool optional = false;  // Whether this door must be connected
    public GameObject doorBlocker;

    public void SealDoorway()
    {
        if (doorBlocker != null)
        {
            doorBlocker.SetActive(true);
        }
    }

    public void OpenDoorway()
    {
        if (doorBlocker != null)
        {
            doorBlocker.SetActive(false);
        }
    }
}

public enum DoorDirection
{
    North,
    South,
    East,
    West
}