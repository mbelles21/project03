using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 0f;
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        EnemyBehavior.hitPlayer += takeDamage;
        slider.value = currentHealth;
    }

    void OnDestroy(){
        EnemyBehavior.hitPlayer -= takeDamage;
    }

    public void takeDamage(float damage){
        currentHealth -= damage;
        slider.value = currentHealth;
        if(currentHealth <= 0){
            //Kill Player
        }
    }
}
