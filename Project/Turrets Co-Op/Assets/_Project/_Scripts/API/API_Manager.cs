using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class API_Manager : MonoBehaviour
{
    public static API_Manager instance;

    void Awake()
    {
        instance = this;
    }
    
    public void UserNameTest(string email,int amountToGen) //example function
    {

        UsernameGen(email, amountToGen, (selectedUsername) =>
        {
            if (selectedUsername != null)
            {
                Debug.Log("Received Random Username: " + selectedUsername);
            }
            else
            {
                Debug.Log("No username was selected.");
            }
        });
    }

    public void UsernameGen(string email, int amount, Action<string> onUsernameSelected) //used to format retrieved usernames and to select a random one
    {
        CallUsernameGenerator(email, amount, (usernames) => {
            if (usernames != null && usernames.Length > 0)
            {
                // Log all generated usernames
                Debug.Log("Generated Usernames:");
                foreach (var username in usernames)
                {
                    Debug.Log(username);
                }

                // Select a random username from the array
                string randomUsername = usernames[Random.Range(0, usernames.Length)];
                Debug.Log("Randomly Selected Username: " + randomUsername);
            
                // Invoke the callback with the random username
                onUsernameSelected?.Invoke(randomUsername);
            }
            else
            {
                Debug.Log("Failed to retrieve usernames.");
                onUsernameSelected?.Invoke(null); // Optionally return null or handle error
            }
        });
    }

    public void CallUsernameGenerator(string email, int count, Action<string[]> callback) //calls UsernamGeneratorAPI and retrieves json return in correct format
    {
        StartCoroutine(UsernameGeneratorAPI.Grab(email, count, callback));
    }
}