using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStun : MonoBehaviour
{
    public float stunLength = 5f;
    public float maxStun = 100f;
    public float currStun = 0f;
    public bool isStunned = false;
    private Animator anim;

    public void Start(){
        anim = GetComponentInChildren<Animator>();
    }
    
    
    public void DealStun(float stun){
        currStun += stun;
        if(currStun >= maxStun){
            StartCoroutine(Stunned());
        }
    }

    public void StunEnemy(){
        currStun = maxStun;
        StartCoroutine(Stunned());
    }

    public IEnumerator Stunned(){
        anim.SetTrigger("Stun");
        isStunned = true;
        yield return new WaitForSeconds(stunLength);
        isStunned = false;
        currStun = 0f;
        anim.SetTrigger("Wake");
    }
}
