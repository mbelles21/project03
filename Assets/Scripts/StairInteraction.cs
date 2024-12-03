using UnityEngine;

public class StairInteraction : MonoBehaviour
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
            // Pass the stair position when moving to next floor
            DungeonFloorManager.Instance.MoveToNextFloor(transform.position);
        }
    }
}