using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform target;
    private float distanceToPlayer;
    private Vector2 input;

    [SerializeField] private MouseSensitivity mouseSensitivity;
    private CameraRotation cameraRotation;
    [SerializeField] private CameraAngle cameraAngle;

    private void Awake(){
        distanceToPlayer = Vector3.Distance(transform.position, target.position);
    }

    private void LateUpdate(){
        transform.eulerAngles = new Vector3(cameraRotation.Pitch, cameraRotation.Yaw, 0.0f);
        transform.position = target.position - transform.forward * distanceToPlayer;
    }

    void Update(){
        cameraRotation.Yaw += input.x * mouseSensitivity.horizontal * BoolToInt(mouseSensitivity.invertHorizontal) * Time.deltaTime;
        cameraRotation.Pitch += input.y * mouseSensitivity.vertical * BoolToInt(mouseSensitivity.invertVertical) * Time.deltaTime;
        cameraRotation.Pitch = Mathf.Clamp(cameraRotation.Pitch, cameraAngle.min, cameraAngle.max);
    }

    public void Look(InputAction.CallbackContext context){
        input = context.ReadValue<Vector2>();
    }

    private static int BoolToInt(bool b) => b ? 1 : -1;
}

[Serializable]
public struct MouseSensitivity{
    public float horizontal;
    public float vertical;
    public bool invertVertical;
    public bool invertHorizontal;
}

public struct CameraRotation {
    public float Pitch;
    public float Yaw;
}

[Serializable]
public struct CameraAngle {
    public float min;
    public float max;
}