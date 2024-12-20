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
    public GameObject deathScreen;
    private bool timerRunning = false;

    public static float carriedTime = 0f;

    public GameObject settings;
    
    public void Awake(){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Start(){
        PlayerHealth.playerDied += Died;

        levelTimer = carriedTime;
    }

    public void OnDestroy(){
        PlayerHealth.playerDied -= Died;
    }

    public void Update(){
        if(timerRunning) {
            levelTimer += Time.deltaTime;
            UpdateTimerText();
        }
        else {
            UpdateTimerText(); // to ensure it's accurate when it stops
        }
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
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                player.enabled = false;
                cinemachineCamera.enabled = false;
                Time.timeScale = 0f;
            } else {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                cinemachineCamera.enabled = true;
                player.enabled = true;
                Time.timeScale = PlayerPrefs.GetFloat("timeScale");
            }
        }
    }

    // used by level manager, not pause menu
    public void UnPause(){
        togglePause = false;
        pauseMenu.SetActive(togglePause);
        cinemachineCamera.enabled = true;
        player.enabled = true;
        Time.timeScale = PlayerPrefs.GetFloat("timeScale");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void GameCleared()
    {
        timerRunning = false; // stop timer when game cleared
        // duplicate functionality bc Pause function needs a parameter that doesn't exist at this point
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        player.enabled = false;
        cinemachineCamera.enabled = false;
        Time.timeScale = 0f;

        ResultsManager resultsManager = FindAnyObjectByType<ResultsManager>();
        resultsManager.GetResults(levelTimer);
    }

    public void Died(){
        togglePause = !togglePause;
        deathScreen.SetActive(togglePause);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        player.enabled = false;
        cinemachineCamera.enabled = false;
        Time.timeScale = 0f;
    }


    public void StoreTimerValue()
    {
        ChangeTimerState();
        carriedTime = levelTimer;
    }

    public void ChangeTimerState()
    {
        timerRunning = !timerRunning;
    }
    
    public void TurnOnSettings(){
        settings.SetActive(true);
    }

    public void TurnOffSettings(){
        settings.SetActive(false);
    }
}
