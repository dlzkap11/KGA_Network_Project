using Firebase.Database;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardView2 : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private GameObject entryPrefab;
    private DatabaseReference scoreRef;
    private List<GameObject> spawnedEntries = new();

    private void Awake()
    {
        scoreRef = FirebaseDatabase.DefaultInstance.RootReference.Child("scores");
    }

    private void OnEnable()
    {
        if (scoreRef != null)
            return;

        scoreRef.ValueChanged += OnScoreChanged;
    }

    private void OnDisable()
    {
        if (scoreRef != null)
            return;

        scoreRef.ValueChanged -= OnScoreChanged;
    }

    private void OnScoreChanged(object sender, ValueChangedEventArgs args)
    {
        if(args.DatabaseError != null)
        {
            Debug.LogError($"Changed Error : {args.DatabaseError.Message}");
            return;
        }

        Rebuild(args.Snapshot);

    }

    private void Rebuild(DataSnapshot snap)
    {
        foreach(GameObject go in spawnedEntries)
        {
            Destroy(go); 
        }
        spawnedEntries.Clear();
        if (!snap.Exists)
            return;

        foreach(DataSnapshot child in snap.Children)
        {
            GameObject entry = Instantiate(entryPrefab, contentRoot);
            entry.GetComponent<TMP_Text>().text = $"{child.Child("name").Value} : {child.Child("score").Value}";
            spawnedEntries.Add(entry);
        }
    }

    [ContextMenu("상위 3위")]
    public async void OnClickLoadTop3()
    {
        if (scoreRef == null)
            return;

        try
        {
            DataSnapshot snap = await scoreRef.OrderByChild("score").LimitToLast(3).GetValueAsync();
            if (!snap.Exists)
                return;

            var  lines = new List<string>();
            foreach(DataSnapshot child in snap.Children)
            {
                lines.Add($"{child.Child("name").Value} : {child.Child("score").Value}");
            }
            lines.Reverse();

            for(int i = 0; i < lines.Count; i++)
            {
                lines[i] = $"{i + 1}. : {lines[i]}";
            }
            Debug.Log(string.Join("\n", lines));

        }
        catch (System.Exception ex)
        {
            Debug.LogError($"LoadTop Failed : {ex.Message}");
        }
        


    }
}
