using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    private float damage;

    void Start(){
        Destroy(gameObject, 2f);
    }

    public void SetDamage(float grenade){
        damage = grenade;
    }

    public void OnTriggerEnter(Collider other){
        EnemyBehavior enemy = other.gameObject.GetComponent<EnemyBehavior>();
        if(enemy != null){
            enemy.TakeStunDamage(damage);
        }
    }
}
