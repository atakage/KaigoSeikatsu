﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitSettingManager : MonoBehaviour
{
    public CSVManager csvManager;
    public PlayerSaveDataManager playerSaveDataManager;
    public SceneTransitionManager sceneTransitionManager;
    public GameObject canvasObj;
    public GameObject loadButtonObj;
    public GameObject newGameAlertBoxObj;
    public PlayerData playerData;
    public int msgCheckIntVal;
    void Start()
    {
        csvManager = new CSVManager();
        playerSaveDataManager = new PlayerSaveDataManager();
        sceneTransitionManager = new SceneTransitionManager();

        canvasObj = GameObject.Find("Canvas");
        newGameAlertBoxObj = canvasObj.transform.Find("newGameAlertBox").gameObject;
        msgCheckIntVal = 0;

        // コンビニで販売するアイテムをセットする(最初)
        // ConvenienceItemInit.txtにある情報をResource/saveData/ConvenienceItem.jsonに移す
        // itemSaleを変更したいときはそのSceneでConvenienceItem.jsonを読み込んで変更したあとセーブすればいい
        // ConvenienceItem.jsonがあると作らない(最初だけ作る)
        csvManager.ReadConvenienceInitFileAndCreateJson();

        // カフェで販売するアイテムをセットする
        csvManager.ReadCafeItemInitFileAndCreateJson();

        // MainEvent.jsonを作る
        csvManager.ReadMainEventInitFileAndCreateJson();
        
        PlayerData playerData = playerSaveDataManager.LoadPlayerData();

        canvasObj.transform.Find("PlayButton").GetComponent<Button>().onClick.AddListener(() => ClickPlayButton(playerData));
        newGameAlertBoxObj.transform.Find("confirmButton").GetComponent<Button>().onClick.AddListener(() => ClickNewGameAlertBoxConfirmBtn(this.msgCheckIntVal));
        newGameAlertBoxObj.transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(() => ClickNewGameAlertBoxCancelBtn());

        // プレイヤーデータがある場合ロードボタンを表示する
        if (playerData != null)
        {
            loadButtonObj = canvasObj.transform.Find("loadButton").gameObject;
            loadButtonObj.GetComponent<Button>().interactable = true;
            loadButtonObj.GetComponent<Button>().onClick.AddListener(() => ClickLoadButton(playerData.currentScene));
        }

        
    }

    public void ClickNewGameAlertBoxConfirmBtn(int msgCheckIntVal)
    {
        Debug.Log("msgCheckIntVal: " + msgCheckIntVal);

        // 追加メッセージ
        if (msgCheckIntVal == 0)
        {
            string msg = "ゲームを始めますか?";
            newGameAlertBoxObj.transform.Find("message").GetComponent<Text>().text = msg;
            this.msgCheckIntVal = 1;
        }
        // ゲームが始まるnew game
        else if (msgCheckIntVal == 1)
        {
            // プレイヤーアイテムデータ初期化
            playerSaveDataManager.RemoveItemListDataJsonFile();
            ItemListData[] itemListData = new ItemListData[1];
            itemListData[0] = new ItemListData();
            itemListData[0].itemName = "名刺";
            itemListData[0].itemDescription = "介護福祉士の名刺だ";
            itemListData[0].quantity = 1;
            itemListData[0].keyItem = "Y";
            playerSaveDataManager.SaveItemListData(itemListData);


            // 新しいプレイヤーデータを作成
            playerData = new PlayerData();
            playerData.money = "15000"; // 円
            playerData.time = "08:00";
            playerData.progress = 0;
            playerData.fatigue = 0;
            playerData.currentScene = "IntroScene";
            playerSaveDataManager.SavePlayerData(playerData);

            sceneTransitionManager.LoadTo("IntroScene");
        }

    }

    public void ClickNewGameAlertBoxCancelBtn()
    {
        newGameAlertBoxObj.SetActive(false);
        string msg = "新しいデータが作成されると\n現在セーブデータは削除されます";
        newGameAlertBoxObj.transform.Find("message").GetComponent<Text>().text = msg;
        this.msgCheckIntVal = 0;
    }

    public void ClickPlayButton(PlayerData playerData)
    {
        // プレイヤーデータがあるとメッセージを表示する
        if (playerData != null)
        {
            newGameAlertBoxObj.SetActive(true);
        }
        // プレイヤーデータがないとnew game
        else
        {
            // プレイヤーアイテムデータ初期化
            playerSaveDataManager.RemoveItemListDataJsonFile();
            ItemListData[] itemListData = new ItemListData[1];
            itemListData[0] = new ItemListData();
            itemListData[0].itemName = "名刺";
            itemListData[0].itemDescription = "介護福祉士の名刺だ";
            itemListData[0].quantity = 1;
            itemListData[0].keyItem = "Y";
            playerSaveDataManager.SaveItemListData(itemListData);

            // 新しいプレイヤーデータを作成
            playerData = new PlayerData();
            playerData.money = "15000"; // 円
            playerData.time = "08:00";
            playerData.progress = 0;
            playerData.fatigue = 0;
            playerData.currentScene = "IntroScene";
            playerSaveDataManager.SavePlayerData(playerData);

            sceneTransitionManager.LoadTo("IntroScene");
        }

    }

    public void ClickLoadButton(string currentScene)
    {
        sceneTransitionManager.LoadTo(currentScene);
    }
}
