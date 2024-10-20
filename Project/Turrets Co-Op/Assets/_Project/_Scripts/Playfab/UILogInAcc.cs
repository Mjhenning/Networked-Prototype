using UnityEngine;
using UnityEngine.UI;

public class UILoginAcc : MonoBehaviour {               //Manager for Login UI
    [SerializeField] Text errorTxt;
    [SerializeField] Canvas canvas;

    string username, password, emailAddress;

    void OnEnable () {
        UserAccManager.OnLoginFailed.AddListener (OnLoginFailed);
        UserAccManager.OnLoginSuccess.AddListener (OnLogInSuccess);
    }

    void OnDisable () {
        UserAccManager.OnLoginFailed.RemoveListener (OnLoginFailed);
        UserAccManager.OnLoginSuccess.RemoveListener (OnLogInSuccess);
    }

    void OnLoginFailed (string error) {
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

    public void SignIn () {
        UserAccManager.instance.SignIn (username, password);
    }
}