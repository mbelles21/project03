using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public string prompt;
    public InteractPromptUI interactPromptUI;
    public GameObject chestLid;

    private bool isOpen = false; 

    void Start()
    {
        interactPromptUI = GetComponentInChildren<InteractPromptUI>();
    }

    public string InteractPrompt => prompt; // return the prompt

    public bool Interact(Interactor interactor)
    {
        if(!isOpen) {
            Debug.Log("opening chest");
            isOpen = true;
            chestLid.transform.Rotate(0f, 0f, -45f);
            interactPromptUI.Close();

            // TODO: get an item

            return true;
        }
        else {
            Debug.Log("chest already opened");
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
            if(!isOpen) {
                interactPromptUI.SetUp(prompt); // only show prompt if chest closed
            }
        }
        else {
            interactPromptUI.Close();
        }
    }
}
