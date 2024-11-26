using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // make sure cursor is usable
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // make sure time is normal (mostly for the scene bg)
        Time.timeScale = 1f;
    }
}
