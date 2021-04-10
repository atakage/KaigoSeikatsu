using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConvenienManager : MonoBehaviour
{
    public PlayerSaveDataManager playerSaveDataManager;
    public EventManager eventManager;
    public ChatManager chatManager;
    public GameObject canvasGameObj;
    public PlayerData playerData;
    // Start is called before the first frame update
    void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;


        canvasGameObj = GameObject.Find("Canvas");

            // プレイヤーデータでUIセット
        playerData = playerSaveDataManager.LoadPlayerData();
        canvasGameObj.transform.Find("time").GetComponent<Text>().text = playerData.time;
        canvasGameObj.transform.Find("fatigueBar").GetComponent<Slider>().value = playerData.fatigue;

        // 挨拶イベント
        LoadEventAndShow("EV012");

            // コンビニで販売するアイテムリストを読み込む(json)
        //CallConvenienceSaleItemJson();
    }

    void Update()
    {
        // 店員さん挨拶イベント
        if ("END".Equals(canvasGameObj.transform.Find("textEventEndSW").GetComponent<Text>().text))
        {
            MenuAndNextButtonInteractable(true);
            canvasGameObj.transform.Find("textEventEndSW").GetComponent<Text>().text = "";
        }
    }

    public void LoadEventAndShow(string eventCode)
    {
        EventListData[] loadedEventListData = playerSaveDataManager.LoadedEventListData();
        EventListData eventItem = eventManager.FindEventByCode(loadedEventListData, eventCode);
        List<string[]> scriptList = eventManager.ScriptSaveToList(eventItem);
        chatManager.ShowDialogue(scriptList, eventCode);
    }

    public void MenuAndNextButtonInteractable(bool sw)
    {
        canvasGameObj.transform.Find("menuButton").GetComponent<Button>().interactable = sw;
        canvasGameObj.transform.Find("nextButton").GetComponent<Button>().interactable = sw;
    }
}
