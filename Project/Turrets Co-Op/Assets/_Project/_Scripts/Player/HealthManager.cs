using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {
    public static HealthManager instance;
    
    [SerializeField]List<GameObject> hearts = new List<GameObject>();
    [SerializeField] Transform heartsParent;


    void Awake () {
        instance = this;
    }

    [Client]
    public void ChangeColor (Color playercolor) { //changes client side heart color
        foreach (GameObject _heart in hearts) {
            _heart.GetComponent<Image> ().color = playercolor;
        }
    }

    [Client]
    public void RemoveAHeart () { //removes last hearts on list
        hearts[^1].SetActive (false);
        hearts.Remove (hearts[^1]);
    }
    [Client]

    public void ResetHearts () {
        foreach (Transform _child in heartsParent) {
            hearts.Add (_child.gameObject);
            _child.gameObject.SetActive (true);
        }
    }
}
