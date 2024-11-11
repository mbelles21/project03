using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    private LevelManager levelManager;
    private TutorialCollider tutorialCollider;

    void Start() 
    {
        levelManager = FindAnyObjectByType<LevelManager>();
    }

    void OnTriggerEnter(Collider collider)
    {
        // logic for loading next floor (if necessary)
        if(collider.CompareTag("Finish")) {
            if(levelManager != null) {
                levelManager.GoDownFloor();
            }
        }

        // logic for tutorials 
        if(collider.CompareTag("tutorial")) {
            tutorialCollider = collider.GetComponent<TutorialCollider>();
            if(tutorialCollider != null) {
                tutorialCollider.PassTutorialID(); // so TutorialManager doesn't have to be in this script
            }
            else {
                Debug.Log("could not get tutorial collider");
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
