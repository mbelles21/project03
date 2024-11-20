using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class EnemyBehavior : MonoBehaviour
{
    public enum EnemyType { Melee, Ranged } // Define types of enemies
    public EnemyType enemyType; // Choose Melee or Ranged in the Inspector

    public Transform[] waypoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 5f;
    public float chaseStopRange = 6f;

    // Ranged-specific properties
    public float attackRange = 8f;
    public float attackCooldown = 2f;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;

    // Melee-specific properties
    public float meleeAttackRange = 1.5f;
    public float meleeAttackCooldown = 1f;
    public float meleeDamage = 20f;

    public Transform player;

    private int currentWaypointIndex = 0;
    private bool isChasing = false;
    private bool isStunned = false;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private float lastAttackTime;

    public float obstacleDetectionDistance = 1.5f; // Distance to check for obstacles
    public float jumpForce = 5f; // Force applied to the enemy's Rigidbody for jumping

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        rb.useGravity = true; // Ensure gravity is enabled
        rb.freezeRotation = true; // Prevent physics-induced rotation
    }

    private void Update()
    {
        if (isStunned)
        {
            rb.velocity = Vector3.zero; // Stop all movement while stunned
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (enemyType == EnemyType.Ranged && distanceToPlayer <= attackRange && distanceToPlayer > detectionRange)
        {
            // Ranged enemy attacks if within attack range but outside detection range
            AttackRanged();
        }
        else if (enemyType == EnemyType.Melee && distanceToPlayer <= meleeAttackRange)
        {
            // Melee enemy attacks if within melee range
            AttackMelee();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // Start chasing the player
            isChasing = true;
        }
        else if (isChasing && distanceToPlayer > chaseStopRange)
        {
            // Stop chasing if the player is out of range
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else if (distanceToPlayer > attackRange)
        {
            Patrol();
        }

        RotateTowards(player.position); // Always face the player
    }

    private void Patrol()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        MoveTowards(targetWaypoint.position, patrolSpeed);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 2f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void ChasePlayer()
    {
        MoveTowards(player.position, chaseSpeed);
    }

    private void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        Vector3 velocity = direction * speed;
        velocity.y = rb.velocity.y; // Retain vertical velocity for gravity
        rb.velocity = velocity;
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Ignore vertical differences

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void AttackRanged()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Debug.Log("Enemy is attacking with a ranged projectile!"); // Debug log
            lastAttackTime = Time.time;

            if (projectilePrefab != null && projectileSpawnPoint != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
                
                if (projectileScript != null)
                {
                    Vector3 direction = (player.position - projectileSpawnPoint.position).normalized;
                    projectileScript.Initialize(direction);
                    Debug.Log("Projectile initialized with direction.");
                }
            }
        }
    }



    private void AttackMelee()
    {
        if (Time.time - lastAttackTime >= meleeAttackCooldown)
        {
            lastAttackTime = Time.time;

            // Deal damage to the player (replace this with your own damage logic)
            Debug.Log($"Melee enemy attacked the player for {meleeDamage} damage!");

            // Optionally, apply damage directly to the player's health script (if implemented)
        }
    }

    public void StunEnemy(float duration = 2f)
    {
        if (!isStunned)
        {
            isStunned = true;
            StartCoroutine(RecoverFromStun(duration));
        }
    }

    public void TakeStunDamage(float damage = 10f, float stunDuration = 2f)
    {
    StunEnemy(stunDuration);
    Debug.Log($"Enemy took {damage} damage and is stunned for {stunDuration} seconds.");
    }


    private IEnumerator RecoverFromStun(float duration)
    {
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }
}
