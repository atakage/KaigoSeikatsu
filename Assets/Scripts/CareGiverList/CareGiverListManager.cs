using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System;
using Newtonsoft.Json;

public class CareGiverListManager : MonoBehaviour
{
    public FirebaseManager firebaseManager;
    public bool successSelectingPlayerDataList; // DBからプレイヤーデータリスト取り出しに成功
    public bool actionFlagInUpdate; // Update()中である動作を指示するflag
    public CareGiverListSharingObjectManager careGiverListSharingObjectManager;
    public Dictionary<string, PlayerDataDBModel> allPlayerDataDBModelDic;

    private void Start()
    {
        careGiverListSharingObjectManager = GameObject.Find("CareGiverListSharingObjectManager").GetComponent<CareGiverListSharingObjectManager>();
        careGiverListSharingObjectManager.connectionFailDefaultGameObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(ClickConnectionRetryButton);

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

    public void ClickConnectionRetryButton()
    {
        // 現在プレイヤーデータリストから
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

        // DB接続チェックを成功すると
        if (connectionResult)
        {
            // DBからプレイヤーデータリストを取り出す(最初は10個取得、あと10個ずつ追加)
            string playerDataListJsonStr = await firebaseManager.SelectPlayerDataListByName(10);
            // プレイヤーデータリストの取り出しに失敗したら
            if (playerDataListJsonStr == null)
            {
                careGiverListSharingObjectManager.transparentScreenGameObj.SetActive(false);
                careGiverListSharingObjectManager.connectionFailDefaultGameObj.transform.SetSiblingIndex(careGiverListSharingObjectManager.careGiverListContentBoxGameObj.transform.childCount - 1);
                careGiverListSharingObjectManager.connectionFailDefaultGameObj.SetActive(true);
            }
            // プレイヤーデータリストの取り出しに成功すると
            else
            {
                allPlayerDataDBModelDic = JsonConvert.DeserializeObject<Dictionary<string, PlayerDataDBModel>>(playerDataListJsonStr);

                // UI設定
            }

        }
        // DB接続チェックができなかったら
        else
        {
            // UI設定
            careGiverListSharingObjectManager.transparentScreenGameObj.SetActive(false);
            careGiverListSharingObjectManager.connectionFailDefaultGameObj.transform.SetSiblingIndex(careGiverListSharingObjectManager.careGiverListContentBoxGameObj.transform.childCount-1);
            careGiverListSharingObjectManager.connectionFailDefaultGameObj.SetActive(true);
        }

    }
}
