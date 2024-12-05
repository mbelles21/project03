using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int CurrentFloor = 1;
    public int maxFloor = 5;
    
    private UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        uiManager.UnPause(); // to make sure game starts not paused

        Debug.Log("Floor " + CurrentFloor);
        maxFloor++; // incrementing value bc floor counter skips from 1 to 3 for some reason
    }

    // TODO: call each time player goes between floors (still needed for going back up)
    public void SaveCheckpoint()
    {
        // save ammo count, timer value (TODO), current floor, and floor layout (TODO)
        PlayerPrefs.SetInt("flash", Inventory.FlashbangAmmo);
        PlayerPrefs.SetInt("taser", Inventory.TaserAmmo);
        PlayerPrefs.SetInt("floor", CurrentFloor);

        // Debug.Log("checkpoint reached");

        PlayerPrefs.Save();
    }

    public void GoDownFloor()
    {
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
