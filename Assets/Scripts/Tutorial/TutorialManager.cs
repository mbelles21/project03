using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public List<GameObject> tutorials;
    private GameObject prevTutorial;
    public static bool TutorialCompleted; // might want to change/use for future logic

    // Start is called before the first frame update
    void Start()
    {
        // TutorialCompleted = false;
    }

    // function will be called by player (OnTriggerEnter)
    public void DisplayTutorial(int i)
    {
        if(prevTutorial != null) {
            prevTutorial.SetActive(false);
        }
        tutorials[i].SetActive(true);
        prevTutorial = tutorials[i];
    }

    public void EndTutorial()
    {
        Debug.Log("tutorial completed!");
        TutorialCompleted = true;
        PlayerPrefs.SetInt("tutorial", 1); // store TutorialCompleted value
        PlayerPrefs.Save();

        SceneManager.LoadScene("DungeonGeneration");
    }
}
