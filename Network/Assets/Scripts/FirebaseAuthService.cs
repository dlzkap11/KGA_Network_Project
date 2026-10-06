using System;
using UnityEngine;
using Firebase;
using Firebase.Auth;

//[FirebaseAuthService 설계도 — 방송하는 쪽]

//정적 이벤트: OnAuthChanged(user) — 상태가 바뀔 때마다 방송

//Start:  
//  이미 로그인된 사용자가 있으면(자동 세션 복원) → 그 사용자로 즉시 방송

//공통 가드 절 TryGetAuth:  
//  Auth가 아직 준비 안 됐으면 → 안내 문구 표시 후 false 반환  
//  준비됐으면 → true 반환

//OnClickSignUp / OnClickSignIn / OnClickAnonymousSignIn (모두 async void, 최상위):  
//  가드 절을 통과하지 못하면 종료  
//  try:  
//    해당 Async 함수를 기다린다  
//    안내 문구를 비운다  
//    OnAuthChanged를 방송한다(결과의 User)  
//  catch:  
//    안내 문구에 실패 메시지를 표시한다

//OnClickSignOut (async void가 아니다 — 왜일까요?):  
//  가드 절을 통과하지 못하면 종료  
//  SignOut을 호출한다  
//  안내 문구를 비운다  
//  OnAuthChanged를 방송한다(null)

public class FirebaseAuthService : MonoBehaviour
{

    public event Action<FirebaseUser> OnAuthChanged;
    private FirebaseAuth Auth => FirebaseBootstrap.Auth;

    private void Start()
    {
        OnAuthChanged?.Invoke(FirebaseBootstrap.Auth.CurrentUser);
    }


    public void OnClickSighOut()
    {
    }




}
