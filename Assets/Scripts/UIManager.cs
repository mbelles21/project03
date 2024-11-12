using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Cinemachine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float levelTimer = 0f; 
    public GameObject pauseMenu;
    private bool togglePause;
    public PlayerMovement player;
    public CinemachineVirtualCamera cinemachineCamera;

    public void Update(){
        levelTimer += Time.deltaTime;
        UpdateTimerText();
    }

    void UpdateTimerText() {
        float mins = Mathf.FloorToInt(levelTimer/60);
        float secs = Mathf.FloorToInt(levelTimer%60);
        timerText.text = string.Format("{0:0}:{1:00}", mins, secs); // displays text in timer format
    }

    public void Pause(InputAction.CallbackContext context){
        if(context.started){
            togglePause = !togglePause;
            pauseMenu.SetActive(togglePause);
            if(togglePause){
                player.enabled = false;
                cinemachineCamera.enabled = false;
                Time.timeScale = 0f;
            } else {
                cinemachineCamera.enabled = true;
                player.enabled = true;
                Time.timeScale = 1f;
            }
        }
    }

    public void UnPause(){
        togglePause = false;
        pauseMenu.SetActive(togglePause);
        cinemachineCamera.enabled = true;
        player.enabled = true;
        Time.timeScale = 1f;
    }
}
