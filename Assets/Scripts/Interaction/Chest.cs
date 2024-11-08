using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public string prompt;
    public InteractPromptUI interactPromptUI;

    void Start()
    {
        interactPromptUI = GetComponentInChildren<InteractPromptUI>();
    }

    public string InteractPrompt => prompt; // return the prompt

    public bool Interact(Interactor interactor)
    {
        Debug.Log("opening chest");
        // TODO: chest logic
        return true;
    }

    public bool GetInteractUIState()
    {
        return interactPromptUI.isDisplayed;
    }

    public void ToggleInteractUI()
    {
        bool state = interactPromptUI.isDisplayed;
        if(!state) {
            interactPromptUI.SetUp(prompt);
        }
        else {
            interactPromptUI.Close();
        }
    }
}
