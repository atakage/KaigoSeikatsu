using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class CareGiverListManager : MonoBehaviour
{
    public FirebaseManager firebaseManager;
    public bool successSelectingPlayerDataList; // DBからプレイヤーデータリスト取り出しに成功
    public bool actionFlagInUpdate; // Update()中である動作を指示するflag

    private void Start()
    {
        actionFlagInUpdate = false;
        firebaseManager = new FirebaseManager();
        FireBaseConnection();
    }

    private void Update()
    {
        // DBからプレイヤーデータリストを取り出すとactionFlagInUpdate変更
        if (successSelectingPlayerDataList) actionFlagInUpdate = true;

        // プレイヤーデータリストの確保とactionFlagInUpdateがtrueなら
        if (successSelectingPlayerDataList && actionFlagInUpdate)
        {
            UnityEngine.Debug.Log("delete block screen");
            successSelectingPlayerDataList = false;
            actionFlagInUpdate = false;
        }

    }

    public async void FireBaseConnection()
    {
        // DB作業
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        bool connectionResult = false;
        // 最大5秒Firebaseに接続を試みる
        while (stopwatch.Elapsed < TimeSpan.FromMilliseconds(5000))
        {
            connectionResult = await firebaseManager.FireBaseConnection();
            if (connectionResult) break;
        }
        UnityEngine.Debug.Log("completed connectionResult: " + connectionResult);
        stopwatch = null;

        // DBからプレイヤーデータリストを取り出す

        // UI設定
    }
}
