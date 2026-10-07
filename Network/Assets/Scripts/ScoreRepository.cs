using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System;
using System.Collections.Generic;
using TMPro;

public class ScoreRepository : MonoBehaviour
{
    [SerializeField] private string nickname;
    [SerializeField] private int score;
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Slider scoreSlider;

    private DatabaseReference root;

    private void Awake()
    {
        root = FirebaseDatabase.DefaultInstance.RootReference;
    }

    [ContextMenu("1. 점수 저장")]
    public void Save()
    {
        var user = FirebaseBootstrap.Auth?.CurrentUser;
        if(user == null)
        {
            Debug.LogWarning("로그인 해야함");
            return;
        }
        
        SaveScore(user.UserId,nameField.text,(int)scoreSlider.value);
    }
    [ContextMenu("2. 점수 읽기")]
    private void Load()
    {
        var user = FirebaseBootstrap.Auth?.CurrentUser;
        if(user == null)
        {
            Debug.LogWarning("로그인 해야함");
            return;
        }
        LoadScore(user.UserId);
    }
    [ContextMenu("3. 점수 제거")]
    private void Delete()
    {
        var user = FirebaseBootstrap.Auth?.CurrentUser;
        if(user == null)
        {
            Debug.LogWarning("로그인 해야함");
            return;
        }

        DeleteScore(user.UserId);
    }

    public async void SaveScore(string uid,string nickname,int score)
    {
        if(string.IsNullOrEmpty(uid))
        {
            Debug.LogWarning("uid가 비어있습니다.");
            return;
        }
        try
        {
            await root.Child("scores").Child(uid).Child("name").SetValueAsync(nickname);
            await root.Child("scores").Child(uid).Child("score").SetValueAsync(score);

            Debug.Log($"저장 완료 : scores/{uid}");
        }
        catch(Exception ex)
        {
            Debug.LogError($"저장 실패 : {ex.Message}");
        }
    }

    public async void LoadScore(string uid)
    {
        if(string.IsNullOrEmpty(uid))
        {
            Debug.LogWarning("uid가 비어있음");
            return;
        }

        try
        {
            DataSnapshot snap = await root.Child("scores").Child(uid).GetValueAsync();

            if(!snap.Exists)
            {
                Debug.Log("저장된 데이터 없음");
                return;
            }

            Debug.Log($"{snap.Child("name").Value} : {snap.Child("score").Value}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"읽기 실패 : {ex.Message}");
        }
    }

    public async void DeleteScore(string uid)
    {
        if(string.IsNullOrEmpty(uid))
        {
            Debug.LogWarning("uid가 비어있음");
            return;
        }

        try
        {
            await root.Child("scores").Child(uid).RemoveValueAsync();
            Debug.Log("삭제 완료");
        }
        catch( Exception ex)
        {
            Debug.LogError($"삭제 실패 : {ex.Message}");
        }
    }

    public async void UpdateScoreOnly(string uid, int newScore)
    {
        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogWarning("uid가 비어있음");
            return;
        }
        try
        {
            var update = new Dictionary<string, object>
            {
                {$"scores/{uid}/score",newScore },
                {$"scores/{uid}/updateAt",System.DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
            };
            Debug.Log("점수 갱신");

            await root.UpdateChildrenAsync(update);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }
    [ContextMenu("5. 점수 갱신")]
    private void UpdateOnly()
    {
        var user = FirebaseBootstrap.Auth?.CurrentUser;
        if (user == null)
        {
            Debug.LogWarning("로그인 해야함");
            return;
        }

        UpdateScoreOnly(user.UserId, score);

    }

    [ContextMenu("4. 덮어쓰기")]
    private void Break()
    {
        var user = FirebaseBootstrap.Auth?.CurrentUser;

        BreakScoreNode(user.UserId, 500);
    }


    public async void BreakScoreNode(string uid,int newScore)
    {
        try
        {
            await root.Child("scores").Child(uid).SetValueAsync(new Dictionary<string, object> { { "score", newScore } });
            Debug.Log("덮어쓰기 완료");
        }
        catch
        {

        }
    }
}
