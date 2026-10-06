using UnityEngine;
using Firebase;
using Firebase.Auth;

public class FirebaseBootstrap : MonoBehaviour
{
    public static FirebaseAuth Auth {get; private set;}

    private async void Awake()
    {
        var status = await FirebaseApp.CheckAndFixDependenciesAsync();

        if(status == DependencyStatus.Available)
        {
            Auth = FirebaseAuth.DefaultInstance;
            Debug.Log("Firebase is ready");
        }
        else
        {
            Debug.LogError($"Firebase is not ready : {status}");
        }
    }
}
