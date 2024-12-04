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
