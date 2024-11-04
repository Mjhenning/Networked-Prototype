using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq; // Make sure to include this for JSON parsing

public static class UsernameGeneratorAPI
{
    const string API_URL = "https://api.apiverve.com/v1/usernamegenerator";

    public static IEnumerator Grab(string email, int count, System.Action<string[]> callback) //used to access json and format correctly  //limited to 550 tokens (so about 550 new users)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{API_URL}?email={email}&count={count}")) 
        {
            request.SetRequestHeader("x-api-key", "8665ebb2-5a69-42f8-867a-cad28fbbd330"); // Replace with your actual API key
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Parse the JSON response
                var jsonResponse = JObject.Parse(request.downloadHandler.text);
                Debug.Log(request.downloadHandler.text); // Log the response for debugging
            
                // Check if the status is "ok"
                if (jsonResponse["status"]?.Value<string>() == "ok")
                {
                    var usernames = jsonResponse["data"]?["suggestions"]?.ToObject<string[]>();
                    if (usernames != null && usernames.Length > 0)
                    {
                        callback?.Invoke(usernames);
                    }
                    else
                    {
                        Debug.LogError("No usernames found in the response.");
                        callback?.Invoke(null);
                    }
                }
                else
                {
                    Debug.LogError("Failed to generate usernames: " + jsonResponse["error"]?.ToString());
                    callback?.Invoke(null);
                }
            }
            else
            {
                Debug.LogError($"Error: {request.error}");
                callback?.Invoke(null);
            }
        }
    }

    
}