using Firebase;
using Firebase.Auth;
using System;
using TMPro;
using UnityEngine;

public class AuthUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_Text statusText;
    private FirebaseAuth Auth => FirebaseBootstrap.Auth;
    public static event Action<FirebaseUser> OnAuthChanged;

    private async void Start()
    {
        while (FirebaseBootstrap.Auth == null)
            await System.Threading.Tasks.Task.Yield();

        if (FirebaseBootstrap.Auth != null)
            OnAuthChanged?.Invoke(FirebaseBootstrap.Auth.CurrentUser);
    }

    public async void OnClickSignUp()
    {
        if (!Ready())
            return;

        try
        {
            var result = await Auth.CreateUserWithEmailAndPasswordAsync(emailInput.text, passwordInput.text);
            Show($"Sign Up Success: {result.User.Email} \nUID : {result.User.UserId}");
            OnAuthChanged?.Invoke(result.User);
        }
        catch(System.Exception ex)
        {
            ShowError("Sigh Up Failed", ex);
        }

    }

    public async void OnClickSignIn()
    {
        if (!Ready())
            return;

        try
        {
            var result = await Auth.SignInWithEmailAndPasswordAsync(emailInput.text, passwordInput.text);
            Show($"Sign In Success: {result.User.Email} \nUID : {result.User.UserId}");
            OnAuthChanged?.Invoke(result.User);
        }
        catch (System.Exception ex)
        {
            ShowError("Sign In Failed", ex);
        }

    }

    public async void OnClickAnonymousSignIn()
    {
        if (!Ready())
            return;

        try
        {
            var result = await FirebaseBootstrap.Auth.SignInAnonymouslyAsync();
            Show($"Anonymous Login Success : {result.User.UserId}");
            OnAuthChanged?.Invoke(result.User);
        }
        catch (System.Exception ex)
        {
            ShowError($"Anonymous Login Failed", ex);
        }

    }

    public void OnClickSignOut()
    {
        if (!Ready())
            return;
        Auth.SignOut();
        OnAuthChanged?.Invoke(null);
        Show($"Sign Out Success");
    }


    //현재 준비가 되었는지 -> 이전 단계의 작업이 끝났는지 확인한다.
    private bool Ready()
    {
        if(Auth == null)
        {
            Show("Not Ready");
            return false;
        }
        return true;
    }

    // 현재 상태를 보여준다.
    private void Show(string msg)
    {
        if (msg == null)
            return;
        statusText.text = msg;
        Debug.Log(msg);
    }

    // 오류 메시지를 보여준다.
    private void ShowError(string label, System.Exception ex)
    {
        var firebaseError = ex.GetBaseException() as FirebaseException;

        string errorCode = "";
        if(firebaseError != null)
        {
            errorCode = ((AuthError)firebaseError.ErrorCode).ToString();
        }
        else
        {
            errorCode = "Not Firebase Error";
        }

        Show($"{label} Error Code : {errorCode}");
        
        Debug.LogWarning($"{label} 실패 | 코드 : {errorCode} | 메시지 : {ex.Message}");
    }

}
