using Mirror;
using TMPro;
using UnityEngine;

public class PlayerScore : MonoBehaviour {
    [SerializeField] TMP_Text scoreText;

    [Client]
    public void SetScore (int amount) { //changes score
        scoreText.text = amount.ToString();
    }
    
    [Client]
    public void SetColor(Color newColor) { //changes score color
        scoreText.color = newColor;
    }
}
