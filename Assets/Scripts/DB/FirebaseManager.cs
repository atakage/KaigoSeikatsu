using System;
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

    public async Task<bool>  FireBaseConnection(PlayerDataDBModel playerDataDBModel)
    {
        bool returnValue = false;
        DatabaseReference connectedRef = FirebaseDatabase.DefaultInstance.GetReference(".info/connected");

        await Task.Run(() =>
        {
            connectedRef.ValueChanged += (object sender, ValueChangedEventArgs a) => {

                bool isConnected = (bool)a.Snapshot.Value;
                if (isConnected)
                {
                    Debug.Log("isConnected: " + isConnected);
                    // 接続に成功するとDB情報を格納
                    // FirebaseDatabase.DefaultInstance.GetReference("player_data"): DB名にaccess
                    this.databaseReference = FirebaseDatabase.DefaultInstance.GetReference("player_data");
                    returnValue = true;
                }
                else
                {
                    Debug.Log("isConnected: " + isConnected);
                    returnValue = false;
                }
            };
        });

        return returnValue;
    }

    public bool HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        bool returnValue = false;

        bool isConnected =  (bool)args.Snapshot.Value;
        if (isConnected)
        {
            // FirebaseDatabase.DefaultInstance.GetReference("player_data"): DB名にaccess
            this.databaseReference = FirebaseDatabase.DefaultInstance.GetReference("player_data");
            returnValue = true;
        }
        else
        {
            returnValue = false;
        }
        print("isConnected" + isConnected);
        return returnValue;
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
