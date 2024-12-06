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

    // values defined in constructor will be default values when no data to load
    public GameData()
    {
        enemyPositions = new List<Vector3>();
        enemyTypes = new List<int>();
        roomPositions = new List<Vector3>();
        roomTypes = new List<int>();
    }
}
