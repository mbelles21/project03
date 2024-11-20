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
        EnemyStun enemy = other.gameObject.GetComponent<EnemyStun>();
        if(enemy != null){
            enemy.DealStun(damage);
        }
    }
}
