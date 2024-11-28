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

    public void GoDownFloor()
    {
        // will need extra logic for keeping track of progression
        Debug.Log("going down...");
        
        TutorialManager tutorialManager = GetComponent<TutorialManager>();

        // if scene has TutorialManager it's the tutorial, else it's a normal level
        // TODO: change logic when getting the real level
        if(tutorialManager != null) {
            SceneManager.LoadScene("DemoLevel");
        }
        else {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void GoUpFloor()
    {
        // will also need extra logic for keeping track of progression
        Debug.Log("going up...");
    }

    public void EndTutorial()
    {
        Debug.Log("tutorial complete");
        
    }
}
