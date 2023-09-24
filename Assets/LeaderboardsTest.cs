using System.Collections.Generic;
using Core.Server;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using Sirenix.OdinInspector;

public class LeaderboardsTest : MonoBehaviour
{
    public string playerName;
    public int score;
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
    
    DatabaseReference databaseReference;

    private void Awake()
    {
        User.SignIn(() => enabled = true);
    }

    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference.Child("players");
    }

    [Button]
    public void AddScore()
    {
        DatabaseReference playerRef = databaseReference.Child(playerName);
        playerRef.Child("score").SetValueAsync(score);
    }

    [Button]
    void FetchLeaderboardData()
    {
        entries.Clear();
        Query query = databaseReference.OrderByChild("score");

        query.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            DataSnapshot snapshot = task.Result;
                
            foreach (DataSnapshot playerSnapshot in snapshot.Children)
            {
                string playerId = playerSnapshot.Key;
                int score = int.Parse(playerSnapshot.Child("score").Value.ToString());
                entries.Add(new LeaderboardEntry(playerId, score));
                
                Debug.Log("Player ID: " + playerId + ", Score: " + score);
            }
        });
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;

    public LeaderboardEntry(string name, int score)
    {
        playerName = name;
        this.score = score;
    }
}
