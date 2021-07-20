using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConvenienManager : MonoBehaviour
{
    public PlayerSaveDataManager playerSaveDataManager;
    public EventManager eventManager;
    public ChatManager chatManager;
    //public ConvenienceItemSetManager convenienceItemSetManager;
    public ConvenienceUIManager convenienceUIManager;
    public GameObject canvasGameObj;
    public PlayerData playerData;
    public ConvenienceItemData[] convenienceItemDataArray;
    public string loadValueSW;
    // Start is called before the first frame update
    void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;
        //convenienceItemSetManager = new ConvenienceItemSetManager();
        convenienceUIManager = new ConvenienceUIManager();

            // TitleSceneからロードした時やMenuSceneからもどる時についてくるvalue
        if (GameObject.Find("loadValueSW") != null) loadValueSW = GameObject.Find("loadValueSW").transform.GetComponent<Text>().text;
        else loadValueSW = "N";

        canvasGameObj = GameObject.Find("Canvas");

            // プレイヤーデータでUIセット
        playerData = playerSaveDataManager.LoadPlayerData();
        canvasGameObj.transform.Find("time").GetComponent<Text>().text = playerData.time;
        canvasGameObj.transform.Find("fatigueBar").GetComponent<Slider>().value = playerData.fatigue;

        // 挨拶イベント
        if(!loadValueSW.Equals("Y")) LoadEventAndShow("EV012");

    }

    void Update()
    {
        // 店員さん挨拶イベント
        if ("END".Equals(canvasGameObj.transform.Find("textEventEndSW").GetComponent<Text>().text) || loadValueSW.Equals("Y"))
        {
            canvasGameObj.transform.Find("menuBox").gameObject.SetActive(true);
            canvasGameObj.transform.Find("orderBox").gameObject.SetActive(true);
            canvasGameObj.transform.Find("orderConfirmButton").gameObject.SetActive(true);
            canvasGameObj.transform.Find("specificationBox").gameObject.SetActive(true);
            convenienceUIManager.ItemClickPanelUISetting(true);
            MenuAndNextButtonInteractable(true);
            canvasGameObj.transform.Find("textEventEndSW").GetComponent<Text>().text = "";
        }
        // 初期化------------------------------------------------------------------------------------------------------------------
        if (loadValueSW.Equals("Y"))
        {
            loadValueSW = "N";
            if (GameObject.Find("loadValueSW") != null) Destroy(GameObject.Find("loadValueSW"));
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
