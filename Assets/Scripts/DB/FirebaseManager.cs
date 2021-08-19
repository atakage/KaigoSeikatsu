using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using UnityEngine;
using Firebase;
using Firebase.Database;

public class FirebaseManager : MonoBehaviour
{
    public DatabaseReference databaseReference;
    public Dictionary<bool, string> findDataDBResultDic; // key: DB通信結果

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

    // awaitが使われているならmethodにasyncをかけてreturnTypeをTaskに
    public async Task<string> FindDataToDB(PlayerDataDBModel playerDataDBModel)
    {
        string returnValue = "サーバーとの通信に失敗しました";
        //databaseReference.Child(playerDataDBModel.name).ValueChanged += HandleValueChanged;

        
         await databaseReference.Child(playerDataDBModel.name).GetValueAsync()
            .ContinueWith( task =>
            {
                if (task.IsFaulted)
                {
                    //findDataDBResultDic.Add(false, "サーバーとの通信に失敗しました");
                    Debug.Log("task.Exception.Message" + task.Exception.Message);
                    returnValue = "サーバーとの通信に失敗しました";
                }
                else
                {
                    DataSnapshot dataSnapshot = task.Result;
                    // DBにない名前データなら
                    if (string.IsNullOrEmpty(dataSnapshot.GetRawJsonValue()))
                    {
                        Debug.Log("non data");
                        returnValue = null;
                    }
                    // すでにDBにある名前データなら
                    else
                    {
                        Debug.Log("data find");
                        returnValue = "すでに使用されている名前です";
                    }
                }
            });

        return returnValue;
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
