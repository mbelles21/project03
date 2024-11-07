using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    private LevelManager levelManager;
    private TutorialManager tutorialManager;

    void Start() 
    {
        levelManager = FindAnyObjectByType<LevelManager>();
        tutorialManager = FindAnyObjectByType<TutorialManager>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Finish")) {
            if(levelManager != null) {
                levelManager.GoDownFloor();
            }
        }

        // logic for tutorials 
        if(collider.CompareTag("tutorial")) {
            if (tutorialManager != null) {
                tutorialManager.DisplayTutorial();
            }
            else {
                Debug.Log("tutorial manager not connected");
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if(collider.CompareTag("tutorial")) {
            Debug.Log("trigger exitted");
            Destroy(collider.gameObject);
        }
    }
}
