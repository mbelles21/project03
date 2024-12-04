using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider slider;
    public float maxHealth = 100f;
    public float currentHealth = 0f;
    public delegate void PlayerDied();
    public static event PlayerDied playerDied;

    public void Start(){
        currentHealth = maxHealth;
        slider.value = currentHealth;
        EnemyBehavior.HitPlayer += TakeDamage;
    }

    public void OnDestroy(){
        EnemyBehavior.HitPlayer -= TakeDamage;
    }

    public void TakeDamage(float damage){
        currentHealth -= damage;
        slider.value = currentHealth;
        if(currentHealth <= 0){
            //End Game
            playerDied.Invoke();
        }
    }
}
