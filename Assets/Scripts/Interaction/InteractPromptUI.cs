using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractPromptUI : MonoBehaviour
{
    private Camera mainCam; 
    public GameObject uiPanel;
    public TextMeshProUGUI promptText; 

    void Start()
    {
        mainCam = Camera.main;
        uiPanel.SetActive(false);
    }

    void LateUpdate()
    {
        var rotation = mainCam.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }

    public bool isDisplayed = false;

    public void SetUp(string prompt)
    {
        promptText.text = prompt;
        uiPanel.SetActive(true);
        isDisplayed = true;
    }

    public void Close()
    {
        isDisplayed = false;
        uiPanel.SetActive(false);
    }
}
