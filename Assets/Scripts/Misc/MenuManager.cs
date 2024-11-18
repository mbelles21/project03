using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartNewGame()
    {
        SceneManager.LoadScene("Tutorial");
        // TODO: add extra functionality to skip tutorial if game already completed
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
