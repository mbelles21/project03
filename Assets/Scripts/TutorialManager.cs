using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public List<GameObject> tutorials;
    private int tutorialNum;

    // Start is called before the first frame update
    void Start()
    {
        tutorialNum = 0;
    }

    // function will be called by player (OnTriggerEnter)
    public void DisplayTutorial()
    {
        if(tutorials[tutorialNum-1] != null) {
            tutorials[tutorialNum-1].SetActive(false); // turn off prev tutorial if it exists
        }

        tutorials[tutorialNum].SetActive(true);
        tutorialNum++;
    }
}
