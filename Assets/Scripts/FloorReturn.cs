using UnityEngine;

public class FloorReturnDoor : MonoBehaviour
{
    private bool canInteract = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
        }
    }

    private void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            DungeonFloorManager.Instance.ReturnToPreviousFloor();
        }
    }
}