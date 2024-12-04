using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    void OnDrawGizmos()
    {
        // Draw a visual indicator in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawRay(transform.position, Vector3.up);
    }
}