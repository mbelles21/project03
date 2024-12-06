using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupUI : MonoBehaviour
{
    public TextMeshProUGUI itemGottenText;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // disable after clicking
        if(Input.GetMouseButtonDown(0)) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;

            gameObject.SetActive(false);
        }
    }

    public void UpdateItemPopupText(string itemName)
    {
        itemGottenText.text = "Got 1 " + itemName;
    }
}
