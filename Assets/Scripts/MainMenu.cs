using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // so mouse reappears when returning from DemoLevel
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
