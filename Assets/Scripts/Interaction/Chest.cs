using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public string prompt;
    public InteractPromptUI interactPromptUI;
    public GameObject chestLid;
    public GameObject itemPopupUI;

    [Header("Item Rates")]
    public float flashChance;
    public float taserChance;

    private bool isOpen = false; 
    private Inventory playerInventory;

    public string InteractPrompt => prompt; // return the prompt

    public bool Interact(Interactor interactor)
    {
        if(!isOpen) {
            Debug.Log("opening chest");
            isOpen = true;
            chestLid.transform.Rotate(0f, 0f, -45f);
            interactPromptUI.Close();

            playerInventory = interactor.GetComponent<Inventory>(); // assigned here bc interactor
            if(playerInventory != null) {
                GiveItem();
            }
            else {
                Debug.Log("no inventory");
            }

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

    void GiveItem()
    {
        string itemName = "";
        float rand = Random.value;
        if(rand < taserChance) {
            Inventory.TaserAmmo++;
            itemName = "Taser";
        }
        else if(rand < flashChance) {
            Inventory.FlashbangAmmo++;
            itemName = "Flashbang";
        }
        playerInventory.UpdateUIText();

        itemPopupUI.SetActive(true);
        itemPopupUI.GetComponent<PopupUI>().UpdateItemPopupText(itemName); // update text on UI
    }
}
