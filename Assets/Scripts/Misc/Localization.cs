using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Localization : MonoBehaviour
{
    private bool active = false;
    public void Start(){
        ChangeLocale(PlayerPrefs.GetInt("language"));
    }
    public void ChangeLocale(int localeID){
        if(active){
            return;
        }
        PlayerPrefs.SetInt("language", localeID);
        StartCoroutine(SetLocale(localeID));
    }
    IEnumerator SetLocale(int _localeID){
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        active = false;
    }
}
