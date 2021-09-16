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

    public void AddPlayerDataItems()
    {
        gameClearFileManager = new GameClearFileManager();
        careGiverListSharingObject = GameObject.Find("CareGiverListSharingObjectManager").GetComponent<CareGiverListSharingObjectManager>();

        clearData = gameClearFileManager.LoadClearData();

        // 最初プレイヤーデータをdefaultobjectにセット
        InitPlayerClearData(clearData);

        Debug.Log("call AddPlayerDataItems()");
        Debug.Log("playerDataList.Count: " + clearData.clearPlayerDataList.Count);

        // プレイヤーデータが2つ以上ならオブジェクトを追加する
        if (clearData.clearPlayerDataList.Count > 1)
        {
            // プレイヤーデータindex1から繰り返す
            for (int i = 1; i < clearData.clearPlayerDataList.Count; i++)
            {
                // プレイヤーデータをセット
                GameObject copiedPlayerDataItem = Instantiate(careGiverListSharingObject.containerItem0GameObj);
                copiedPlayerDataItem.name = "item" + i;
                copiedPlayerDataItem.transform.SetParent(careGiverListSharingObject.playerClearScrollContainerGameObj.transform);
                copiedPlayerDataItem.transform.parent = careGiverListSharingObject.playerClearScrollContainerGameObj.transform;

                copiedPlayerDataItem.transform.Find("upperBox").transform.Find("nameBox").transform.Find("value").GetComponent<Text>().text = clearData.clearPlayerDataList[i].name;
                copiedPlayerDataItem.transform.Find("upperBox").transform.Find("moneyBox").transform.Find("value").GetComponent<Text>().text = clearData.clearPlayerDataList[i].money;
                copiedPlayerDataItem.transform.Find("upperBox").transform.Find("satisfactionBox").transform.Find("value").GetComponent<Text>().text = clearData.clearPlayerDataList[i].satisfaction.ToString();
                copiedPlayerDataItem.transform.Find("lowerBox").transform.Find("playTimeBox").transform.Find("value").GetComponent<Text>().text = TimeSpan.FromSeconds(clearData.clearPlayerDataList[i].playTime).ToString("hh':'mm':'ss");
                copiedPlayerDataItem.transform.Find("lowerBox").transform.Find("endDateBox").transform.Find("value").GetComponent<Text>().text = clearData.clearPlayerDataList[i].endDate;

                RectTransform rectCopiedPlayerDataItem = (RectTransform)copiedPlayerDataItem.transform;
                rectCopiedPlayerDataItem.anchoredPosition = new Vector2(0, 0);
            }
        }
        
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
