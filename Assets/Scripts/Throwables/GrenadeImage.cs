using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GrenadeImage : MonoBehaviour
{
    public List<Sprite> spriteArray = new List<Sprite>();
    private int currentIndex = 0;
    private Image imageComponent;

    public void Start(){
        imageComponent = GetComponent<Image>();
        imageComponent.sprite = spriteArray[currentIndex];
    }

    public void Swap(InputAction.CallbackContext context){
        if(context.started){
            currentIndex = (currentIndex + 1) % spriteArray.Count;
            imageComponent.sprite = spriteArray[currentIndex];
        }
    }
}
