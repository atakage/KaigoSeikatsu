using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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

    // ChoiceEvent
    public string GetChoiceEvent(string eventCode)
    {
        TextAsset textAsset = Resources.Load("eventScript/"+eventCode+"-CHOICE", typeof(TextAsset)) as TextAsset;
        return textAsset.text;
    }

    public List<string[]> SingleScriptSaveToList(string script)
    {

    }

    public List<string[]> ScriptSaveToList(EventListData eventItem)
    {
        List<string[]> returnScriptArrList = new List<string[]>();
        string[] scriptArrayPara = eventItem.script.Split('/');
        string[] scriptArray = null;
        char[] chars = null;
        for (int i=0; i< scriptArrayPara.Length; i++)
        {
            // '/'分かれた文章を文字ごとで入れる
            chars = scriptArrayPara[i].ToCharArray();
            Array.Resize(ref scriptArray, chars.Length);
            for(int j=0; j < chars.Length; j++)
            {
                scriptArray[j] = new string(chars[j], 1);
            }
            returnScriptArrList.Add(scriptArray);
        }
        return returnScriptArrList;
    }

    public EventListData FindEventByCode(EventListData[] eventListData, string eventCode)
    {
        foreach (EventListData eventItem in eventListData)
        {
            if(eventCode.Equals(eventItem.eventCode)) return eventItem;
        }
        return null;
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
