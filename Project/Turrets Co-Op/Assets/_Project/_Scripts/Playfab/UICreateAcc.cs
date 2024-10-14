using System;
using UnityEngine;
using UnityEngine.UI;

public class UICreateAcc : MonoBehaviour {

    [SerializeField] Text errorTxt;
    
    [SerializeField] Canvas canvas;
    
    string username, password, emailAddress;

    void OnEnable () {
        UserAccManager.OnCreateAccountFailed.AddListener (OnCreateAccountFailed);
        UserAccManager.OnLoginSuccess.AddListener (OnLogInSuccess);
    }
    
    void OnDisable () {
        UserAccManager.OnCreateAccountFailed.RemoveListener (OnCreateAccountFailed);
        UserAccManager.OnLoginSuccess.RemoveListener (OnLogInSuccess);
    }

    void OnCreateAccountFailed (string error) {
        errorTxt.gameObject.SetActive (true);
        errorTxt.text = error;
    }
    
    void OnLogInSuccess () {
        canvas.enabled = false;
    }

    public void UpdateUsername (string _username) {
        username = _username;
    }

    public void UpdatePassword (string _password) {
        password = _password;
    }
    
    public void UpdateEmailAddress (string _emailAddress) {
        emailAddress = _emailAddress;
    }

    public void CreateAccount () {
        UserAccManager.instance.CreatAccount (username, emailAddress, password);
    }
    
}
