﻿using System.Collections;
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
    public bool startingCheckScrollPos;

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

        UnityEngine.Debug.Log("position scroll"+careGiverListSharingObjectManager.careGiverListScrollViewGameObj.GetComponent<ScrollRect>().verticalNormalizedPosition);
        // プレイヤーデータリストのスクロールが一番下についたら
        //if (careGiverListSharingObjectManager.careGiverListScrollViewGameObj.GetComponent<ScrollRect>().verticalNormalizedPosition == 0)
        if (startingCheckScrollPos && 
            careGiverListSharingObjectManager.careGiverListScrollViewGameObj.GetComponent<ScrollRect>().verticalNormalizedPosition <= 0.05f)
        {
            careGiverListSharingObjectManager.dataReadingMsgGameObj.SetActive(true);
            // DBからリストを取り出す(現在childCount+7個)


            // スクロールの位置調整
            UnityEngine.Debug.Log("ENDED SCROLL");
            careGiverListSharingObjectManager.careGiverListScrollViewGameObj.GetComponent<ScrollRect>().verticalNormalizedPosition = 0.35f;
        }
            

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
            string playerDataListJsonStr = await firebaseManager.SelectPlayerDataListByName(7);
            // プレイヤーデータリストの取り出しに失敗したら
            if (playerDataListJsonStr == null)
            {
                // UI設定
                careGiverListSharingObjectManager.transparentScreenGameObj.SetActive(false);
                careGiverListSharingObjectManager.connectionFailDefaultGameObj.transform.SetSiblingIndex(careGiverListSharingObjectManager.careGiverListContentBoxGameObj.transform.childCount - 1);
                careGiverListSharingObjectManager.connectionFailDefaultGameObj.SetActive(true);
            }
            // プレイヤーデータリストの取り出しに成功すると
            else
            {
                careGiverListSharingObjectManager.transparentScreenGameObj.SetActive(false);

                allPlayerDataDBModelDic = JsonConvert.DeserializeObject<Dictionary<string, PlayerDataDBModel>>(playerDataListJsonStr);
                // UI設定
                // Dictionaryにデータがあると
                if(allPlayerDataDBModelDic.Count > 0)
                {      
                    int dataCount = 1;

                    // sizeだけ繰り返す
                    foreach (KeyValuePair<string, PlayerDataDBModel> playerDataKey in allPlayerDataDBModelDic)
                    {
                        // プレイヤーデータが8以上ならgridLayoutのcellsizeを伸ばす
                        if (dataCount > 7)
                        {
                            Vector2 defaultViewportCellSize = careGiverListSharingObjectManager.careGiverListViewportGameObj.GetComponent<GridLayoutGroup>().cellSize;
                            careGiverListSharingObjectManager.careGiverListViewportGameObj.GetComponent<GridLayoutGroup>().cellSize = new Vector2(defaultViewportCellSize.x, defaultViewportCellSize.y + 100);
                        }
                       
                        GameObject copiedDefaultField = Instantiate(careGiverListSharingObjectManager.defaultFieldsGameObj);
                        copiedDefaultField.AddComponent<Outline>();

                        Destroy(copiedDefaultField.transform.Find("nameButton").gameObject);
                        Destroy(copiedDefaultField.transform.Find("moneyButton").gameObject);
                        Destroy(copiedDefaultField.transform.Find("satisfactionButton").gameObject);
                        Destroy(copiedDefaultField.transform.Find("playTimeButton").gameObject);
                        Destroy(copiedDefaultField.transform.Find("endDateButton").gameObject);

                        GameObject nameValue = new GameObject();
                        nameValue.name = "nameValue";
                        nameValue.AddComponent<Text>();
                        nameValue.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                        nameValue.GetComponent<Text>().text = playerDataKey.Value.name;
                        nameValue.GetComponent<Text>().fontSize = 20;
                        nameValue.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                        nameValue.GetComponent<Text>().color = new Color(0,0,0,255);
                        nameValue.transform.SetParent(copiedDefaultField.transform);

                        GameObject moneyValue = new GameObject();
                        moneyValue.name = "moneyValue";
                        moneyValue.AddComponent<Text>();
                        moneyValue.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                        moneyValue.GetComponent<Text>().text = playerDataKey.Value.money;
                        moneyValue.GetComponent<Text>().fontSize = 20;
                        moneyValue.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                        moneyValue.GetComponent<Text>().color = new Color(0, 0, 0, 255);
                        moneyValue.transform.SetParent(copiedDefaultField.transform);

                        GameObject satisfactionValue = new GameObject();
                        satisfactionValue.name = "satisfactionValue";
                        satisfactionValue.AddComponent<Text>();
                        satisfactionValue.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                        satisfactionValue.GetComponent<Text>().text = playerDataKey.Value.satisfaction.ToString();
                        satisfactionValue.GetComponent<Text>().fontSize = 20;
                        satisfactionValue.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                        satisfactionValue.GetComponent<Text>().color = new Color(0, 0, 0, 255);
                        satisfactionValue.transform.SetParent(copiedDefaultField.transform);

                        GameObject playTimeValue = new GameObject();
                        playTimeValue.name = "playTimeValue";
                        playTimeValue.AddComponent<Text>();
                        playTimeValue.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                        playTimeValue.GetComponent<Text>().text = TimeSpan.FromSeconds(playerDataKey.Value.playTime).ToString("hh':'mm':'ss");
                        playTimeValue.GetComponent<Text>().fontSize = 20;
                        playTimeValue.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                        playTimeValue.GetComponent<Text>().color = new Color(0, 0, 0, 255);
                        playTimeValue.transform.SetParent(copiedDefaultField.transform);

                        GameObject endDateValue = new GameObject();
                        endDateValue.name = "endDateValue";
                        endDateValue.AddComponent<Text>();
                        endDateValue.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                        endDateValue.GetComponent<Text>().text = playerDataKey.Value.endDate;
                        endDateValue.GetComponent<Text>().fontSize = 20;
                        endDateValue.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                        endDateValue.GetComponent<Text>().color = new Color(0, 0, 0, 255);
                        endDateValue.transform.SetParent(copiedDefaultField.transform);

                        copiedDefaultField.transform.SetParent(careGiverListSharingObjectManager.careGiverListContentBoxGameObj.transform);

                        ++dataCount;
                    }
                    
                    // scroll update用
                    Vector2 viewportCellSize = careGiverListSharingObjectManager.careGiverListViewportGameObj.GetComponent<GridLayoutGroup>().cellSize;
                    careGiverListSharingObjectManager.careGiverListViewportGameObj.GetComponent<GridLayoutGroup>().cellSize = new Vector2(viewportCellSize.x, viewportCellSize.y + 100);

                    careGiverListSharingObjectManager.dataReadingMsgGameObj.transform.SetSiblingIndex(careGiverListSharingObjectManager.careGiverListContentBoxGameObj.transform.childCount-1);

                    /*
                    GameObject scrollUpdateAlert = Instantiate(careGiverListSharingObjectManager.defaultFieldsGameObj);
                    scrollUpdateAlert.GetComponent<GridLayoutGroup>().cellSize = new Vector2(645, 100);
                    scrollUpdateAlert.AddComponent<Outline>();
                    scrollUpdateAlert.name = "scrollUpdateAlert";

                    Destroy(scrollUpdateAlert.transform.Find("nameButton").gameObject);
                    Destroy(scrollUpdateAlert.transform.Find("moneyButton").gameObject);
                    Destroy(scrollUpdateAlert.transform.Find("satisfactionButton").gameObject);
                    Destroy(scrollUpdateAlert.transform.Find("playTimeButton").gameObject);
                    Destroy(scrollUpdateAlert.transform.Find("endDateButton").gameObject);

                    GameObject serverConnectionMsgValue = new GameObject();
                    serverConnectionMsgValue.name = "serverConnectionMsgValue";
                    serverConnectionMsgValue.AddComponent<Text>();
                    serverConnectionMsgValue.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    serverConnectionMsgValue.GetComponent<Text>().text = "データの読み込み中...";
                    serverConnectionMsgValue.GetComponent<Text>().fontSize = 20;
                    serverConnectionMsgValue.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                    serverConnectionMsgValue.GetComponent<Text>().color = new Color(0, 0, 0, 255);
                    serverConnectionMsgValue.transform.SetParent(scrollUpdateAlert.transform);
                    
                    scrollUpdateAlert.transform.SetParent(careGiverListSharingObjectManager.careGiverListContentBoxGameObj.transform);
                    */
                }

                
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

        // リスト更新がすべて終わったあとからスクロールの位置をチェック
        startingCheckScrollPos = true;
    }
}
