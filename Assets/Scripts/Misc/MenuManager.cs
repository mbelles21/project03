using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private MainMenu mainMenu;

    private int savedFlashAmmo;
    private int savedTaserAmmo;
    private int savedFloor;
    private float savedTime;

    void Start()
    {
        mainMenu = GetComponent<MainMenu>();

        // get tutorial completion state
        int tutorialProgress = PlayerPrefs.GetInt("tutorial", 0);
        if(tutorialProgress == 1) {
            TutorialManager.TutorialCompleted = true;
        } else {
            TutorialManager.TutorialCompleted = false;
        }

        // get ammo 
        savedFlashAmmo = PlayerPrefs.GetInt("flash", 0);
        savedTaserAmmo = PlayerPrefs.GetInt("taser", 0);

        // get floor
        savedFloor = PlayerPrefs.GetInt("floor", 1);

        // get time
        savedTime = PlayerPrefs.GetFloat("timer", 0);
    }

    public void StartNewGame()
    {
        // set defaults for new game
        Inventory.FlashbangAmmo = 0;
        Inventory.TaserAmmo = 0;
        LevelManager.CurrentFloor = 1;
        UIManager.carriedTime = 0f;

        // to give option to replay tutorial level if already played
        if(TutorialManager.TutorialCompleted) {
            mainMenu.ActivateTutorialUI();
        }
        else {
            GoToTutorial();
        }
    }

    public void GoToTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void GoToLevel()
    {
        SceneManager.LoadScene("DungeonGeneration");
    }

    public void LoadGame()
    {
        // TODO: finish implementing
        // TODO: logic for going straight to treasure room (maybe not necessary bc checkpoint doesn't get triggered in TreasureRoom)
        // TODO: load timer value

        Inventory.FlashbangAmmo = savedFlashAmmo;
        Inventory.TaserAmmo = savedTaserAmmo;
        LevelManager.CurrentFloor = savedFloor;
        UIManager.carriedTime = savedTime;

        // Debug.Log("loading floor " + LevelManager.CurrentFloor);
        SceneManager.LoadScene("DungeonGeneration");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void QuitGame()
    {
        Debug.Log("quitting game...");
        Application.Quit();
    }
}
