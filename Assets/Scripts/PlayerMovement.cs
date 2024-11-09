using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private Vector2 input;
    private CharacterController cc;
    private Vector3 direction;
    private float gravity = -9.81f;
    private float velocity;
    private float rotationSpeed = 500f;
    public Camera cam;
    public GameObject HandHit;
    public GameObject HipSpot;
    public GameObject Mace;
    private bool isAiming = false;
    private bool isSwinging = false;
    private bool isThrowing = false;
    private Animator anim;
    private MaceSwing maceSwing;
    [SerializeField] private Movement movement;
    public GameObject spawnPoint;
    public GameObject grenade;
    private GameObject thrownGrenade;
    
    void Start(){
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        maceSwing = Mace.GetComponent<MaceSwing>();
    }

    void Update(){
        ApplyRotation();
        ApplyGravity();
        ApplyMovement();
        if(thrownGrenade != null){
            thrownGrenade.transform.localPosition = new Vector3(0,0,0);
            Vector3 throwDirection = cam.transform.forward;
            GrenadeThrow grenade = thrownGrenade.GetComponent<GrenadeThrow>();
            grenade.ApplyDirection(new Vector3(throwDirection.x-0.1f, throwDirection.y+0.8f, throwDirection.z));
        }
    }

    private void ApplyGravity(){
        if(IsGrounded() && velocity < 0.0f){
            velocity = -1.0f;
        } else {
            velocity += gravity * Time.deltaTime;
        }
        direction.y = velocity;
    }

    private void ApplyRotation(){
        direction = Quaternion.Euler(0.0f, cam.transform.eulerAngles.y, 0.0f) * new Vector3(input.x, 0.0f, input.y);
        Vector3 cameraForward = cam.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        var targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void ApplyMovement(){
        var targetSpeed = movement.isSprinting ? movement.speed * movement.multiplier : movement.speed;
        movement.currentSpeed = Mathf.MoveTowards(movement.currentSpeed, targetSpeed, movement.acceleration * Time.deltaTime);
        cc.Move(direction * movement.currentSpeed * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context){
        if(context.started){
            anim.SetBool("Walking", true);
        } else if(context.canceled){
            anim.SetBool("Walking", false);
        }
        input = context.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0, input.y);
    }

    public void Sprint(InputAction.CallbackContext context){
        if(context.started){
            movement.isSprinting = true;
        } else if (context.canceled) {
            movement.isSprinting= false;
        }
    }

    public void Interact(InputAction.CallbackContext context){
        if (context.started) {
            Debug.Log("Interact With Item");
            GetComponent<Interactor>().InteractWithObject();
        }
    }

    public void Aim(InputAction.CallbackContext context){
        // only allow if player has grenades to use (note: logic will need to be changed if adding different throwables)
        if(Inventory.GrenadeCount > 0) {
            if(context.started){
                Debug.Log("Aiming");
                isAiming = true;
                anim.SetBool("Aiming", true);
                thrownGrenade = Instantiate(grenade, spawnPoint.transform.position, Quaternion.identity);
                thrownGrenade.transform.SetParent(spawnPoint.transform, false);
            }
            if(context.canceled){
                isAiming = false;
                anim.SetBool("Aiming", false);
                if(thrownGrenade != null && !isThrowing){
                    GrenadeThrow grenade = thrownGrenade.GetComponent<GrenadeThrow>();
                    grenade.DestroyGrenade();
                }
            }
        }
        else {
            // repeat logic to prevent getting stuck in throwing state
            Debug.Log("no grenades");
            isAiming = false;
            anim.SetBool("Aiming", false);
            if(thrownGrenade != null && !isThrowing){
                GrenadeThrow grenade = thrownGrenade.GetComponent<GrenadeThrow>();
                grenade.DestroyGrenade();
            }
        }
    }

    public void Attack(InputAction.CallbackContext context){
        if(context.started){
            if(isAiming){
                Debug.Log("Throwing");
                if(!isThrowing && !isSwinging){
                    anim.SetTrigger("Throw");
                    StartCoroutine(Throw());
                }
            } else {
                Debug.Log("Attacking");
                if(!isSwinging && !isThrowing){
                    Mace.transform.SetParent(HandHit.transform, false);
                    Mace.transform.localPosition = new Vector3(0.2794f, -0.225f, 0.0393f);
                    Mace.transform.localRotation = Quaternion.Euler(23.548f, 36.662f, -14.287f);
                    StartCoroutine(Swing());
                    anim.SetTrigger("Hit");
                }
            }
        }
    }
    public IEnumerator Swing(){
        maceSwing.isActive = true;
        isSwinging = true;
        yield return new WaitForSeconds(2.1f);
        Debug.Log("Done Swinging");
        maceSwing.isActive = true;
        isSwinging = false;
        Mace.transform.SetParent(HipSpot.transform, false);
        Mace.transform.localPosition = new Vector3(0.191f, 1.075f, -0.044f);
        Mace.transform.localRotation = Quaternion.Euler(-180f, 0f, -15f);
    }

    public IEnumerator Throw(){
        Inventory.GrenadeCount--; // use a grenade
        Debug.Log("Grenades: " + Inventory.GrenadeCount);

        isThrowing = true;
        GrenadeThrow grenadeScript = thrownGrenade.GetComponent<GrenadeThrow>();
        grenadeScript.TurnOffLine();
        yield return new WaitForSeconds(.8f);

        Debug.Log("Done Throwing");
        thrownGrenade.transform.SetParent(null, false);
        thrownGrenade.transform.localPosition = spawnPoint.transform.position;
        thrownGrenade = null;
        if(grenadeScript != null){
            Vector3 throwDirection = cam.transform.forward;
            throwDirection = new Vector3(throwDirection.x, throwDirection.y+2f, throwDirection.z);
            grenadeScript.Throw(throwDirection);
        }
        yield return new WaitForSeconds(.4f);
        isThrowing = false;
    }

    private bool IsGrounded() => cc.isGrounded;
}

[Serializable]
public struct Movement{
    public float speed;
    public float multiplier;
    public float acceleration;
    [HideInInspector] public bool isSprinting;
    [HideInInspector] public float currentSpeed;
}
