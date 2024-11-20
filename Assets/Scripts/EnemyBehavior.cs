using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public float fieldOfViewAngle = 120f; // Default FOV angle
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

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

    // Check if the player is within the specified field of view angle
    return angleBetweenEnemyAndPlayer < fieldOfViewAngle / 2;
}
    
    private void Patroling(){
        if(!walkPointSet) SearchWalkPoint();
        if(walkPointSet) agent.SetDestination(walkPoint);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if(distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint(){
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer(){
        agent.SetDestination(player.position);
    }

    private void AttackPlayer(){
        //Make sure the enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!alreadyAttacked){
            ///Attack code here
            ///
            
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack(){
        alreadyAttacked = false;
    }

    public void TakeStunDamage(float stun){
        
    }

    public void StunEnemy(){
        
    }
}
