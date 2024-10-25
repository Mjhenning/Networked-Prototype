using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {
    public static HealthManager instance;
    
    [SerializeField]List<GameObject> hearts;

    [SerializeField]Transform heartsParent;


    void Awake () {
        instance = this;
        ResetHearts ();
    }

    [Client]
    public void ChangeHeartsColor (Color playercolor) { //changes client side heart color
        foreach (GameObject _heart in hearts) {
            _heart.GetComponent<Image> ().color = playercolor;
        }
    }

    [Client]
    public void RemoveAHeart() {
        hearts[^1].SetActive (false);
        hearts.Remove (hearts[^1]);
        Debug.Log ("Removed a heart");
    }

    [Client]
    public void ResetHearts() {

        hearts.Clear ();
        
        foreach (Transform _child in heartsParent) {
            GameObject _gameObject;
            (_gameObject = _child.gameObject).SetActive (true);
            hearts.Add (_gameObject);
            Debug.Log ("added a heart");
        }
    }
}
