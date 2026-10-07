using Firebase.Database;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class LeaderboardView : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private GameObject entryPrefab;
    private int topCount = 10;

    private Query scoresRef;
    private List<GameObject> spawnedEntries = new();

    private void Awake()
    {
        Query q = FirebaseDatabase.DefaultInstance.RootReference.Child("scores").OrderByChild("score");
        if(topCount > 0)
        {
            q = q.LimitToLast(topCount);
        }

        scoresRef = q;
    }

    private void OnEnable()
    {
        if (scoresRef == null)
            return;

        scoresRef.ValueChanged += OnScoresChanged;

    }

    private void OnDisable()
    {
        if (scoresRef == null)
            return;

        scoresRef.ValueChanged -= OnScoresChanged;
    }

    private void OnScoresChanged(object sender, ValueChangedEventArgs args)
    {
        if(args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Rebuild(args.Snapshot);
    }

    private void Rebuild(DataSnapshot snap)
    {
        // 기존 항목을 전부 삭제
        foreach(GameObject go in spawnedEntries)
        {
            Destroy(go);
        }
        spawnedEntries.Clear();
        // 데이터가 없으면 return
        if (!snap.Exists)
            return;

        var children = new List<DataSnapshot>(snap.Children);
        children.Reverse();

        // 자식수만큼 프리팹만들어서 이름 : 점수로 표기
        for(int i = 0; i < children.Count; i++)
        {
            DataSnapshot child = children[i];
            GameObject entry = Instantiate(entryPrefab, contentRoot);
            entry.GetComponent<TMP_Text>().text = $"{i + 1}.{child.Child("name").Value} : {child.Child("score").Value}";
            spawnedEntries.Add(entry);
        }
    }
}
