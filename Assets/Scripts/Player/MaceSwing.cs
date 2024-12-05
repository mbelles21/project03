using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceSwing : MonoBehaviour
{
    public float stunDamage;
    public bool isActive = false;
    // Start is called before the first frame update
    public delegate void EnemyBonked();
    public static event EnemyBonked mace;

    public void OnTriggerEnter(Collider other){
        if(isActive){
            EnemyStun enemy = other.gameObject.GetComponent<EnemyStun>();
            if (enemy != null)
            {
                enemy.DealStun(stunDamage);
                mace.Invoke();
            }
        }
    }
}
