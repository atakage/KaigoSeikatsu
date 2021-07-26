using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager1 : MonoBehaviour
{
    public ChatManager chatManager;
    public MsgChoiceManager msgChoiceManager;
    public SceneTransitionManager sceneTransitionManager;
    public EventManager eventManager;
    public PlayerSaveDataManager playerSaveDataManager;
    public PlayerData playerData;
    public GameObject canvasObj;
    private void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        sceneTransitionManager = new SceneTransitionManager();
        

        // 外部componentからスクリプトを読み込む
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;
        canvasObj = GameObject.Find("Canvas");

        // イベントリストファイルを読み込む(.json)
        EventListData[] loadedEventListData = playerSaveDataManager.LoadedEventListData();
        // イベントを探す
        EventListData eventItem = eventManager.FindEventByCode(loadedEventListData, "EV000");
        // イベントスクリプトを配列に入れる
        List<string[]> scriptList = eventManager.ScriptSaveToList(eventItem);
        chatManager.ShowDialogue(scriptList, "EV000", eventItem.script);
    }


    private void Update()
    {
        
        if (canvasObj.transform.Find("fadeOutPersistEventCheck") != null
            && canvasObj.transform.Find("fadeOutPersistEventCheck").GetComponent<Text>().text.Equals("Y"))
        {
            
            if (canvasObj.transform.Find("endedEventCode") != null
                && canvasObj.transform.Find("endedEventCode").GetComponent<Text>().text.Equals("EV000"))
            {
                Debug.Log("goToAtHome");
                canvasObj.transform.Find("fadeOutEventCheck").GetComponent<Text>().text = "";
                canvasObj.transform.Find("endedEventCode").GetComponent<Text>().text = "";
                playerData = playerSaveDataManager.LoadPlayerData();
                playerData.currentScene = "AtHomeScene";
                playerSaveDataManager.SavePlayerData(playerData);
                sceneTransitionManager.LoadTo("AtHomeScene");
            }
            
        }
        
    }
}
