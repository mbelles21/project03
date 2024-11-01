using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        TutorialManager tutorialManager = collider.GetComponent<TutorialManager>();
        if (tutorialManager != null) {
            tutorialManager.DisplayTutorial();
            collider.transform.position += Vector3.forward * 5f; // move collider forward for next tutorial
        }
        else {
            Debug.Log("Collider does not have a TutorialManager component.");
        }
    }
}
