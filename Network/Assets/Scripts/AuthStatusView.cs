using Firebase.Auth;
using TMPro;
using UnityEngine;

//[AuthStatusView 설계도 — 구독하는 쪽]

//필드: statusText

//OnEnable: OnAuthChanged 이벤트에 HandleAuthChanged를 구독한다 (+=)  
//OnDisable: 구독을 해제한다(-=) — 반드시 짝으로

//Start:  
//  구독 전에 이미 로그인돼 있던 경우(자동 세션 복원)를 위해  
//  현재 사용자를 한 번 읽어와 HandleAuthChanged에 직접 전달한다

//HandleAuthChanged(user):  
//  statusText가 없으면 종료  
//  user가 null이면 → "로그아웃 상태"  
//  아니면 → "로그인 중: " + user의 UID

public class AuthStatusView : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText;

    private void OnEnable()
    {
        AuthUI.OnAuthChanged += HandleAuthChanged;
    }

    private void OnDisable()
    {
        AuthUI.OnAuthChanged -= HandleAuthChanged;
    }

    private void Start()
    {
        // 구독 전 로그인 세션 복원
        if(FirebaseBootstrap.Auth != null && FirebaseBootstrap.Auth.CurrentUser != null)
            HandleAuthChanged(FirebaseBootstrap.Auth.CurrentUser);
    }

    private void HandleAuthChanged(FirebaseUser user)
    {

        if (statusText == null)
            return;

        if (user == null)
        {
            statusText.text = "Logout!";
        }
        else
            statusText.text = $"Login : {user.UserId}";
    }
}
