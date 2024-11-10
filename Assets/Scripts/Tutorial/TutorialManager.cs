using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public List<GameObject> tutorials;
    private GameObject prevTutorial;
    private int tutorialNum;
    private bool tutorialCompleted; // might want to change/use for future logic

    // Start is called before the first frame update
    void Start()
    {
        tutorialNum = 0;
        tutorialCompleted = false;
    }

    // function will be called by player (OnTriggerEnter)
    public void DisplayTutorial(int i)
    {
        if(prevTutorial != null) {
            prevTutorial.SetActive(false);
        }
        tutorials[i].SetActive(true);
        prevTutorial = tutorials[i];

        tutorialNum++;
        if(tutorialNum >= tutorials.Count) {
            tutorialCompleted = true;
        }
    }
}
