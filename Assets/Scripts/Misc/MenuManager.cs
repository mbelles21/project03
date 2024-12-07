using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private MainMenu mainMenu;

    private int savedFlashAmmo;
    private int savedTaserAmmo;
    private float savedHealth;
    private int savedFloor;
    private float savedTime;

    public GameObject settings;
    public Button loadButton;

    void Start()
    {
        mainMenu = GetComponent<MainMenu>();

        // get tutorial completion state
        int tutorialProgress = PlayerPrefs.GetInt("tutorial", 0);
        if(tutorialProgress == 1) {
            TutorialManager.TutorialCompleted = true;
        } else {
            TutorialManager.TutorialCompleted = false;
            loadButton.interactable = false;
        }

        // get ammo 
        savedFlashAmmo = PlayerPrefs.GetInt("flash", 0);
        savedTaserAmmo = PlayerPrefs.GetInt("taser", 0);

        // get health
        savedHealth = PlayerPrefs.GetFloat("health", 0); // default to 0 bc logic in PlayerHealth will handle that

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
        PlayerHealth.SavedHealth = 0f;
        LevelManager.CurrentFloor = 1;
        UIManager.carriedTime = 0f;
        LevelManager.IsLoadedFloor = false;

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
        Inventory.FlashbangAmmo = savedFlashAmmo;
        Inventory.TaserAmmo = savedTaserAmmo;
        PlayerHealth.SavedHealth = savedHealth;
        LevelManager.CurrentFloor = savedFloor;
        UIManager.carriedTime = savedTime;
        LevelManager.IsLoadedFloor = true; // TODO: disable this button if no saved data for room so no bugs occur

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

    public void OpenSettings(){
        settings.SetActive(true);
    }

    public void CloseSettings(){
        settings.SetActive(false);
    }
}
