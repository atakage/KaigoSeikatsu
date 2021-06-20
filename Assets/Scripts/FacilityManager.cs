using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FacilityManager : MonoBehaviour
{
    public PlayerSaveDataManager playerSaveDataManager;
    public EventManager eventManager;
    public ChatManager chatManager;
    public SceneTransitionManager sceneTransitionManager;
    public MainEventManager mainEventManager;
    public UtilManager utilManager;
    public JobEventManager jobEventManager;
    public JobEventSetManager jobEventSetManager;
    public Button nextButton;
    public PlayerData playerData = null;
    public GameObject canvasObj;
    public string[] morningrequiredEvent = null;
    //public string[] careQuizEvent = null;
    public string[] lunchEvent = null;
    public string[] recreationEvent = null;
    public string[] afternoonEvent = null;
    public bool timeCheckSW;
    public bool jobEventDayCompletedBool;
    // Start is called before the first frame update
    void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;
        sceneTransitionManager = new SceneTransitionManager();
        utilManager = new UtilManager();
        mainEventManager = new MainEventManager();
        jobEventManager = new JobEventManager();
        jobEventSetManager = new JobEventSetManager();

        nextButton = GameObject.Find("Canvas").transform.Find("nextButton").GetComponent<Button>();
        nextButton.onClick.AddListener(ClickNextButton);

        canvasObj = GameObject.Find("Canvas");

        playerData = playerSaveDataManager.LoadPlayerData();
        Debug.Log("playerData: " + playerData.ToString());

        canvasObj.transform.Find("GoToCafeButton").GetComponent<Button>().onClick.AddListener(delegate { ClickGoToButton("カフェ"); });
        canvasObj.transform.Find("GoToParkButton").GetComponent<Button>().onClick.AddListener(delegate { ClickGoToButton("公園"); });
        canvasObj.transform.Find("AlertGoing").transform.Find("No").GetComponent<Button>().onClick.AddListener(ClickGoToAlertNoButton);
        canvasObj.transform.Find("AlertGoing").transform.Find("Yes").GetComponent<Button>().onClick.AddListener(ClickGoToAlertYesButton);

        // イベントコードセット
        morningrequiredEvent = new string[]{ "EV001", "EV002", "EV003" };  // 08:00 ~ 09:00
        //careQuizEvent = new string[] { "ET000"}; // 9:00 ~ 11:50
        lunchEvent = new string[] {"EV004"}; // 11:50 ~ 12:50
        recreationEvent = new string[] {"EV005","EV006","EV007" }; // 14:00 ~ 17:00
        afternoonEvent = new string[] {"EV008"}; // 17:00

        timeCheckSW = true;
        // Panelを除いたUI Display off
        FacilityUISetActive(false);

        // ランダムで朝のイベント
        if (playerData.time.Equals("09:00"))
        {
            string morningEventCode = CallRandomEvent(morningrequiredEvent);
            LoadEventAndShow(morningEventCode);
        }
        else
        {
            FacilityUISetActive(true);
        }
        

        

        // UI setting
        GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text = playerData.time;
        GameObject.Find("Canvas").transform.Find("fatigueBar").GetComponent<Slider>().value = playerData.fatigue;


    }

    private void Update()
    {
        if (timeCheckSW)
        {
            string timeStr = canvasObj.transform.Find("time").GetComponent<Text>().text;

            // メインイベントが終わった場合
            if (canvasObj.transform.Find("mainEventCompleteSW") != null
                && canvasObj.transform.Find("fadeOutEndMomentSW") != null
                && canvasObj.transform.Find("mainEventCompleteSW").GetComponent<Text>().text.Equals("Y")
                && canvasObj.transform.Find("time").gameObject.activeInHierarchy
                && canvasObj.transform.Find("fadeOutEndMomentSW").GetComponent<Text>().text.Equals("Y"))
            {
                chatManager.DestroyMainEventBlackBox();
                Destroy(canvasObj.transform.Find("mainEventCompleteSW").gameObject);
            }
            // jobEventが終わった場合
            else if (canvasObj.transform.Find("jobEventCompleteSW") != null
                && canvasObj.transform.Find("onlyScriptEventEnd") != null
                && canvasObj.transform.Find("jobEventCompleteSW").GetComponent<Text>().text.Equals("Y")
                && canvasObj.transform.Find("onlyScriptEventEnd").GetComponent<Text>().text.Equals("END"))
            {
                // 初期化
                canvasObj.transform.Find("jobEventCompleteSW").GetComponent<Text>().text = "N";
                canvasObj.transform.Find("onlyScriptEventEnd").GetComponent<Text>().text = "";

                // UI時間更新
                chatManager.SetTime();
                // fade out 
                chatManager.executeFadeOutSimple();
                // uiをActive
            }
            else if (timeStr.Equals("12:50") && GameObject.Find("Canvas").transform.Find("time").gameObject.activeInHierarchy)
            {
                SetPanelText("休憩時間だ\n何をしようかな?");
                timeCheckSW = false;
            }
            else if (timeStr.Equals("11:50") && GameObject.Find("Canvas").transform.Find("time").gameObject.activeInHierarchy)
            {
                SetPanelText("もうすぐお昼の時間だ");
                timeCheckSW = false;
            }
            // careQuizEventがNOのとき
            else if (timeStr.Equals("11:50") && GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("FadeSwitchText").GetComponent<Text>().text.Equals("call"))
            {
                FacilityUISetActive(true);
                SetPanelText("もうすぐお昼の時間だ");
                timeCheckSW = false;
                GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("FadeSwitchText").GetComponent<Text>().text = "";
            }
            else if (timeStr.Equals("14:00") && GameObject.Find("Canvas").transform.Find("time").gameObject.activeInHierarchy)
            {
                SetPanelText("まもなく午後のスケジュールが始まる");
                timeCheckSW = false;
            }
            else if (timeStr.Equals("17:00") && GameObject.Find("Canvas").transform.Find("time").gameObject.activeInHierarchy)
            {
                SetPanelText("午後のスケジュールが終わった");
                timeCheckSW = false;
            }
            else if (timeStr.Equals("17:20") && GameObject.Find("Canvas").transform.Find("time").gameObject.activeInHierarchy)
            {
                SetPanelText("これからどうする?");
                SetGoToButton(true);
                GameObject.Find("nextButton").transform.Find("Text").GetComponent<Text>().text = "帰宅";
                timeCheckSW = false;
            }
            // 17:20 -> 家へ
            else if (GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("FadeSwitchText").GetComponent<Text>().text.Equals("call") &&
                GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("DestinationValue").GetComponent<Text>().text.Equals("帰宅"))
            {
                playerData.time = GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text;
                playerData.currentScene = "AtHomeScene";
                playerSaveDataManager.SavePlayerData(playerData);
                sceneTransitionManager.LoadTo("AtHomeScene");
            }
            // 17:20 -> カフェへ
            else if (GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("FadeSwitchText").GetComponent<Text>().text.Equals("call") &&
                GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("DestinationValue").GetComponent<Text>().text.Equals("カフェ"))
            {
                playerData.time = GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text;
                playerData.currentScene = "CafeScene";
                playerSaveDataManager.SavePlayerData(playerData);
                sceneTransitionManager.LoadTo("CafeScene");
            }
            // 17:20 -> 公園へ
            else if (GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("FadeSwitchText").GetComponent<Text>().text.Equals("call") &&
                GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("DestinationValue").GetComponent<Text>().text.Equals("公園"))
            {
                playerData.time = GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text;
                playerData.currentScene = "ParkScene";
                playerSaveDataManager.SavePlayerData(playerData);
                sceneTransitionManager.LoadTo("ParkScene");
            }

            // UI初期化---------------------------------------------------------------------------------------------------------------
            // fade out後menuButtonとnextButtonのinteractableをtrue
            if (canvasObj.transform.Find("fadeOutEndMomentSW") != null
                && canvasObj.transform.Find("fadeOutEndMomentSW").GetComponent<Text>().text.Equals("Y"))
            {
                menuBtnAndNextBtnInteractable(true);

            }

            // 初期化------------------------------------------------------------------------------------------------------------------
            if (canvasObj.transform.Find("fadeOutEndMomentSW") != null)
            {
              Destroy(canvasObj.transform.Find("fadeOutEndMomentSW").gameObject);
            }
        }
        
        
    }

    public void ClickNextButton()
    {
        string timeStr = GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text;

        string nextBtnText = GameObject.Find("nextButton").transform.Find("Text").GetComponent<Text>().text;

        // nextButtonのテキストが進行なら
        if (nextBtnText.Equals("進行"))
        {
            //時間による次のイベント(switch)
            switch (timeStr)
            {
                // ９時なら(-> 11:00)
                case "09:00":
                        // ランダムで介護クイズイベント発動

                              // 発動できるメインイベントがあるならメインイベントを先にする
                    bool completeMainEvent = RunMainEvent();
                              // jobEventを発動させるかを決める
                    string callEventFlag = utilManager.GetYesOrNo();// 'YES' or 'NO'

                              // メインイベントを発動しない状態そしてjobEventを発動すると
                    if (completeMainEvent != true && "YES".Equals(callEventFlag))
                    {
                        // jobEventの中でactiveされているイベントをランダムで呼び出す
                        JobEventModel[] jobeventModelArray = jobEventSetManager.GetJobEventJsonFile();
                        JobEventModel jobEvent = jobEventManager.GetActiveJobEventRandom(jobeventModelArray);

                        if(jobEvent != null)
                        {
                            LoadJobEventAndShow(jobEvent);
                            menuBtnAndNextBtnInteractable(false);
                        }
                        else
                        {
                            // fade out
                            FacilityUISetActive(false);
                            chatManager.executeFadeOutSimple();
                            chatManager.SetTime();
                        }
                        // 他の時間にjobEventが発動しないようにフラグ処理
                        jobEventDayCompletedBool = true;
                    }
                    // イベントを発動させないとそのまま進行する
                    else if(completeMainEvent != true && !"YES".Equals(callEventFlag))
                    {
                        // fade out
                        FacilityUISetActive(false);
                        chatManager.executeFadeOutSimple();
                        chatManager.SetTime();
                    }

                    /*
                    string eventCode = CallRandomEventAddNone(careQuizEvent);
                    if (!eventCode.Equals("NO"))
                    {
                        LoadEventAndShow(eventCode);
                    }
                    // クイズイベントなかったら時間がたつ(->11:50)
                    else
                    {
                        // fade out
                        FacilityUISetActive(false);
                        chatManager.executeFadeOutSimple();
                        chatManager.SetTime();
                    }
                    */

                    // ゲームロードをしながらイベントを繰り返す行為を防ぐために
                    // プレイヤーデータに時間をアプデ
                    playerData = playerSaveDataManager.LoadPlayerData();
                    playerData.time = "11:50";
                    playerSaveDataManager.SavePlayerData(playerData);

                    timeCheckSW = true;
                    break;
                // 11時なら食事に (-> 12:00(昼ご飯) -> 12:50)
                case "11:50":
                    FacilityUISetActive(false);
                    LoadEventAndShow(CallRandomEvent(lunchEvent));

                    playerData = playerSaveDataManager.LoadPlayerData();
                    playerData.time = "12:50";
                    playerSaveDataManager.SavePlayerData(playerData);

                    // 時間が経つ
                    chatManager.SetTime();
                    timeCheckSW = true;
                    break;
                // 12:50なら休憩時間( -> 14:00)
                case "12:50":
                    FacilityUISetActive(false);
                    SetPanelText("");

                    playerData = playerSaveDataManager.LoadPlayerData();
                    playerData.time = "14:00";
                    playerSaveDataManager.SavePlayerData(playerData);

                    chatManager.SetTime();
                    chatManager.executeFadeOut();
                    timeCheckSW = true;
                    break;
                // 14時ならレクリエーションの時間( -> 17:00)
                case "14:00":
                    FacilityUISetActive(false);
                    LoadEventAndShow(CallRandomEvent(recreationEvent));

                    playerData = playerSaveDataManager.LoadPlayerData();
                    playerData.time = "17:00";
                    playerSaveDataManager.SavePlayerData(playerData);

                    // 時間が経つ
                    chatManager.SetTime();
                    timeCheckSW = true;
                    break;
                // 17時なら帰宅準備( -> 仕事終り)
                case "17:00":
                    FacilityUISetActive(false);
                    LoadEventAndShow(CallRandomEvent(afternoonEvent));

                    playerData = playerSaveDataManager.LoadPlayerData();
                    playerData.time = "17:20";
                    playerSaveDataManager.SavePlayerData(playerData);

                    // 時間が経つ
                    chatManager.SetTime();
                    timeCheckSW = true;
                    break;
            }
        }
        // nextButtonのテキストが進行がないなら帰宅する
        else
        {
            ClickGoToHomeButton();
        }


    }

    // 17:00 -> 18:00
    public void ClickGoToAlertYesButton()
    {
        // プレイヤーデータに時間をセーブ
        PlayerData playerData = playerSaveDataManager.LoadPlayerData();
        playerData.time = "18:00";
        //playerSaveDataManager.SavePlayerData(playerData);

        GameObject.Find("Canvas").transform.Find("AlertGoing").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Panel").gameObject.SetActive(false);
        FacilityUISetActive(false);
        chatManager.executeFadeOutSimple();
        chatManager.SetTime();
        timeCheckSW = true;
    }

    public void ClickGoToAlertNoButton()
    {
        GameObject.Find("Canvas").transform.Find("AlertGoing").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("GoToCafeButton").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("GoToParkButton").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("menuButton").GetComponent<Button>().interactable = true;
        GameObject.Find("Canvas").transform.Find("nextButton").GetComponent<Button>().interactable = true;
    }

    public void ClickGoToHomeButton()
    {
        SetGoToButton(false);
        GameObject.Find("Canvas").transform.Find("AlertGoing").gameObject.SetActive(true);
        GameObject.Find("AlertGoing").transform.Find("DestinationValue").gameObject.SetActive(false);
        GameObject.Find("AlertGoing").transform.Find("alertMessage").GetComponent<Text>().text =
        "<color=#f54242>" + "帰宅" + "</color>"  + "しますか?";
        GameObject.Find("AlertGoing").transform.Find("DestinationValue").GetComponent<Text>().text = "帰宅";
    }

    public void ClickGoToButton(string destination)
    {
        SetGoToButton(false);
        GameObject.Find("Canvas").transform.Find("AlertGoing").gameObject.SetActive(true);
        GameObject.Find("AlertGoing").transform.Find("DestinationValue").gameObject.SetActive(false);
        GameObject.Find("AlertGoing").transform.Find("alertMessage").GetComponent<Text>().text =
        "<color=#f54242>" + destination + "</color>" + "に行って時間を過ごしますか?";
        GameObject.Find("AlertGoing").transform.Find("DestinationValue").GetComponent<Text>().text = destination;
    }

    public void SetGoToButton(bool sw)
    {
        if (sw)
        {
            GameObject.Find("Canvas").transform.Find("GoToCafeButton").gameObject.SetActive(sw);
            GameObject.Find("Canvas").transform.Find("GoToParkButton").gameObject.SetActive(sw);
        }
        else
        {
            GameObject.Find("Canvas").transform.Find("GoToCafeButton").gameObject.SetActive(sw);
            GameObject.Find("Canvas").transform.Find("GoToParkButton").gameObject.SetActive(sw);
        }
    }

    public void SetPanelText(string text)
    {
        GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text = text;    
    }

    public bool RunMainEvent()
    {
        bool returnValue;

        // 条件に合うMainEventを探す
        playerData = playerSaveDataManager.LoadPlayerData();
        string mainEventCode = mainEventManager.findMainEvent(playerData);
        if (mainEventCode != null)
        {
            // 条件に合うMainEventを発動させる前にすでに完了になっているかを確認する
            bool completedEventBool = mainEventManager.CheckCompletedMainEvent(mainEventCode);
            Debug.Log("completedEventBool: " + completedEventBool);
            // 完了されたイベントがないならメインイベント発動
            if (!completedEventBool)
            {
                EventListData[] loadedEventListData = playerSaveDataManager.LoadedEventListData();
                EventListData eventItem = eventManager.FindEventByCode(loadedEventListData, mainEventCode);
                List<string[]> scriptList = eventManager.ScriptSaveToList(eventItem);

                chatManager.ShowDialogueForMainEvent(scriptList, mainEventCode);


                playerData = playerSaveDataManager.LoadPlayerData();
                // addingprogress
                int addingProgress = mainEventManager.getAddingProgressFromMainEventJsonFile(mainEventCode);
                playerData.progress += addingProgress;

                // 終わったMainEventはプレイヤーデータに記録する
                string[] eventCodeArray = playerSaveDataManager.SaveCompletedEvent(playerData.eventCodeArray, mainEventCode);
                playerData.eventCodeArray = eventCodeArray;
                playerSaveDataManager.SavePlayerData(playerData);

                returnValue = true;
            }
            else
            {
                returnValue = false;
            }
        }
        else
        {
            returnValue = false;
        }
        return returnValue;
    }

    public void LoadJobEventAndShow(JobEventModel jobEvent)
    {
        EventListData eventListData = new EventListData();
        eventListData.script = jobEvent.eventScript;
        List<string[]> scriptList = eventManager.ScriptSaveToList(eventListData);
        chatManager.ShowDialogueForJobEvent(scriptList, jobEvent);
    }

    public void LoadEventAndShow(string eventCode)
    {
        EventListData[] loadedEventListData = playerSaveDataManager.LoadedEventListData();
        EventListData eventItem = eventManager.FindEventByCode(loadedEventListData, eventCode);
        List<string[]> scriptList = eventManager.ScriptSaveToList(eventItem);
        chatManager.ShowDialogue(scriptList, eventCode);
    }

    public void menuBtnAndNextBtnInteractable(bool sw)
    {
        canvasObj.transform.Find("menuButton").GetComponent<Button>().interactable = sw;
        canvasObj.transform.Find("nextButton").GetComponent<Button>().interactable = sw;
    }

    public void FacilityUISetActive(bool setActive)
    {
        if(GameObject.Find("Canvas").transform.Find("menuButton") != null) GameObject.Find("Canvas").transform.Find("menuButton").gameObject.SetActive(setActive);
        if(GameObject.Find("Canvas").transform.Find("nextButton") != null) GameObject.Find("Canvas").transform.Find("nextButton").gameObject.SetActive(setActive);
        if(GameObject.Find("Canvas").transform.Find("Image") != null) GameObject.Find("Canvas").transform.Find("Image").gameObject.SetActive(setActive);
        if(GameObject.Find("Canvas").transform.Find("time") != null) GameObject.Find("Canvas").transform.Find("time").gameObject.SetActive(setActive);
        if(GameObject.Find("Canvas").transform.Find("fatigueText") != null) GameObject.Find("Canvas").transform.Find("fatigueText").gameObject.SetActive(setActive);
        if(GameObject.Find("Canvas").transform.Find("fatigueBar") != null) GameObject.Find("Canvas").transform.Find("fatigueBar").gameObject.SetActive(setActive);
    }
    
    public string CallRandomEvent(string[] randomrequiredEvent)
    {
        System.Random random = new System.Random();
        int randomIndex = random.Next(0, randomrequiredEvent.Length);
        Debug.Log("Random EventCode: " + randomrequiredEvent[randomIndex]);
        return randomrequiredEvent[randomIndex];
    }

    public string CallRandomEventAddNone(string[] randomrequiredEvent)
    {
        System.Random random = new System.Random();
        int randomIndex = random.Next(0, randomrequiredEvent.Length);
        Debug.Log("Random EventCode: " + randomrequiredEvent[randomIndex]);


        // 50%確率でイベントを決める
        int randomValue = random.Next(0, 2);
        Debug.Log("randomValue: " + randomValue);
        if(randomValue == 0)
        {
            return "NO";
        }
        else
        {
            return randomrequiredEvent[randomIndex];
        }
    }
}
