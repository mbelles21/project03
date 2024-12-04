using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public TextMeshProUGUI grenadeCountText;

    public static int FlashbangAmmo;
    public static int TaserAmmo;
    public static int GrenadeType; // should match with grenade list in PlayerMovement
    
    // Start is called before the first frame update
    void Start()
    {
        GrenadeType = 0;
        UpdateUIText(); // make sure text displays correct num on start
    }

    public void UpdateUIText()
    {
        switch(GrenadeType)
        {
            case 0:
                // flashbang
                grenadeCountText.text = "Flash: " + FlashbangAmmo;
                break;

            case 1:
                // taser
                grenadeCountText.text = "Taser: " + TaserAmmo;
                break;

            default:
                // in this case there was a problem assiging the type or it hasn't been added yet
                Debug.Log("GrenadeType index not assigned correctly");
                break;
        }
        
    }

    public void ThrowGrenade()
    {
        switch(GrenadeType)
        {
            case 0:
                FlashbangAmmo--;
                break;
            case 1:
                TaserAmmo--;
                break;
            default:
                Debug.Log("error with throwing GrenadeType");
                break;
        }
        UpdateUIText();
    }

    // used for aiming logic in PlayerMovement
    public int ReturnCurrentAmmo()
    {
        switch(GrenadeType)
        {
            case 0:
                return FlashbangAmmo;
            case 1:
                return TaserAmmo;
            default:
                Debug.Log("could not get ammo count");
                return 0;
        }
    }
}
