using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject replayTutorialOptionsUI;

    // Start is called before the first frame update
    void Start()
    {
        // make sure cursor is usable
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // make sure time is normal (mostly for the scene bg)
        Time.timeScale = 1f;

        replayTutorialOptionsUI.SetActive(false); // ensure ui is off when scene starts
    }

    public void ActivateTutorialUI()
    {
        replayTutorialOptionsUI.SetActive(true);
    }
}
