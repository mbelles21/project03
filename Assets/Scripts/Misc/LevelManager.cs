using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int CurrentFloor = 1;
    
    private UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        uiManager.UnPause(); // to make sure game starts not paused
    }

    // TODO: call each time player goes between floors (still needed for going back up)
    public void SaveCheckpoint()
    {
        // save ammo count, current floor, and floor layout (TODO)
        PlayerPrefs.SetInt("flash", Inventory.FlashbangAmmo);
        PlayerPrefs.SetInt("taser", Inventory.TaserAmmo);
        PlayerPrefs.SetInt("floor", CurrentFloor);

        PlayerPrefs.Save();
    }

    public void GoDownFloor()
    {
        //
    }

    public void GoUpFloor()
    {
        //
    }
}
