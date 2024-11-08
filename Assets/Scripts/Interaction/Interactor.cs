using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public Transform interactPoint;
    public float interactRadius = 0.5f;
    public LayerMask interactMask;

    private readonly Collider[] colliders = new Collider[3];
    private int numFound;

    private IInteractable interactable;

    void Update()
    {
        numFound = Physics.OverlapSphereNonAlloc(interactPoint.position, interactRadius, colliders, interactMask);
        if(numFound > 0) {
            interactable = colliders[0].GetComponent<IInteractable>();

            if(interactable != null) {
                if(!interactable.GetInteractUIState()) {
                    interactable.ToggleInteractUI();
                }
            }
        }
        else {
            if(interactable != null) {
                if(interactable.GetInteractUIState()) { 
                    interactable.ToggleInteractUI();
                    interactable = null; // unassign interactable
                }
            }
        }
    }

    public void InteractWithObject()
    {
        if(interactable != null) {
            interactable.Interact(this);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactPoint.position, interactRadius);
    }
}
