using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public List<GameObject> tutorials;
    private GameObject prevTutorial;
    private int tutorialNum;
    private bool tutorialCompleted;

    // Start is called before the first frame update
    void Start()
    {
        tutorialNum = 0;
        tutorialCompleted = false;
    }

    // function will be called by player (OnTriggerEnter)
    public void DisplayTutorial()
    {
        if(tutorialCompleted) {
            return; // exit early if done already
        }

        if(tutorialNum < tutorials.Count) {
            tutorials[tutorialNum].SetActive(true);
            if(prevTutorial != null) {
                prevTutorial.SetActive(false);
            }

            prevTutorial = tutorials[tutorialNum]; // save obj so it can be turned off next time func is called
            tutorialNum++;
        }
        else {
            // disable last tutorial popup
            prevTutorial.SetActive(false);
            tutorialCompleted = true;
            Debug.Log("tutorial complete");
        }
    }
}
