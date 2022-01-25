using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;
using UnityEngine;
using Firebase;
using Firebase.Database;

public class FirebaseManager : MonoBehaviour
{
    public DatabaseReference databaseReference;
    public string realDbName = "player_data";
    public string testDbName = "test_player_data";
    public string dbName;
    public BuildManager buildManager;

    // 2022.01.25 追加
    public FirebaseManager(bool realMode)
    {
        if (realMode) this.dbName = this.realDbName;
        else this.dbName = this.testDbName;
        Debug.Log("dbName In FirebaseManager Instance: " + dbName);
    }

    public void Awake()
    {
        buildManager = GameObject.Find("BuildManager").GetComponent("BuildManager") as BuildManager;
        if(buildManager.realMode) this.dbName = this.realDbName;
        else this.dbName = this.testDbName;
        Debug.Log("dbName In FirebaseManager  Awake(): " + dbName);
    }

    public async Task<string> SelectPlayerDataListByName(int selectCount)
    {
        Debug.Log("SelectPlayerDataListByName selectCount: " + selectCount);

        string returnValue = null;

        // 日本語の場合データを読み込んだ時にunicodeに変換される問題(たまにある)

        try
        {
           await FirebaseDatabase.DefaultInstance.GetReference(dbName).OrderByKey().LimitToFirst(selectCount).GetValueAsync()
           //await databaseReference.OrderByKey().LimitToFirst(selectCount).GetValueAsync()        
           //await databaseReference.LimitToFirst(selectCount).GetValueAsync()
           //await databaseReference.OrderByKey().GetValueAsync()
           .ContinueWith(task =>
           {
               if (task.IsFaulted)
               {
                   //findDataDBResultDic.Add(false, "サーバーとの通信に失敗しました");
                   Debug.Log("task.Exception.Message" + task.Exception.Message);
               }
               else if (task.IsCompleted)
               {
                   DataSnapshot dataSnapshot = task.Result;
                   Debug.Log("dataSnapshot.GetRawJsonValue(): " + dataSnapshot.GetRawJsonValue());
                   returnValue = dataSnapshot.GetRawJsonValue();
               }
               // new CancellationTokenSource(5000).Token: Taskが無限に待機するのを防止する、5秒後Exception
           }, new CancellationTokenSource(5000).Token);
        }
        catch(TaskCanceledException e)
        {
            return returnValue="timeOut";
        }

        

        return returnValue;
    }

    public async Task<bool>  FireBaseConnection()
    {
        bool returnValue = false;
        DatabaseReference connectedRef = FirebaseDatabase.DefaultInstance.GetReference(".info/connected");
        connectedRef.KeepSynced(true);

        await Task.Run(() =>
        {
            connectedRef.ValueChanged += (object sender, ValueChangedEventArgs a) => {

                bool isConnected = (bool)a.Snapshot.Value;
                if (isConnected)
                {
                    Debug.Log("isConnected: " + isConnected);
                    // 接続に成功するとDB情報を格納
                    // FirebaseDatabase.DefaultInstance.GetReference("player_data"): DB名にaccess
                     this.databaseReference = FirebaseDatabase.DefaultInstance.GetReference(dbName);
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
            this.databaseReference = FirebaseDatabase.DefaultInstance.GetReference(dbName);
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

        try
        {
            //await databaseReference.Child(playerDataDBModel.name).GetValueAsync()
            await FirebaseDatabase.DefaultInstance.GetReference(dbName).Child(playerDataDBModel.name).GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    //findDataDBResultDic.Add(false, "サーバーとの通信に失敗しました");
                    Debug.Log("task.Exception.Message: " + task.Exception);
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
            }, new CancellationTokenSource(5000).Token);
        }
        catch(TaskCanceledException e)
        {
            return returnValue = "timeOut";
        }

        return returnValue;
    }

    public async Task<string> InsertUpdateToDB(PlayerDataDBModel playerDataDBModel)
    {
        string returnValue = "データ作成に失敗しました";

        try
        {
            //await databaseReference.Child(playerDataDBModel.name).SetRawJsonValueAsync(JsonUtility.ToJson(playerDataDBModel))
            await FirebaseDatabase.DefaultInstance.GetReference(dbName).Child(playerDataDBModel.name).SetRawJsonValueAsync(JsonUtility.ToJson(playerDataDBModel))
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("insert update fail: " + task.Exception);
                    returnValue = task.Exception.Message;
                }
                else
                {
                    Debug.Log("insert update success");
                    returnValue = "success";
                }
            }, new CancellationTokenSource(3000).Token);
        }
        catch (TaskCanceledException e)
        {
            return returnValue;
        }
;
        return returnValue;
    }


    // 既存データをアプデするにはいいだが新しいデータをこのようにtransactionで入れようとすれば問題がある
    // 1. firebaseデータのchild名を変更不可能(child名が0から自動で指定される)
    /*
    public async Task<string> InsertUpdateTransaction(PlayerDataDBModel playerDataDBModel)
    {
        string returnValue = "データ作成に失敗しました";

        await databaseReference.RunTransaction(mutableData => {
            List<object> playerDataListFromDB = mutableData.Value as List<object>;

            bool alreadyName = false;

            if (playerDataListFromDB == null)
            {
                playerDataListFromDB = new List<object>();
            }
            else
            {
                foreach (var playerDataDBModelFromDB in playerDataListFromDB)
                {
                    if (((Dictionary<string, object>)playerDataDBModelFromDB)["name"].Equals(playerDataDBModel.name))
                    {
                        alreadyName = true;
                        break;
                    }
                }
            }

            if (alreadyName) return TransactionResult.Abort();
            else
            {
                //Dictionary<string, Dictionary<string, object>> castedNewPlayerData = new Dictionary<string, Dictionary<string, object>>();
                Dictionary<string, object> newPlayerData = new Dictionary<string, object>();

                newPlayerData["name"] = playerDataDBModel.name;
                newPlayerData["localMode"] = playerDataDBModel.localMode;
                newPlayerData["startDate"] = playerDataDBModel.startDate;

                //castedNewPlayerData[playerDataDBModel.name] = newPlayerData;

                playerDataListFromDB.Add(newPlayerData);
                mutableData.Value = playerDataListFromDB;

                return TransactionResult.Success(mutableData);
            }
        }).ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log("insert update fail: " + task.Exception);
                returnValue = task.Exception.Message;
            }
            else
            {
                Debug.Log("insert update success");
                returnValue = "success";
            }
        });
        return returnValue;
    }
    */
}
