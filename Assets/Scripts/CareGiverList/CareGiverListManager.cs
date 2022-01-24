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
    public SceneTransitionManager sceneTransitionManager;
    public bool successSelectingPlayerDataList; // DBからプレイヤーデータリスト取り出しに成功
    public bool actionFlagInUpdate; // Update()中である動作を指示するflag
    public CareGiverListSharingObjectManager careGiverListSharingObjectManager;
    public Dictionary<string, PlayerDataDBModel> allPlayerDataDBModelDic;
    public bool startingCheckScrollPos;
    public int reqPlayerDataCount;
    public string connectionFailDefault = "connectionFailDefault"; // objectName
    public string dataReadingMessage = "dataReadingMessage"; // objectName
    public string defaultFields = "defaultFields"; // objectName
    public bool completedReadingData; // 追加リストを読み込んだあとスクロール位置を調整するため
    public bool readingDataBool;

    private void Start()
    {
        sceneTransitionManager = new SceneTransitionManager();

        careGiverListSharingObjectManager = GameObject.Find("CareGiverListSharingObjectManager").GetComponent<CareGiverListSharingObjectManager>();
        careGiverListSharingObjectManager.connectionFailDefaultGameObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(ClickConnectionRetryButton);
        careGiverListSharingObjectManager.returnButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickReturnButton);

        reqPlayerDataCount = 7;

        actionFlagInUpdate = false;
        firebaseManager = new FirebaseManager();
        FireBaseConnectionAndSelectPlayerDataList();
    }

    private void Update()
    {
        // リスト読み込みが完了されてないなら
        if (!startingCheckScrollPos)
        {

        }

        // 追加リストを読み込むと
        if (completedReadingData)
        {
            completedReadingData = false;
            // スクロール位置を調整
            careGiverListSharingObjectManager.careGiverListScrollViewGameObj.GetComponent<ScrollRect>().verticalNormalizedPosition = 0.25f;
        }

        if (startingCheckScrollPos &&
            !readingDataBool &&
            careGiverListSharingObjectManager.careGiverListScrollViewGameObj.GetComponent<ScrollRect>().verticalNormalizedPosition <= 0.05f)
        {
            readingDataBool = true;

            // drag機能を防ぐ(バグ防止)
            careGiverListSharingObjectManager.careGiverListScrollViewGameObj.GetComponent<ScrollRect>().vertical = false;

            //careGiverListSharingObjectManager.dataReadingMsgGameObj.SetActive(true);
            // DBからリストを取り出す(現在childCount+7個)
            AdditionalPlayerDataList();

            
            UnityEngine.Debug.Log("ENDED SCROLL");

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

    public void ClickReturnButton()
    {
        sceneTransitionManager.LoadTo("TitleScene");
    }

    public async void AdditionalPlayerDataList()
    {
        careGiverListSharingObjectManager.transparentScreenGameObj.SetActive(true);

        // DB作業
        /*
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
        */
        bool connectionResult = true;
        // DB接続チェックを成功すると
        if (connectionResult)
        {
            // リストを追加で読み込むときlimitCountを増加させてくれる(3ずつ)
            string playerDataListJsonStr = await firebaseManager.SelectPlayerDataListByName(reqPlayerDataCount+=7);

            // プレイヤーデータリストの取り出しに失敗したら
            if (playerDataListJsonStr == null)
            {
                // reset count
                reqPlayerDataCount -= 3;
                // UI設定
                //careGiverListSharingObjectManager.dataReadingMsgGameObj.SetActive(false);
                // drag機能をいかす
                careGiverListSharingObjectManager.careGiverListScrollViewGameObj.GetComponent<ScrollRect>().vertical = true;

            }
            // プレイヤーデータリストの取り出しに成功すると
            else
            {
                // UI設定
                //careGiverListSharingObjectManager.dataReadingMsgGameObj.SetActive(false);
                allPlayerDataDBModelDic = JsonConvert.DeserializeObject<Dictionary<string, PlayerDataDBModel>>(playerDataListJsonStr);

                // Dictionaryにデータがあると
                if (allPlayerDataDBModelDic.Count > 0)
                {
                   
                    List<GameObject> beingDestroyedObjectList = new List<GameObject>();

                    // contentBox内のdefaultObject(connectionFailDefault, dataReadingMessage, defaultFields)を除いた既存プレイヤーデータを削除
                    foreach (Transform contentBoxChildTransform in careGiverListSharingObjectManager.careGiverListContentBoxGameObj.transform)
                    {
                        UnityEngine.Debug.Log("name DestroyImmediate: " + contentBoxChildTransform.name);

                        if (!connectionFailDefault.Equals(contentBoxChildTransform.gameObject.name)
                        && !dataReadingMessage.Equals(contentBoxChildTransform.gameObject.name)
                        && !defaultFields.Equals(contentBoxChildTransform.gameObject.name))
                        {
                            //UnityEngine.Debug.Log("DestroyImmediate: " + contentBoxChildTransform.transform.Find("nameValue").GetComponent<Text>().text);
                            //DestroyImmediate(contentBoxChildTransform.gameObject);
                            // ★loop中childObjectを削除するとindex接近に問題が発生するためリストに貯めておいてあとから削除する
                            beingDestroyedObjectList.Add(contentBoxChildTransform.gameObject);
                        }
                    }

                    // 削除するオブジェクトがあると
                    foreach (GameObject beingDestroyedObject in beingDestroyedObjectList)
                    {
                        DestroyImmediate(beingDestroyedObject);
                    }

                    // sizeだけ繰り返す
                    foreach (KeyValuePair<string, PlayerDataDBModel> playerDataKey in allPlayerDataDBModelDic)
                    {
                        GameObject copiedDefaultField = Instantiate(careGiverListSharingObjectManager.defaultFieldsGameObj);
                        copiedDefaultField.AddComponent<Outline>();
                        copiedDefaultField.name = playerDataKey.Value.name;
                        copiedDefaultField.GetComponent<Image>().color = new Color32(255, 191, 193, 255);

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
                        nameValue.GetComponent<Text>().color = new Color(0, 0, 0, 255);
                        nameValue.transform.SetParent(copiedDefaultField.transform);

                        GameObject moneyValue = new GameObject();
                        moneyValue.name = "moneyValue";
                        moneyValue.AddComponent<Text>();
                        moneyValue.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                        // 2022.01.25 修正
                        moneyValue.GetComponent<Text>().text = string.IsNullOrEmpty(playerDataKey.Value.money) ? "0" + "円" : playerDataKey.Value.money + "円";
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
                        // 2022.01.25 修正
                        endDateValue.GetComponent<Text>().text = string.IsNullOrEmpty(playerDataKey.Value.endDate) ? "" : DateTime.ParseExact(playerDataKey.Value.endDate, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd");
                        endDateValue.GetComponent<Text>().fontSize = 20;
                        endDateValue.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                        endDateValue.GetComponent<Text>().color = new Color(0, 0, 0, 255);
                        endDateValue.transform.SetParent(copiedDefaultField.transform);

                        copiedDefaultField.transform.SetParent(careGiverListSharingObjectManager.careGiverListContentBoxGameObj.transform);
                    }

                    // scroll update用
                    Vector2 viewportCellSize = careGiverListSharingObjectManager.careGiverListViewportGameObj.GetComponent<GridLayoutGroup>().cellSize;
                    viewportCellSize = new Vector2(viewportCellSize.x, 0);
                    // 2021.10.25 追加
                    // cellSizeが最小限の高さを維持できるようにする
                    if (1300.005 < viewportCellSize.y + (100 * (careGiverListSharingObjectManager.careGiverListContentBoxGameObj.transform.childCount - 1)))
                    {
                        careGiverListSharingObjectManager.careGiverListViewportGameObj.GetComponent<GridLayoutGroup>().cellSize = new Vector2(viewportCellSize.x, viewportCellSize.y + (100 * (careGiverListSharingObjectManager.careGiverListContentBoxGameObj.transform.childCount - 1)));
                    }
                    

                }
                // Dictionaryにデータがないと
                else
                {
                    UnityEngine.Debug.Log("Dictionary Data Count 0!");
                    // UI設定
                    //careGiverListSharingObjectManager.dataReadingMsgGameObj.SetActive(false);
                    // drag機能をいかす
                    //careGiverListSharingObjectManager.careGiverListScrollViewGameObj.GetComponent<ScrollRect>().vertical = true;
                }
                // drag機能をいかす
                careGiverListSharingObjectManager.careGiverListScrollViewGameObj.GetComponent<ScrollRect>().vertical = true;
                // 追加リスト作業が終わったことを示す
                completedReadingData = true;
            }
            // サーバー通信画面false
            careGiverListSharingObjectManager.transparentScreenGameObj.SetActive(false);
            readingDataBool = false;
        }
        // DB接続チェックができなかったら
        else
        {
            UnityEngine.Debug.Log("DB Connection Fail");
            // UI設定
            //careGiverListSharingObjectManager.dataReadingMsgGameObj.SetActive(false);
            // drag機能をいかす
            careGiverListSharingObjectManager.careGiverListScrollViewGameObj.GetComponent<ScrollRect>().vertical = true;
            // サーバー通信画面false
            careGiverListSharingObjectManager.transparentScreenGameObj.SetActive(false);
            readingDataBool = false;
            // 追加リスト作業が終わったことを示す
            completedReadingData = true;
        }

        UnityEngine.Debug.Log("reqPlayerDataCount: " + reqPlayerDataCount);
        UnityEngine.Debug.Log("careGiverListContentBoxGameObj.transform.childCount: " + careGiverListSharingObjectManager.careGiverListContentBoxGameObj.transform.childCount);
        UnityEngine.Debug.Log("careGiverListContentBoxGameObj.transform.childCount-3: " + (careGiverListSharingObjectManager.careGiverListContentBoxGameObj.transform.childCount-3));
        UnityEngine.Debug.Log("startingCheckScrollPos: " + startingCheckScrollPos);

        // サーバーからすべてのデータを読み込んだとき
        // 要請したプレイヤーデータカウントより読み込んだリストのカウント(contentBox内のdefaultObject(connectionFailDefault, dataReadingMessage, defaultFields)を除いた)が小さくと
        if (connectionResult && reqPlayerDataCount > (careGiverListSharingObjectManager.careGiverListContentBoxGameObj.transform.childCount-3))
        {
            UnityEngine.Debug.Log("Reading Complete!");
            // UI設定
            //careGiverListSharingObjectManager.dataReadingMsgGameObj.SetActive(false);

            // viewPortの余る空間を除く
            //Vector2 viewportCellSize = careGiverListSharingObjectManager.careGiverListViewportGameObj.GetComponent<GridLayoutGroup>().cellSize;
            // viewportのcellSize.yが800.0052(最低限)を超えると
            //if (viewportCellSize.y > 800.0052) careGiverListSharingObjectManager.careGiverListViewportGameObj.GetComponent<GridLayoutGroup>().cellSize = new Vector2(viewportCellSize.x, viewportCellSize.y - 100);

            // リスト更新機能を防ぐ
            startingCheckScrollPos = false;
            // リスト読み込み完了表示
            careGiverListSharingObjectManager.dataReadingMsgGameObj.transform.SetSiblingIndex(careGiverListSharingObjectManager.careGiverListContentBoxGameObj.transform.childCount - 1);
            careGiverListSharingObjectManager.dataReadingMsgGameObj.SetActive(true);
        }
        // スクロールの位置調整
        //if (startingCheckScrollPos) careGiverListSharingObjectManager.careGiverListScrollViewGameObj.GetComponent<ScrollRect>().verticalNormalizedPosition = 0.4f;

        // drag機能をいかす
        careGiverListSharingObjectManager.careGiverListScrollViewGameObj.GetComponent<ScrollRect>().vertical = true;
    }

    public void ClickConnectionRetryButton()
    {
        careGiverListSharingObjectManager.connectionFailDefaultGameObj.SetActive(false);
        careGiverListSharingObjectManager.transparentScreenGameObj.SetActive(true);

        FireBaseConnectionAndSelectPlayerDataList();
    }

    public async void FireBaseConnectionAndSelectPlayerDataList()
    {
        // DB作業
        // 2021.11.17 修正
        // firebase DB Connection Checkによる様々な問題発生を防止するためチェック作業を除く
        /*
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
        */

        bool connectionResult = true;
        // DB接続チェックを成功すると
        if (connectionResult)
        {
            // DBからプレイヤーデータリストを取り出す(最初は10個取得、あと7個ずつ追加)
            string playerDataListJsonStr = await firebaseManager.SelectPlayerDataListByName(reqPlayerDataCount);
            // プレイヤーデータリストの取り出しに失敗したら
            if (playerDataListJsonStr == null)
            {
                // UI設定
                careGiverListSharingObjectManager.transparentScreenGameObj.SetActive(false);
                careGiverListSharingObjectManager.dataNoneMessage.transform.SetSiblingIndex(careGiverListSharingObjectManager.careGiverListContentBoxGameObj.transform.childCount - 1);
                careGiverListSharingObjectManager.dataNoneMessage.SetActive(true);

            // 2021.11.17 追加
            // taskがtimeOutなら
            }else if (playerDataListJsonStr.Equals("timeOut"))
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
                        copiedDefaultField.name = playerDataKey.Value.name;
                        copiedDefaultField.GetComponent<Image>().color = new Color32(255, 191, 193, 255);

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
                        // 2022.01.25 修正
                        moneyValue.GetComponent<Text>().text = string.IsNullOrEmpty(playerDataKey.Value.money) ? "0" + "円" : playerDataKey.Value.money + "円";
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
                        // 2022.01.25 修正
                        endDateValue.GetComponent<Text>().text = string.IsNullOrEmpty(playerDataKey.Value.endDate) ? "" : DateTime.ParseExact(playerDataKey.Value.endDate, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd");
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

                }
                // リスト更新機能ON
                startingCheckScrollPos = true;
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
        // startingCheckScrollPos = true;
    }
}
