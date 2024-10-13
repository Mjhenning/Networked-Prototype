using System;
using Mirror;
using UnityEngine;
using UnityEngine.UI;


public class Crosshair : MonoBehaviour {
    [SerializeField]Image chImg;


    [Client]
    public void SetColor(Color newColor) { //changes crosshair color
        chImg.color = newColor;
    }

    [Client]
    public void UpdateFill (float amount) { //changes crosshair fill amount
        chImg.fillAmount = amount;
    }
    
    
    
}
