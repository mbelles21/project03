using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCollider : MonoBehaviour
{
    public int tutorialID;
    private TutorialManager tutorialManager;

    void Start()
    {
        tutorialManager = FindAnyObjectByType<TutorialManager>();
    }

    public void PassTutorialID()
    {
        tutorialManager.DisplayTutorial(tutorialID);
    }
}
