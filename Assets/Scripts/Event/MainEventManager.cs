using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainEventManager : MonoBehaviour
{
    public MainEventSetManager mainEventSetManager;


    public string findMainEvent(PlayerData playerdata)
    {
        List<MainEventModel> mainEventModelList = new List<MainEventModel>();

        // mainEvent.jsonからリストを読み込む
        mainEventSetManager = new MainEventSetManager();
        MainEventModel[] mainEventModelArray = mainEventSetManager.GetMainEventJsonFile();
        // 全体メインイベント数くらい繰り返す
        foreach (MainEventModel mainEventModel in mainEventModelArray)
        {
            // 完了したイベントコードチェックフラグ
            bool checkCompletedEventBool = false;

            // プレイヤーデータと条件を比べる
            if (playerdata.progress < mainEventModel.requiredProgress) continue;
            if (playerdata.satisfaction < mainEventModel.requiredSatisfaction) continue;
            if (!playerdata.currentScene.Equals(mainEventModel.requiredScene)) continue;

            string[] requiredCompletedEventArray = mainEventModel.requiredCompletedEvent.Split(':');
            if (!requiredCompletedEventArray[0].Equals(""))
            {
                foreach (string requiredCompletedEvent in requiredCompletedEventArray)
                {
                    if(playerdata.eventCodeArray != null)
                    {
                        int resultInt = Array.IndexOf(playerdata.eventCodeArray, requiredCompletedEvent);
                        if (resultInt < 0)
                        {
                            checkCompletedEventBool = false;
                            break;
                        }
                        else
                        {
                            checkCompletedEventBool = true;
                        }
                    }
                    else
                    {
                        checkCompletedEventBool = false;
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("No requiredEventCode");
                checkCompletedEventBool = true;
            }
            // 最後の条件まで合うならメインイベントをリストに追加する
            if (checkCompletedEventBool) mainEventModelList.Add(mainEventModel);
        }
        // 条件に合うメインイベントが2つ以上なら一つのメインイベントだけreturnする
        if (mainEventModelList.Count > 1) return PickUpOneMainEvent(mainEventModelList);
        else if (mainEventModelList.Count == 1) return mainEventModelList[0].eventCode;
        else return null;
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
