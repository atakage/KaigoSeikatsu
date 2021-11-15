using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainEventManager : MonoBehaviour
{
    public MainEventSetManager mainEventSetManager;
    public PlayerSaveDataManager playerSaveDataManager;
    public BuildManager buildManager;

    private void Start()
    {
        buildManager = GameObject.Find("BuildManager").GetComponent("BuildManager") as BuildManager;
    }

    public int getAddingProgressFromMainEventJsonFile(string mainEventCode)
    {
        int addingProgress = 0;
        MainEventModel[] mainEventModelArray = mainEventSetManager.GetMainEventJsonFile();

        foreach(MainEventModel mainEventModel in mainEventModelArray)
        {
            if (mainEventModel.eventCode.Equals(mainEventCode))
            {
                addingProgress = mainEventModel.addingProgress;
                break;
            }
                
        }
        return addingProgress;
    }

    public bool CheckCompletedMainEvent(string mainEventCode)
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        PlayerData playerData = playerSaveDataManager.LoadPlayerData();

        bool completedEventBool = false;

        if(playerData.eventCodeObject != null 
            && playerData.eventCodeObject.completedMainEventArray != null && playerData.eventCodeObject.completedMainEventArray.Length > 0)
        {
            //　完了されたイベントリストからイベントを探す
            foreach (string eventCode in playerData.eventCodeObject.completedMainEventArray)
            {
                Debug.Log("player completed eventCode: " + eventCode);
                Debug.Log("mainEventCode: " + mainEventCode);
                if (eventCode.Equals(mainEventCode))
                {
                    completedEventBool = true;
                    break;
                }
            }
        }

        return completedEventBool;
    }

    public string findMainEvent(PlayerData playerdata)
    {
        string returnEventCode = null;

        // mainEvent.jsonからリストを読み込む
        mainEventSetManager = new MainEventSetManager();
        MainEventModel[] mainEventModelArray = mainEventSetManager.GetMainEventJsonFile();
        // 全体メインイベント数くらい繰り返す
        foreach (MainEventModel mainEventModel in mainEventModelArray)
        {
            Debug.Log("mainEventModel.eventCode: " + mainEventModel.eventCode);


            if(playerdata.eventCodeObject.completedMainEventArray != null) Debug.Log("Array.IndexOf(playerdata.eventCodeObject.completedMainEventArray, mainEventModel.eventCode): " + Array.IndexOf(playerdata.eventCodeObject.completedMainEventArray, mainEventModel.eventCode));
            if(playerdata.eventCodeObject.completedMainEventArray != null) Debug.Log("Array.IndexOf(playerdata.eventCodeObject.completedMainEventArray, mainEventModel.requiredCompletedMainEvent): " + Array.IndexOf(playerdata.eventCodeObject.completedMainEventArray, mainEventModel.requiredCompletedMainEvent));
            if (playerdata.eventCodeObject.completedJobEventArray != null) Debug.Log("playerdata.eventCodeObject.completedJobEventArray.Length: " + playerdata.eventCodeObject.completedJobEventArray.Length);
            if (playerdata.eventCodeObject.completedJobEventArray != null) Debug.Log("mainEventModel.requiredCompletedJobEvent.Split(':').Length: " + mainEventModel.requiredCompletedJobEvent.Split(':').Length);
            // すでにクリアしたイベントならcontinue
            if (playerdata.eventCodeObject.completedMainEventArray != null && Array.IndexOf(playerdata.eventCodeObject.completedMainEventArray, mainEventModel.eventCode) >= 0)
            {
                Debug.Log("すでにクリアしたメインイベント");
                continue;
            }   
            // メインイベントを発動させるために必要なcompletedMainEventを確認
            if ((playerdata.eventCodeObject.completedMainEventArray == null && !string.IsNullOrEmpty(mainEventModel.requiredCompletedMainEvent))
                ||
                playerdata.eventCodeObject.completedMainEventArray != null && Array.IndexOf(playerdata.eventCodeObject.completedMainEventArray, mainEventModel.requiredCompletedMainEvent) < 0)
            {
                Debug.Log("メインイベントを発動させるために必要なcompletedMainEventが不足");
                continue;
            }
            // メインイベントを発動させるために必要なcompletedJobEventの数を確認
            if ((playerdata.eventCodeObject.completedJobEventArray == null && !string.IsNullOrEmpty(mainEventModel.requiredCompletedJobEvent))
                ||
                (playerdata.eventCodeObject.completedJobEventArray != null && playerdata.eventCodeObject.completedJobEventArray.Length < mainEventModel.requiredCompletedJobEvent.Split(':').Length))
            {
                Debug.Log("メインイベントを発動させるために必要なcompletedJobEventの数が不足");
                continue;
            }

            // プレイヤーデータと条件を比べる
            if (playerdata.progress < mainEventModel.requiredProgress) continue;
            if (playerdata.satisfaction < mainEventModel.requiredSatisfaction) continue;
            if (!playerdata.currentScene.Equals(mainEventModel.requiredScene)) continue;

            // メインイベント発動させるためのすべての条件を確認完了
            returnEventCode = mainEventModel.eventCode;
            Debug.Log("発動するメインイベントコードに追加: " + returnEventCode);
            break;
        }
        Debug.Log("returnEventCode: " + returnEventCode);
        return returnEventCode;
    }

    public string PickUpOneMainEvent(List<MainEventModel> mainEventModelList)
    {
        MainEventModel minMainEvent = mainEventModelList[0];
        // requiredProgressが一番低いイベントを返還する
        for(int i=0; i<mainEventModelList.Count; i++)
        {
            if (minMainEvent.requiredProgress > mainEventModelList[i].requiredProgress)
            {
                minMainEvent = mainEventModelList[i];
            }
        }
        Debug.Log("minimum progress mainEventCode: " + minMainEvent.eventCode);
        return minMainEvent.eventCode;
    }
}
