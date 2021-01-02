using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public PlayerSaveDataManager playerSaveDataManager;
    public EventScriptDicData eventScriptDicData;
    // Start is called before the first frame update
    void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();

        // イベントスクリプトを読む
        eventScriptDicData = playerSaveDataManager.GetEventScript();
        // 新しいイベントを登録する時や既存イベントを修正するときに使う
        EventListData[] eventListData = CreateEventListForSave(eventScriptDicData);
        // 新しいイベントを登録する時や既存イベントを修正するときに使う
        Debug.Log("Event Save Start-----------------------------------------------------------");
        playerSaveDataManager.SaveEventListData(eventListData);

    }

    public EventListData[] CreateEventListForSave(EventScriptDicData eventScriptDicData)
    {
        EventListData[] eventListArr = null;
        int index = 0;
        string[] scriptORSwitch = null;
        //イベントリストを一つでまとめる
        Dictionary<string, string> EVEventListDic = eventScriptDicData.EVEventScript;
        Dictionary<string, string> ETEventListDic = eventScriptDicData.ETEventScript;
        Dictionary<string, string> mergedEventListDic = new Dictionary<string, string>();
        mergedEventListDic = EVEventListDic;
        foreach (KeyValuePair<string, string> item in ETEventListDic)
        {
            mergedEventListDic.Add(item.Key, item.Value);
        }

        Debug.Log("allEventCount: " + mergedEventListDic.Count);
        eventListArr = new EventListData[mergedEventListDic.Count];

        // イベントファイル数を読み出して配列を作る
        foreach (KeyValuePair<string, string> item in mergedEventListDic)
        {
            eventListArr[index] = new EventListData();
            eventListArr[index].eventCode = item.Key;
            scriptORSwitch = item.Value.Split('|');
            eventListArr[index].script = scriptORSwitch[0];
            // 使うイベント:ON, 使わないイベント:OFF
            eventListArr[index].optionSW = scriptORSwitch[1];
            ++index;
        }
        

        return eventListArr;
    }
}
