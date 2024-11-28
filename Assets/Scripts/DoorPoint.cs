using UnityEngine;

public class DoorPoint : MonoBehaviour
{
    void Start()
    {
        // Just add the trigger collider
        BoxCollider trigger = gameObject.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(2f, 2f, 2f);
    }

    void OnDrawGizmos()
    {
        // Draw red box by default
        Gizmos.color = Color.red;

        // Check for nearby doors and draw green if found
        Collider[] nearby = Physics.OverlapSphere(transform.position, 1f);
        foreach (Collider other in nearby)
        {
            if (other.GetComponent<DoorPoint>() != null && other.gameObject != gameObject)
            {
                Gizmos.color = Color.green;
                break;
            }
        }

        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}