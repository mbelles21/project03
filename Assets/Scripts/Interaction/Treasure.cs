using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour, IInteractable
{
    public string prompt;
    public InteractPromptUI interactPromptUI;
    public GameObject treasureObj;

    private bool isTaken = false; 

    public string InteractPrompt => prompt; // return the prompt

    public bool Interact(Interactor interactor)
    {
        if(!isTaken) {
            Debug.Log("grabbed treasure");
            isTaken = true;
            treasureObj.SetActive(false);
            interactPromptUI.Close();
            return true;
        }
        else {
            Debug.Log("no more treasure");
            return false; // cannot be opened again
        }
        
    }

    public bool GetInteractUIState()
    {
        return interactPromptUI.isDisplayed;
    }

    public void ToggleInteractUI()
    {
        bool state = interactPromptUI.isDisplayed;
        if(!state) {
            if(!isTaken) {
                interactPromptUI.SetUp(prompt); // only show prompt if interactable
            }
        }
        else {
            interactPromptUI.Close();
        }
    }
}
