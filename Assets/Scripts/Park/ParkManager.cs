using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ParkManager : MonoBehaviour
{
    public PlayerSaveDataManager playerSaveDataManager;
    public EventManager eventManager;
    public EventCodeManager eventCodeManager;
    public ChatManager chatManager;
    public UtilManager utilManager;
    public ConvenienceItemSetManager convenienceItemSetManager;
    public SceneTransitionManager sceneTransitionManager;
    public GameObject canvasGameObj;
    void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        eventCodeManager = new EventCodeManager();
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;
        utilManager = new UtilManager();
        sceneTransitionManager = new SceneTransitionManager();
        convenienceItemSetManager = new ConvenienceItemSetManager();

        canvasGameObj = GameObject.Find("Canvas");
        PlayerData playerData = playerSaveDataManager.LoadPlayerData();
        canvasGameObj.transform.Find("time").GetComponent<Text>().text = playerData.time;
        canvasGameObj.transform.Find("fatigueBar").GetComponent<Slider>().value = playerData.fatigue;

        canvasGameObj.transform.Find("walkButton").GetComponent<Button>().onClick.AddListener(() => ClickWalkAndExerciseButton("散歩"));
        canvasGameObj.transform.Find("exerciseButton").GetComponent<Button>().onClick.AddListener(() => ClickWalkAndExerciseButton("運動"));

        LoadEventAndShow("EV014");
    }

    private void Update()
    {
        // テキストイベント終了後
        if ("END".Equals(canvasGameObj.transform.Find("textEventEndSW").GetComponent<Text>().text)
            && canvasGameObj.transform.Find("endedTextEventCode").GetComponent<Text>().text.Equals("EV014"))
        {
            SetButtonUI(true);
            SetActiveWalkAndExerciseBtn(true);
            canvasGameObj.transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text = "何をしようか?";
            canvasGameObj.transform.Find("textEventEndSW").GetComponent<Text>().text = "";

        }else if ("END".Equals(canvasGameObj.transform.Find("textEventEndSW").GetComponent<Text>().text)
            && canvasGameObj.transform.Find("endedTextEventCode").GetComponent<Text>().text.Equals("EV017"))
        {
            canvasGameObj.transform.Find("textEventEndSW").GetComponent<Text>().text = "";
            // 家に帰るイベント
            LoadEventAndShow("EV016");
        
        // 運動イベント
        }else if ("END".Equals(canvasGameObj.transform.Find("textEventEndSW").GetComponent<Text>().text)
            && (canvasGameObj.transform.Find("endedTextEventCode").GetComponent<Text>().text.Equals("EV018")
               || canvasGameObj.transform.Find("endedTextEventCode").GetComponent<Text>().text.Equals("EV019")))
        {
            string endedTextEventCode = canvasGameObj.transform.Find("endedTextEventCode").GetComponent<Text>().text;
            // プレイヤーの疲れ数値変更
            PlayerData playerData = playerSaveDataManager.LoadPlayerData();
            if (endedTextEventCode.Equals("EV018")) playerData.fatigue -= (float)3;
            else if (endedTextEventCode.Equals("EV019")) playerData.satisfaction += 3;
            playerSaveDataManager.SavePlayerData(playerData);

            canvasGameObj.transform.Find("textEventEndSW").GetComponent<Text>().text = "";
            // 家に帰るイベント
            LoadEventAndShow("EV016");
        }

        // 散歩イベント
        // Actionイベントの終了を確認
        if (!canvasGameObj.transform.Find("endedActionEventCode").GetComponent<Text>().text.Equals(""))
        {
            // 2次action発動
            if (canvasGameObj.transform.Find("endedActionEventCode").GetComponent<Text>().text.Equals("EV015"))
            {
                canvasGameObj.transform.Find("endedActionEventCode").GetComponent<Text>().text = "";
                Debug.Log("before call PickUpItemOrMoneyEvent");
                PickUpMoneyOrItemEvent();

            }    
        }

        // 家へ帰る前のスクリプトイベント
        if (canvasGameObj.transform.Find("onlyScriptEventEnd").GetComponent<Text>().text.Equals("END"))
        {
            canvasGameObj.transform.Find("onlyScriptEventEnd").GetComponent<Text>().text = "";
            LoadEventAndShow("EV016");
        }
        // 家へ帰る
        if (canvasGameObj.transform.Find("fadeOutPersistEventCheck").GetComponent<Text>().text.Equals("Y"))
        {
            PlayerData playerData = playerSaveDataManager.LoadPlayerData();
            DateTime addedDateTime = utilManager.TimeCal(playerData.time, 60);
            playerData.currentScene = "AtHomeScene";
            playerData.time = addedDateTime.Hour.ToString("D2") + ":" + addedDateTime.Minute.ToString("D2");
            playerSaveDataManager.SavePlayerData(playerData);

            sceneTransitionManager.LoadTo("AtHomeScene");
        }
    }

    public void ClickWalkAndExerciseButton(string action)
    {
        SetButtonUI(false);
        SetActiveWalkAndExerciseBtn(false);
        canvasGameObj.transform.Find("actionReadyAlertBox").transform.Find("Text").GetComponent<Text>().text = action + "をしながら時間を過ごしますか?";
        canvasGameObj.transform.Find("actionReadyAlertBox").gameObject.SetActive(true);

        canvasGameObj.transform.Find("actionReadyAlertBox").transform.Find("confirmButton").GetComponent<Button>().onClick.AddListener(() => ClickActionReadyAlertConfirmButton(action));
        canvasGameObj.transform.Find("actionReadyAlertBox").transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(() => ClickActionReadyAlertCancelButton());
        
    }

    public void ClickActionReadyAlertConfirmButton(string action)
    {
        PlayerData playerData = playerSaveDataManager.LoadPlayerData();
        // TimeCal(playerDataTime, addingMinute)
        DateTime addedDateTime = utilManager.TimeCal(playerData.time, 60);
        playerData.currentScene = "AtHomeScene";
        playerData.time = addedDateTime.Hour.ToString("D2") + ":" + addedDateTime.Minute.ToString("D2");
        playerSaveDataManager.SavePlayerData(playerData);

        canvasGameObj.transform.Find("actionReadyAlertBox").gameObject.SetActive(false);
        SetButtonUI(true);
        SetActiveWalkAndExerciseBtn(false);
        // ボタンのlistenerが積もるのを防止するためにイベント後呼び出されたもとのボタンのlistenerをすべて削除する
        canvasGameObj.transform.Find("actionReadyAlertBox").transform.Find("confirmButton").GetComponent<Button>().onClick.RemoveAllListeners();

        string eventCode = "";

        // 散歩や運動のイベントコードを読み込む
        if ("散歩".Equals(action))
        {
            eventCode = eventCodeManager.GetParkWalkEventCode();
        }
        else if ("運動".Equals(action))
        {
            eventCode = eventCodeManager.GetExerciseEventCode();
        }
        // イベント発動
        LoadEventAndShow(eventCode);

    }

    public void ClickActionReadyAlertCancelButton()
    {
        canvasGameObj.transform.Find("actionReadyAlertBox").gameObject.SetActive(false);
        SetButtonUI(true);
        SetActiveWalkAndExerciseBtn(true);
        // ボタンのlistenerが積もるのを防止するためにイベント後呼び出されたもとのボタンのlistenerをすべて削除する
        canvasGameObj.transform.Find("actionReadyAlertBox").transform.Find("cancelButton").GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void SetActiveWalkAndExerciseBtn(bool sw)
    {
        canvasGameObj.transform.Find("exerciseButton").gameObject.SetActive(sw);
        canvasGameObj.transform.Find("walkButton").gameObject.SetActive(sw);
    }

    public void SetButtonUI(bool sw)
    {
        canvasGameObj.transform.Find("menuButton").GetComponent<Button>().interactable = sw;
        canvasGameObj.transform.Find("nextButton").GetComponent<Button>().interactable = sw;
    }

    public void PickUpMoneyOrItemEvent()
    {
        System.Random random = new System.Random();
        int eventInt = random.Next(0, 2);
        // 0ならお金獲得
        if (0 == eventInt)
        {
            // 100円 ~ 500円
            int money = random.Next(1, 6) * 100;
            PlayerData playerData = playerSaveDataManager.LoadPlayerData();
            playerData.money = (Int32.Parse(playerData.money) + money).ToString();
            playerSaveDataManager.SavePlayerData(playerData);

            List<string[]> scriptList = eventManager.SingleScriptSaveToList(money + "円を拾った!");
            chatManager.ShowDialogue(scriptList, "");
        }
        // 1ならアイテム獲得
        else if (1 == eventInt)
        {
            // コンビニのアイテムリストを取り出す
            ConvenienceItemData[] convenienceItemDataArray = convenienceItemSetManager.GetConvenienceJsonFile();
            // アイテムリストから一つをランダムで取り出す
            string pickUpItemName = SearchluckyItemToConvenience(convenienceItemDataArray);

            List<string[]> scriptList = eventManager.SingleScriptSaveToList(pickUpItemName + "を拾った!");
            chatManager.ShowDialogue(scriptList, "");
        }
    }

    public string SearchluckyItemToConvenience(ConvenienceItemData[] convenienceItemDataArray)
    {
        System.Random random = new System.Random();
        Debug.Log("convenienceItemDataArray.length: " + convenienceItemDataArray.Length);
        // ランダムアイテム１つを取り出す
        ConvenienceItemData convenienceItemData = convenienceItemDataArray[random.Next(0, convenienceItemDataArray.Length)];
        Debug.Log(convenienceItemData.itemName + "を獲得");
        // プレイヤーアイテムに追加する
        ItemListData itemListData = new ItemListData();
        itemListData.itemDescription = convenienceItemData.itemDescription;
        itemListData.itemName = convenienceItemData.itemName;
        itemListData.keyItem = "N";
        itemListData.quantity = random.Next(1, 4);
        Debug.Log("ランダムアイテム数: " + itemListData.quantity);

        ItemListData[] itemListDataArray = new ItemListData[1];
        itemListDataArray[0] = itemListData;

        playerSaveDataManager.SaveItemListData(itemListDataArray);

        return convenienceItemData.itemName;
    }
    public void LoadEventAndShow(string eventCode)
    {
        Debug.Log("call LoadEventAndShow");
        EventListData[] loadedEventListData = playerSaveDataManager.LoadedEventListData();
        EventListData eventItem = eventManager.FindEventByCode(loadedEventListData, eventCode);
        List<string[]> scriptList = eventManager.ScriptSaveToList(eventItem);
        chatManager.ShowDialogue(scriptList, eventCode);
    }
}
