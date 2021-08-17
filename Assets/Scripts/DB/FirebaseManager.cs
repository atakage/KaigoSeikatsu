using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;

public class FirebaseManager : MonoBehaviour
{
    public DatabaseReference databaseReference;
    public void FireBaseConnection()
    {
        try
        {
            // FirebaseDatabase.DefaultInstance.GetReference("player_data"): DB名にaccess
            this.databaseReference = FirebaseDatabase.DefaultInstance.GetReference("player_data");
        }
        catch
        {

        }
    }

    public void FindDataToDB(PlayerDataDBModel playerDataDBModel)
    {
        try
        {
            databaseReference.Child(playerDataDBModel.name).ValueChanged += HandleValueChanged;
        }
        catch
        {

        }
    }

    public void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log("args.Snapshot.Key: " + args.Snapshot.Key);
        // DBにデータががないならempty
        Debug.Log("args.Snapshot.GetRawJsonValue(): " + args.Snapshot.GetRawJsonValue());
    }

    public void InsertUpdateToDB(PlayerDataDBModel playerDataDBModel)
    {
        try
        {
            databaseReference.Child(playerDataDBModel.name).SetRawJsonValueAsync(JsonUtility.ToJson(playerDataDBModel))
                .ContinueWith(task => {
                    if (task.IsFaulted)
                    {
                        Debug.Log("insert update fail: " + task.Exception);
                    }
                    else
                    {
                        Debug.Log("insert update success");
                    }
                });
        }
        catch
        {

        }
    }
}
