using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CareGiverListUIManager : MonoBehaviour
{
    public ClearData clearData;
    public GameClearFileManager gameClearFileManager;
    public CareGiverListSharingObjectManager careGiverListSharingObject;

    private void Start()
    {
        gameClearFileManager = new GameClearFileManager();
        careGiverListSharingObject = GameObject.Find("CareGiverListSharingObjectManager").GetComponent<CareGiverListSharingObjectManager>();

        clearData = gameClearFileManager.LoadClearData();

        // 最初プレイヤーデータをdefaultobjectにセット
        InitPlayerClearData(clearData);

        // プレイヤーデータが2つ以上ならオブジェクトを追加する
    }

    public void InitPlayerClearData(ClearData clearData)
    {
        // 一番目のデータを取り出す
        PlayerData playerDataFirst = clearData.clearPlayerDataList[0];

        // default objectに格納する
        careGiverListSharingObject.containerItem0GameObj.transform.Find("upperBox").transform.Find("nameBox").transform.Find("value").GetComponent<Text>().text = playerDataFirst.name;
        careGiverListSharingObject.containerItem0GameObj.transform.Find("upperBox").transform.Find("moneyBox").transform.Find("value").GetComponent<Text>().text = playerDataFirst.money;
        careGiverListSharingObject.containerItem0GameObj.transform.Find("upperBox").transform.Find("satisfactionBox").transform.Find("value").GetComponent<Text>().text = playerDataFirst.satisfaction.ToString();
        careGiverListSharingObject.containerItem0GameObj.transform.Find("lowerBox").transform.Find("playTimeBox").transform.Find("value").GetComponent<Text>().text = TimeSpan.FromSeconds(playerDataFirst.playTime).ToString("hh':'mm':'ss");
        careGiverListSharingObject.containerItem0GameObj.transform.Find("lowerBox").transform.Find("endDateBox").transform.Find("value").GetComponent<Text>().text = playerDataFirst.endDate;
    }
}
