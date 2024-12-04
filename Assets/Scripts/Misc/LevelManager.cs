using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int CurrentFloor = 1;
    
    private UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        uiManager.UnPause(); // to make sure game starts not paused
    }

    public void GoDownFloor()
    {
        //
    }

    public void GoUpFloor()
    {
        //
    }
}
