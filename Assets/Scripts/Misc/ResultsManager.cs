using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultsManager : MonoBehaviour
{
    public GameObject resultsUI;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI rankText;

    [Header("Rank Times (seconds)")]
    public float sTime;
    public float aTime;
    public float bTime;
    public float cTime;
    public float dTime;
    public float fTime;
    
    public void GetResults(float clearTime)
    {
        float mins = Mathf.FloorToInt(clearTime/60);
        float secs = Mathf.FloorToInt(clearTime%60);
        timeText.text = "Clear Time: " + string.Format("{0:0}:{1:00}", mins, secs);

        string rank = GetRank(clearTime);
        rankText.text = "Rank: " + rank;
        
        resultsUI.SetActive(true);
    }

    string GetRank(float clearTime)
    {
        if(clearTime < sTime) {
            return "S";
        }
        else if(clearTime < aTime) {
            return "A";
        }
        else if(clearTime < bTime) {
            return "B";
        }
        else if(clearTime < cTime) {
            return "C";
        }
        else if(clearTime < dTime) {
            return "D";
        }
        else if(clearTime < fTime) {
            return "F";
        }
        else {
            Debug.Log("error getting rank");
            return "--";
        }
    }
}
