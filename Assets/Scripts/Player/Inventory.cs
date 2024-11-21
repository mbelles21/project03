using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public TextMeshProUGUI grenadeCountText;

    public static int GrenadeCount;
    public int ammo;
    
    // Start is called before the first frame update
    void Start()
    {
        if(GrenadeCount == 0) {
            GrenadeCount = ammo; // so only resets if empty on grenades (TODO: change this later)
        }
        UpdateUIText(); // make sure text displays correct num on start
    }

    public void UpdateUIText()
    {
        grenadeCountText.text = "Grenades: " + GrenadeCount;
    }
}
