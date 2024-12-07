using UnityEngine;

public class StairInteraction : MonoBehaviour, IInteractable
{
    // private bool canInteract = false;
    public string prompt;
    public InteractPromptUI interactPromptUI;

    public string InteractPrompt => prompt; // return the prompt

    public bool Interact(Interactor interactor)
    {
        DungeonFloorManager.Instance.MoveToNextFloor(transform.position);
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

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         canInteract = true;
    //     }
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         canInteract = false;
    //     }
    // }

    // private void Update()
    // {
    //     if (canInteract && Input.GetKeyDown(KeyCode.E))
    //     {
    //         // Pass the stair position when moving to next floor
    //         DungeonFloorManager.Instance.MoveToNextFloor(transform.position);
    //     }
    // }
}