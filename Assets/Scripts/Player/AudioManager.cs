using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource Music;
    public AudioSource Walking;
    public AudioSource taserSound;
    public AudioSource flashBang;
    public AudioSource maceHit;
    public AudioSource playerHit;
    public AudioSource fireBall;
    // Start is called before the first frame update
    void Start()
    {
        Music.Play();
        PlayerMovement.StartWalk += StartWalking;
        PlayerMovement.StopWalk += StopWalking;
        GrenadeThrow.taserSound += Taser;
        GrenadeThrow.flashSound += Flash;
        MaceSwing.mace += MaceHit;
        EnemyBehavior.playerSound += PlayerHit;
        EnemyBehavior.fire += FireBall;
    }

    void OnDestroy()
    {
        PlayerMovement.StartWalk -= StartWalking;
        PlayerMovement.StopWalk -= StopWalking;
        GrenadeThrow.taserSound -= Taser;
        GrenadeThrow.flashSound -= Flash;
        MaceSwing.mace -= MaceHit;
        EnemyBehavior.playerSound -= PlayerHit;
        EnemyBehavior.fire -= FireBall;
    }

    public void StartWalking(){
        Walking.Play();
    }

    public void StopWalking(){
        Walking.Stop();
    }

    public void Taser(){
        taserSound.Play();
    }

    public void Flash(){
        flashBang.Play();
    }

    public void MaceHit(){
        maceHit.Play();
    }

    public void PlayerHit(){
        playerHit.Play();
    }

    public void FireBall(){
        fireBall.Play();
    }
}
