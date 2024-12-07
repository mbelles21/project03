using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<Vector3> enemyPositions;
    public List<int> enemyTypes;
    public List<Vector3> roomPositions;
    public List<int> roomTypes;
    public List<DoorData> doorStates;    // Add this for door states

    [System.Serializable]
    public class DoorData
    {
        public Vector3 roomPosition;      // Position of the room this door belongs to
        public int wallIndex;            // Index of the wall section
        public bool isDoorway;           // Whether it's a doorway or wall
    }

    // values defined in constructor will be default values when no data to load
    public GameData()
    {
        enemyPositions = new List<Vector3>();
        enemyTypes = new List<int>();
        roomPositions = new List<Vector3>();
        roomTypes = new List<int>();
        doorStates = new List<DoorData>();
    }
}

// [System.Serializable]
// public class GameData
// {
//     public List<Vector3> enemyPositions;
//     public List<int> enemyTypes;
//     public List<Vector3> roomPositions;
//     public List<int> roomTypes;

//     // values defined in constructor will be default values when no data to load
//     public GameData()
//     {
//         enemyPositions = new List<Vector3>();
//         enemyTypes = new List<int>();
//         roomPositions = new List<Vector3>();
//         roomTypes = new List<int>();
//     }
// }
