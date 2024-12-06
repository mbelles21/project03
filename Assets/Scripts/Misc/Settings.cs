using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Audio;
using TMPro;


public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider ySlider;
    [SerializeField] private Slider xSlider;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private Toggle xInvert;
    [SerializeField] private Toggle yInvert;
    public GameObject Player;
    private CinemachineVirtualCamera virtualCamera;
    private CinemachinePOV pov;

    public TMP_Text musicValue;
    public TMP_Text sfxValue;
    public TMP_Text yValue;
    public TMP_Text xValue;
    public TMP_Text timeValue;
    private bool noUpdate = false;
    private bool isPaused = false;
    void Start()
    {
        virtualCamera = Player.GetComponentInChildren<CinemachineVirtualCamera>();
        pov = virtualCamera.GetComponentInChildren<CinemachinePOV>();
        if(PlayerPrefs.HasKey("xSensitivity")){
            LoadSettings();
        } else {
            SetXSensitivity();
            SetYSensitivity();
            SetTimeScale();
            SetMusicVolume();
            SetSfxVolume();
            SetXToggle();
            SetYToggle();
        }
    }

    public void Update(){
        musicValue.text = PlayerPrefs.GetFloat("musicVolume").ToString("F2");
        sfxValue.text = PlayerPrefs.GetFloat("sfxVolume").ToString("F2");
        xValue.text = PlayerPrefs.GetFloat("xSensitivity").ToString("F2");
        yValue.text = PlayerPrefs.GetFloat("ySensitivity").ToString("F2");
        timeValue.text = PlayerPrefs.GetFloat("timeScale").ToString("F2");
    }

    public void SetAllSettings(){
        SetXSensitivity();
        SetYSensitivity();
        SetTimeScale();
        SetMusicVolume();
        SetSfxVolume();
        SetXToggle();
        SetYToggle();
    }

    private void LoadSettings(){
        xSlider.value = PlayerPrefs.GetFloat("xSensitivity");
        SetXSensitivity();
        ySlider.value = PlayerPrefs.GetFloat("ySensitivity");
        SetYSensitivity();
        timeSlider.value = PlayerPrefs.GetFloat("timeScale");
        SetTimeScale();
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        SetSfxVolume();
        xInvert.isOn = PlayerPrefs.GetInt("xToggle") == 1;
        SetXToggle();
        yInvert.isOn = PlayerPrefs.GetInt("yToggle") == 1;
        SetYToggle();
    }

    public void SetXSensitivity(){
        float sense = xSlider.value;
        pov.m_HorizontalAxis.m_MaxSpeed = sense;
        PlayerPrefs.SetFloat("xSensitivity", sense);
    }

    public void SetYSensitivity(){
        float sense = ySlider.value;
        pov.m_VerticalAxis.m_MaxSpeed = sense;
        PlayerPrefs.SetFloat("ySensitivity", sense);
    }
    
    public void SetTimeScale(){
        float timer = timeSlider.value;
        Time.timeScale = timer;
        PlayerPrefs.SetFloat("timeScale", timer);
    }

    public void SetMusicVolume(){
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSfxVolume(){
        float volume = sfxSlider.value;
        myMixer.SetFloat("sfx", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    public void SetXToggle(){
        bool toggle = xInvert.isOn;
        pov.m_HorizontalAxis.m_InvertInput = toggle;
        PlayerPrefs.SetInt("xToggle", toggle ? 1 : 0);
    }

    public void SetYToggle(){
        bool toggle = yInvert.isOn;
        pov.m_VerticalAxis.m_InvertInput = toggle;
        PlayerPrefs.SetInt("yToggle", toggle ? 1 : 0);
    }
}
