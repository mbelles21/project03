using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace DmeoInfoGamerHubAssets
{
    public class DemoColorPicker : MonoBehaviour
    {
        public GameObject ChildColor;
        public void SetColor(Color newColor)
        {
            if(ChildColor.GetComponent<Image>() != null){
                ChildColor.GetComponent<Image>().color = newColor;
                return;
            }
            if(ChildColor.GetComponent<LineRenderer>() != null){
                ChildColor.GetComponent<LineRenderer>().sharedMaterial.color = newColor;
                return;
            }
            ChildColor.GetComponent<MeshRenderer>().material.color = newColor;
            //Debug.Log(newColor);
        }
    }
}