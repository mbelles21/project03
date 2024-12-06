using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    void LoadData(GameData gameData);
    void SaveData(ref GameData gameData);  // pass by reference so implementing scripts can modify the data
}
