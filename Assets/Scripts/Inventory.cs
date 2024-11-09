using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static int GrenadeCount;
    public int ammo;
    
    // Start is called before the first frame update
    void Start()
    {
        GrenadeCount = ammo;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
