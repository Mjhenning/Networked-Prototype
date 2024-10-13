using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {
    public static HealthManager instance;
    
    [SerializeField]List<GameObject> hearts = new List<GameObject>();


    void Awake () {
        instance = this;
    }

    public void ChangeColor (Color playercolor) { //changes client side heart color
        foreach (GameObject _heart in hearts) {
            _heart.GetComponent<Image> ().color = playercolor;
        }
    }

    public void RemoveAHeart () { //removes last hearts on list
        hearts[hearts.Count - 1].SetActive (false);
        hearts.Remove (hearts[hearts.Count -1]);
    }
}
