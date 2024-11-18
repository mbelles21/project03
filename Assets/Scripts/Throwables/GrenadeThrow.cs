using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeThrow : MonoBehaviour
{
    public int Type;
    //1 = flashbang, 2 = stun ball
    public bool AimingCurrently;
    [Range(0, -10)]
    public float customGravity;
    private Vector3 gravity;
    [Range(0, 20)]
    public float throwSpeed;
    public float stunDamage = 75f;
    private Rigidbody rb;
    private Vector3 throwDir;
    private GameObject spawnPoint;
    public GameObject flash;
    private ItemTrajectory itemTrajectory;
    // Start is called before the first frame update
    void Awake() {
        rb = GetComponent<Rigidbody>();
        itemTrajectory = GetComponentInChildren<ItemTrajectory>();
        rb.useGravity = false;
        gravity = new Vector3(0, customGravity, 0);
    }

    void Start(){
        AimingCurrently = true;
        itemTrajectory.enabled = true;
    }

    void Update() {
        if(!AimingCurrently) {
            ApplyCustomGravity();
        }
    }

    public void OnTriggerEnter(Collider other){
        if(other.CompareTag("Player")){
            return;
        }
        if(Type == 1){
            GameObject flashbang = Instantiate(flash, transform.position, Quaternion.identity);
            Flash flasher = flashbang.GetComponent<Flash>();
            flasher.SetDamage(stunDamage);
        } else {
            //Stun Enemy AI
            EnemyBehavior enemy = other.gameObject.GetComponent<EnemyBehavior>();
            if(enemy != null){
                enemy.StunEnemy();
            }
        }
    }

    public void Throw(Vector3 throwDirection){
        AimingCurrently = false;
        throwDir = throwDirection;
        rb.AddForce(throwDir * throwSpeed, ForceMode.Impulse);
    }

    public void DestroyGrenade(){
        Destroy(gameObject);
    }

    public void ApplyDirection(Vector3 throwDirection){
        throwDir = throwDirection;
    }

    public void ApplyCustomGravity()
    {
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    public float GetCustomGravity(){
        return customGravity;
    }

    public float GetThrowSpeed(){
        return throwSpeed;
    }

    public Vector3 GetDirection(){
        return throwDir;
    }
    public void TurnOffLine(){
        itemTrajectory.enabled = false;
    }
}