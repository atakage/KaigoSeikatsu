using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager1 : MonoBehaviour
{
    public ChatManager chatManager;
    public EventManager eventManager;
    public PlayerSaveDataManager playerSaveDataManager;
    private void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();

        // 外部componentからスクリプトを読み込む
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;
        // イベントリストファイルを読み込む(.json)
        EventListData[] loadedEventListData = playerSaveDataManager.LoadedEventListData();
        // イベントを探す
        EventListData eventItem = eventManager.FindEventByCode(loadedEventListData, "EV000");
        // イベントスクリプトを配列に入れる
        List<string[]> scriptList = eventManager.ScriptSaveToList(eventItem);
        chatManager.ShowDialogue(scriptList, "EV000");
    }


    private void Update()
    {
        if(chatManager.completeEventSW["EV000"] == true)
        {
            Debug.Log("選択肢表示");
        }
    }
}
