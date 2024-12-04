using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private MainMenu mainMenu;

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
        int savedFlashAmmo = PlayerPrefs.GetInt("flash", 0);
        int savedTaserAmmo = PlayerPrefs.GetInt("taser", 0);

        // get floor
        int savedFloor = PlayerPrefs.GetInt("floor", 1);
    }

    public void StartNewGame()
    {
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
        Debug.Log("not yet implemented");
        // TODO: implement
        // TODO: logic for going straight to treasure room 
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
