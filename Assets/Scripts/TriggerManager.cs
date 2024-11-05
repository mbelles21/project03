using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    public TutorialManager tutorialManager;

    void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("tutorial")) {
            if (tutorialManager != null) {
                tutorialManager.DisplayTutorial();
            }
            else {
                Debug.Log("Collider does not have a TutorialManager component.");
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
