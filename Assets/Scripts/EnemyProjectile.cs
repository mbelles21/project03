using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f; // Speed of the projectile
    public float lifetime = 5f; // Time before the projectile is destroyed
    private Vector3 direction; // Direction to move the projectile

    public void Initialize(Vector3 targetDirection)
    {
        direction = targetDirection.normalized;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy after a certain time
    }

    private void Update()
    {
        // Move the projectile in the assigned direction
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Projectile hit: {other.name}");
        Destroy(gameObject); // Destroy the projectile on impact
    }
}
