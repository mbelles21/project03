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

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake(){
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
{
    // Check if the player is within sight range
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
    playerInSightRange = distanceToPlayer <= sightRange && IsPlayerInFieldOfView();

    // Check if the player is within attack range
    playerInAttackRange = distanceToPlayer <= attackRange;

    if (!playerInSightRange && !playerInAttackRange)
    {
        Patroling();
    }
    if (playerInSightRange && !playerInAttackRange)
    {
        ChasePlayer();
    }
    if (playerInSightRange && playerInAttackRange)
    {
        AttackPlayer();
    }
}
private bool IsPlayerInFieldOfView()
{
    Vector3 directionToPlayer = (player.position - transform.position).normalized;
    float angleBetweenEnemyAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);

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
