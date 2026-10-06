using UnityEngine;

public class AnonymousLogin : MonoBehaviour
{
    private async void Start()
    {
        // Auth가 생성될 때까지 대기
        while(FirebaseBootstrap.Auth == null)
            await System.Threading.Tasks.Task.Yield();

        try
        {
            var result = await FirebaseBootstrap.Auth.SignInAnonymouslyAsync();
            Debug.Log($"Anonymous Login Success : {result.User.UserId}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Anonymous Login Failed : {ex.GetBaseException().Message}");
        }
    }
}