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
        // set player ammo for tutorial
        Inventory.FlashbangAmmo = 1;
        Inventory.TaserAmmo = 1;
        Inventory inventory = FindAnyObjectByType<Inventory>();
        inventory.UpdateUIText();
    }

    // function will be called by player (OnTriggerEnter)
    public void DisplayTutorial(int i)
    {
        if(prevTutorial != null) {
            prevTutorial.SetActive(false);
        }

        // if index exists show the next tutorial
        if(i >= 0 && i < tutorials.Count) {
            tutorials[i].SetActive(true);
            prevTutorial = tutorials[i];
        }
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
