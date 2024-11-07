using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(API_Manager))]
public class API_ManagerEditor : Editor
{
    private string email = "sean@mail.com"; // Default email
    private int amount = 10; // Default amount

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Input fields for parameters
        email = EditorGUILayout.TextField("Email", email);
        amount = EditorGUILayout.IntField("Amount", amount);

        // Button to call the TestUsernameGen method
        if (GUILayout.Button("Generate Usernames"))
        {
            API_Manager apiManager = (API_Manager)target;
            apiManager.UserNameTest(email, amount);
        }
    }
}