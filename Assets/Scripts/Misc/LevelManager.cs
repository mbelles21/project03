using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static bool IsLoadedFloor = false;
    public static int CurrentFloor = 1;
    public int maxFloor = 5;
    
    private UIManager uiManager;

    // delayed slightly in project settings so DataPersistenceManager can execute its Start before this
    void Start()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        uiManager.UnPause(); // to make sure game starts not paused

        // dataPersistenceManager = GetComponent<DataPersistenceManager>();

        Debug.Log("Floor " + CurrentFloor);
        maxFloor++; // incrementing value bc floor counter skips from 1 to 3 for some reason

        if(IsLoadedFloor) {
            DataPersistenceManager.instance.LoadGame();

            // also move player to start of floor
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = Vector3.zero;
        }        
    }

    public void SaveCheckpoint()
    {
        // save floor layout and enemy positions
        DataPersistenceManager.instance.SaveGame();

        // save ammo count, health, timer value, current floor
        PlayerPrefs.SetInt("flash", Inventory.FlashbangAmmo);
        PlayerPrefs.SetInt("taser", Inventory.TaserAmmo);
        PlayerPrefs.SetFloat("health", PlayerHealth.SavedHealth);
        PlayerPrefs.SetInt("floor", CurrentFloor);
        PlayerPrefs.SetFloat("timer", UIManager.carriedTime);

        // Debug.Log("checkpoint reached");

        PlayerPrefs.Save();

        uiManager.ChangeTimerState();
    }

    public void GoDownFloor()
    {
        uiManager.StoreTimerValue(); // stop timer

        CurrentFloor++;

        if(CurrentFloor == maxFloor) {
            // load treasure scene
            SceneManager.LoadScene("TreasureRoom");
        }
        else if(CurrentFloor >= maxFloor) {
            // just in case it increments weirdly again
            Debug.Log("incremented incorrectly for some reason (see LevelManager)");
        }
        else {
            // regenerate level
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }

    public void GoUpFloor()
    {
        //
    }
}
