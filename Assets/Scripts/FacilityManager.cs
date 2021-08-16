using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    public Button menuButton;
    public Button useItemButton;
    public Button nextButton;
    public PlayerData playerData = null;
    public GameObject canvasObj;
    public GameObject FadeRefObj;
    public string[] morningrequiredEvent = null;
    //public string[] careQuizEvent = null;
    public string[] lunchEvent = null;
    public string[] recreationEvent = null;
    public string[] afternoonEvent = null;
    public bool timeCheckSW;
    public bool completeMainEvent;
    public string callEventFlag;
    public bool jobEventSearchSkip;
    public bool jobEventDayCompletedBool;
    public string loadValueSW;
    

    private string defaultCharFileName = "toyota";
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

        // TitleSceneからロードした時やMenuSceneからもどる時についてくるvalue
        if (GameObject.Find("loadValueSW") != null) loadValueSW = GameObject.Find("loadValueSW").transform.GetComponent<Text>().text;
        else loadValueSW = "N";

        if (GameObject.Find("SceneChangeManager") != null) GameObject.Find("SceneChangeManager").transform.Find("SceneChangeCanvas").transform.Find("destinationFrom-toItemCheckScene").GetComponent<Text>().text = SceneManager.GetActiveScene().name;

        canvasObj = GameObject.Find("Canvas");
        FadeRefObj = GameObject.Find("FadeInOutRefObject");
        nextButton = canvasObj.transform.Find("nextButton").GetComponent<Button>();
        nextButton.onClick.AddListener(ClickNextButton);
        menuButton = canvasObj.transform.Find("menuButton").GetComponent<Button>();
        menuButton.onClick.AddListener(ClickMenuButton);
        useItemButton = canvasObj.transform.Find("useItemButton").GetComponent<Button>();
        useItemButton.onClick.AddListener(() => ClickUseItemButton());

        playerData = playerSaveDataManager.LoadPlayerData();
        completeMainEvent = playerData.flag.completeMainEvent;
        jobEventDayCompletedBool = playerData.flag.jobEventDayCompletedBool;

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
        if (playerData.time.Equals("08:50"))
        {
                  // 現在プレイヤーデータの時間を変更する(add minute)
            DateTime addedDateTime = utilManager.TimeCal(playerData.time, 10);
            playerData.time = addedDateTime.Hour.ToString("D2") + ":" + addedDateTime.Minute.ToString("D2");
            playerSaveDataManager.SavePlayerData(playerData);
            string morningEventCode = CallRandomEvent(morningrequiredEvent);
            LoadEventAndShow(morningEventCode);
        }
        else
        {
            FacilityUISetActive(true);
        }
        
        // UI setting
        canvasObj.transform.Find("time").GetComponent<Text>().text = playerData.time;
        canvasObj.transform.Find("fatigueBar").GetComponent<Slider>().value = playerData.fatigue;

        // defaultCharacterImageSetting
        SetDefaultCharacterImage();
       
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
                //chatManager.SetTime(); // 時間がすぎる
                chatManager.DestroyMainEventBlackBox();
                Destroy(canvasObj.transform.Find("mainEventCompleteSW").gameObject);
            }
            // jobEventが終わった場合
            else if (canvasObj.transform.Find("jobEventCompleteSW") != null
                && canvasObj.transform.Find("onlyScriptEventEnd") != null
                && canvasObj.transform.Find("jobEventCompleteSW").GetComponent<Text>().text.Equals("Y")
                && canvasObj.transform.Find("onlyScriptEventEnd").GetComponent<Text>().text.Equals("END"))
            {
                Debug.Log("ENDED JOBEVENT CLICK");
                // 初期化
                canvasObj.transform.Find("jobEventCompleteSW").GetComponent<Text>().text = "N";
                canvasObj.transform.Find("onlyScriptEventEnd").GetComponent<Text>().text = "";

                // UI時間更新
                chatManager.SetTime();
                // fade out 
                chatManager.executeFadeOutSimple();
                // uiをActive
            }

            if ((timeStr.Equals("09:00")
                && canvasObj.transform.Find("fadeOutEndMomentSW") != null
                && canvasObj.transform.Find("fadeOutEndMomentSW").GetComponent<Text>().text.Equals("Y"))
                || (timeStr.Equals("09:00") && loadValueSW.Equals("Y")))
            {
                SetPanelText("今日も頑張ろう");
                ActiveDefaultCharacterImage(true);
            }
            else if ((timeStr.Equals("11:50")
                && canvasObj.transform.Find("fadeOutEndMomentSW") != null
                && canvasObj.transform.Find("fadeOutEndMomentSW").GetComponent<Text>().text.Equals("Y"))
                || (timeStr.Equals("11:50") && loadValueSW.Equals("Y")))
            {
                SetPanelText("もうすぐお昼の時間だ");
                ActiveDefaultCharacterImage(true);
            }
            else if ((timeStr.Equals("12:50")
                && canvasObj.transform.Find("fadeOutEndMomentSW") != null
                && canvasObj.transform.Find("fadeOutEndMomentSW").GetComponent<Text>().text.Equals("Y"))
                || (timeStr.Equals("12:50") && loadValueSW.Equals("Y")))
            {
                SetPanelText("休憩時間だ\n何をしようかな?");
                ActiveDefaultCharacterImage(true);
            }
            else if ((timeStr.Equals("14:00")
                && canvasObj.transform.Find("fadeOutEndMomentSW") != null
                && canvasObj.transform.Find("fadeOutEndMomentSW").GetComponent<Text>().text.Equals("Y"))
                || (timeStr.Equals("14:00") && loadValueSW.Equals("Y")))
            {
                SetPanelText("まもなく午後のスケジュールが始まる");
                ActiveDefaultCharacterImage(true);
            }
            else if ((timeStr.Equals("17:00")
                && canvasObj.transform.Find("fadeOutEndMomentSW") != null
                && canvasObj.transform.Find("fadeOutEndMomentSW").GetComponent<Text>().text.Equals("Y"))
                || (timeStr.Equals("17:00") && loadValueSW.Equals("Y")))
            {
                SetPanelText("午後のスケジュールが終わった");
                ActiveDefaultCharacterImage(true);
            }
            else if ((timeStr.Equals("17:20")
                && canvasObj.transform.Find("fadeOutEndMomentSW") != null
                && canvasObj.transform.Find("fadeOutEndMomentSW").GetComponent<Text>().text.Equals("Y"))
                || (timeStr.Equals("17:20") && loadValueSW.Equals("Y")))
            {
                SetPanelText("これからどうする?");
                SetGoToButton(true);
                ActiveDefaultCharacterImage(false);
                canvasObj.transform.Find("nextButton").transform.Find("Text").GetComponent<Text>().text = "帰宅";
                //timeCheckSW = false;
            }
            //  fade out後 17:20 -> 家へ
            else if (canvasObj.transform.Find("fadeOutPersistEventCheck") != null
                && canvasObj.transform.Find("fadeOutPersistEventCheck").GetComponent<Text>().text.Equals("Y") &&
                canvasObj.transform.Find("AlertGoing").transform.Find("DestinationValue").GetComponent<Text>().text.Equals("帰宅"))
            {
                playerData.time = canvasObj.transform.Find("time").GetComponent<Text>().text;
                playerData.currentScene = "AtHomeScene";
                playerSaveDataManager.SavePlayerData(playerData);
                sceneTransitionManager.LoadTo("AtHomeScene");
            }
            // fade out後 17:20 -> カフェへ
            else if (canvasObj.transform.Find("fadeOutPersistEventCheck") != null
                && canvasObj.transform.Find("fadeOutPersistEventCheck").GetComponent<Text>().text.Equals("Y") &&
                canvasObj.transform.Find("AlertGoing").transform.Find("DestinationValue").GetComponent<Text>().text.Equals("カフェ"))
            {
                playerData.time = canvasObj.transform.Find("time").GetComponent<Text>().text;
                playerData.currentScene = "CafeScene";
                playerSaveDataManager.SavePlayerData(playerData);
                sceneTransitionManager.LoadTo("CafeScene");
            }
            //  fade out後 17:20 -> 公園へ
            else if (canvasObj.transform.Find("fadeOutPersistEventCheck") != null
                && canvasObj.transform.Find("fadeOutPersistEventCheck").GetComponent<Text>().text.Equals("Y") &&
                canvasObj.transform.Find("AlertGoing").transform.Find("DestinationValue").GetComponent<Text>().text.Equals("公園"))
            {
                playerData.time = canvasObj.transform.Find("time").GetComponent<Text>().text;
                playerData.currentScene = "ParkScene";
                playerSaveDataManager.SavePlayerData(playerData);
                sceneTransitionManager.LoadTo("ParkScene");
            }
            // game end
            else if (FadeRefObj.transform.Find("ChangeSceneName") != null
                  && FadeRefObj.transform.Find("ChangeSceneFadeInOutManagerSW") != null
                  && FadeRefObj.transform.Find("ChangeSceneFadeInOutManagerSW").GetComponent<Text>().text.Equals("Y"))
            {
                // ReadyForEndingScene移動
                string changeSceneName = FadeRefObj.transform.Find("ChangeSceneName").GetComponent<Text>().text;
                sceneTransitionManager.LoadTo(changeSceneName);
            }

            // UI初期化---------------------------------------------------------------------------------------------------------------
            // fade out後menuButtonとnextButtonのinteractableをtrue
            if (canvasObj.transform.Find("fadeOutEndMomentSW") != null
                && canvasObj.transform.Find("fadeOutEndMomentSW").GetComponent<Text>().text.Equals("Y"))
            {
                FacilityUISetActive(true);
                menuBtnAndNextBtnInteractable(true);

            }

            // 初期化------------------------------------------------------------------------------------------------------------------
            if (canvasObj.transform.Find("fadeOutEndMomentSW") != null)
            {
                canvasObj.transform.Find("fadeOutEndMomentSW").GetComponent<Text>().text = "N";
                //Destroy(canvasObj.transform.Find("fadeOutEndMomentSW").gameObject);
            }
            if (loadValueSW.Equals("Y"))
            {
                loadValueSW = "N";
                if (GameObject.Find("loadValueSW") != null) Destroy(GameObject.Find("loadValueSW"));
            }
        }
        
        
    }

    public void SetDefaultCharacterImage()
    {
        canvasObj.transform.Find("charaterImageBox").transform.Find("defaultCharacterImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("img/character/" + defaultCharFileName);
        canvasObj.transform.Find("charaterImageBox").transform.Find("defaultCharacterImage").GetComponent<Image>().color = new Color(255, 255, 255, 255);
    }

    public void ActiveDefaultCharacterImage(bool sw)
    {
        canvasObj.transform.Find("charaterImageBox").transform.Find("defaultCharacterImage").gameObject.SetActive(sw);
    }

    public void ClickUseItemButton()
    {
        sceneTransitionManager.LoadTo("ItemCheckScene");
    }

    public void ClickMenuButton()
    {
        // Scene転換後戻るためのsceneNameを指定
        SetSceneDestinationValue();

        sceneTransitionManager.LoadTo("MenuScene");
    }

    public void SetSceneDestinationValue()
    {
        // 戻るボタンの目的地を設定
        if (GameObject.Find("SceneChangeManager") != null)
        {
            GameObject.Find("SceneChangeManager").transform.Find("SceneChangeCanvas").transform.Find("destinationFrom-toItemCheckScene").GetComponent<Text>().text = SceneManager.GetActiveScene().name;
        }
    }

    public void ClickNextButton()
    {
        string timeStr = canvasObj.transform.Find("time").GetComponent<Text>().text;
        string nextBtnText = GameObject.Find("nextButton").transform.Find("Text").GetComponent<Text>().text;

        // nextButtonのテキストが進行なら
        if (nextBtnText.Equals("進行"))
        {
            //時間による次のイベント(switch)
            switch (timeStr)
            {
                // ９時なら(-> 11:50)
                case "09:00":
                    SetPanelText("");

                              // 発動できるメインイベントがあるならメインイベントを先にする
                    if (completeMainEvent != true)
                    {
                        completeMainEvent = RunMainEvent();
                                    // メインイベントを実行したらこの時間にはjobEventを探さない
                        if (completeMainEvent == true)
                        {
                            chatManager.SetTime();
                            jobEventSearchSkip = true;
                        }         
                    }

                               // メインイベントが実行されなかった場合
                    if(jobEventSearchSkip == false)
                    {
                                    // 前の時間にjobEventが実行されたらもう実行させない
                        if(jobEventDayCompletedBool == true)
                        {
                            // fade out
                            FacilityUISetActive(false);
                            chatManager.executeFadeOutSimple();
                            chatManager.SetTime();
                        }
                        else
                        {
                                          // jobEventを発動させるかを決める
                            callEventFlag = utilManager.GetYesOrNo();// 'YES' or 'NO'

                            switch (callEventFlag)
                            {
                                case "YES":
                                    // jobEventの中でactiveされているイベントをランダムで呼び出す
                                    JobEventModel[] jobeventModelArray = jobEventSetManager.GetJobEventJsonFile();
                                    JobEventModel jobEvent = jobEventManager.GetActiveJobEventRandom(jobeventModelArray);

                                    if (jobEvent != null)
                                    {
                                        LoadJobEventAndShow(jobEvent);
                                        menuBtnAndNextBtnInteractable(false);
                                                            // 他の時間にjobEventが発動しないようにフラグ処理
                                        jobEventDayCompletedBool = true;

                                        playerData = playerSaveDataManager.LoadPlayerData();
                                        playerData.flag.jobEventDayCompletedBool = jobEventDayCompletedBool;
                                        playerSaveDataManager.SavePlayerData(playerData);
                                    }
                                    else
                                    {
                                        // fade out
                                        FacilityUISetActive(false);
                                        chatManager.executeFadeOutSimple();
                                        chatManager.SetTime();
                                    }
                                    break;
                                case "NO":
                                    // fade out
                                    FacilityUISetActive(false);
                                    chatManager.executeFadeOutSimple();
                                    chatManager.SetTime();
                                    break;
                            }
                        }
                    }

                    // 次の時間にjobEventを探せるように初期化
                    jobEventSearchSkip = false;

                              // ゲームロードをしながらイベントを繰り返す行為を防ぐために
                              // プレイヤーデータに時間をアプデ
                    playerData = playerSaveDataManager.LoadPlayerData();;
                    playerData.time = "11:50";
                    playerSaveDataManager.SavePlayerData(playerData);

                    break;

                        // 11時なら食事に (-> 12:00(昼ご飯) -> 12:50)
                case "11:50":
                               // 発動できるメインイベントがあるならメインイベントを先にする
                    if (completeMainEvent != true)
                    {
                        completeMainEvent = RunMainEvent();
                                     // メインイベントを実行したらこの時間にはjobEventを探さない
                        if (completeMainEvent == true)
                        {
                            chatManager.SetTime();
                            jobEventSearchSkip = true;
                        }
                            
                    }

                               // メインイベントが実行されなかった場合
                    if (jobEventSearchSkip == false)
                    {
                                     // 前の時間にjobEventが実行されたらもう実行させない
                        if (jobEventDayCompletedBool == true)
                        {
                            // fade out
                            FacilityUISetActive(false);
                            LoadEventAndShow(CallRandomEvent(lunchEvent));
                            chatManager.SetTime();
                        }
                        else
                        {
                                           // jobEventを発動させるかを決める
                            callEventFlag = utilManager.GetYesOrNo();// 'YES' or 'NO'

                            switch (callEventFlag)
                            {
                                case "YES":
                                    // jobEventの中でactiveされているイベントをランダムで呼び出す
                                    JobEventModel[] jobeventModelArray = jobEventSetManager.GetJobEventJsonFile();
                                    JobEventModel jobEvent = jobEventManager.GetActiveJobEventRandom(jobeventModelArray);

                                    if (jobEvent != null)
                                    {
                                        LoadJobEventAndShow(jobEvent);
                                        menuBtnAndNextBtnInteractable(false);
                                                            // 他の時間にjobEventが発動しないようにフラグ処理
                                        jobEventDayCompletedBool = true;

                                        playerData = playerSaveDataManager.LoadPlayerData();
                                        playerData.flag.jobEventDayCompletedBool = jobEventDayCompletedBool;
                                        playerSaveDataManager.SavePlayerData(playerData);
                                    }
                                    else
                                    {
                                        // fade out
                                        FacilityUISetActive(false);
                                        LoadEventAndShow(CallRandomEvent(lunchEvent));
                                        chatManager.SetTime();
                                    }
                                    break;
                                case "NO":
                                    // fade out
                                    FacilityUISetActive(false);
                                    LoadEventAndShow(CallRandomEvent(lunchEvent));
                                    chatManager.SetTime();
                                    break;
                            }
                        }
                    }

                    // 次の時間にjobEventを探せるように初期化
                    jobEventSearchSkip = false;

                              // ゲームロードをしながらイベントを繰り返す行為を防ぐために
                              // プレイヤーデータに時間をアプデ
                    playerData = playerSaveDataManager.LoadPlayerData();
                    playerData.time = "12:50";
                    playerSaveDataManager.SavePlayerData(playerData);

                    break;

                // 12:50なら休憩時間( -> 14:00)
                case "12:50":
                    SetPanelText("");
                               // 発動できるメインイベントがあるならメインイベントを先にする
                    if (completeMainEvent != true)
                    {
                        completeMainEvent = RunMainEvent();
                                    // メインイベントを実行したらこの時間にはjobEventを探さない
                        if (completeMainEvent == true)
                        {
                            chatManager.SetTime();
                            jobEventSearchSkip = true;
                        }
                    }

                              // メインイベントが実行されなかった場合
                    if (jobEventSearchSkip == false)
                    {
                                     // 前の時間にjobEventが実行されたらもう実行させない
                        if (jobEventDayCompletedBool == true)
                        {
                            // fade out
                            FacilityUISetActive(false);
                            chatManager.executeFadeOutSimple();
                            chatManager.SetTime();
                        }
                        else
                        {
                                          // jobEventを発動させるかを決める
                            callEventFlag = utilManager.GetYesOrNo();// 'YES' or 'NO'

                            switch (callEventFlag)
                            {
                                case "YES":
                                    // jobEventの中でactiveされているイベントをランダムで呼び出す
                                    JobEventModel[] jobeventModelArray = jobEventSetManager.GetJobEventJsonFile();
                                    JobEventModel jobEvent = jobEventManager.GetActiveJobEventRandom(jobeventModelArray);

                                    if (jobEvent != null)
                                    {
                                        LoadJobEventAndShow(jobEvent);
                                        menuBtnAndNextBtnInteractable(false);
                                                            // 他の時間にjobEventが発動しないようにフラグ処理
                                        jobEventDayCompletedBool = true;

                                        playerData = playerSaveDataManager.LoadPlayerData();
                                        playerData.flag.jobEventDayCompletedBool = jobEventDayCompletedBool;
                                        playerSaveDataManager.SavePlayerData(playerData);
                                    }
                                    else
                                    {
                                        // fade out
                                        FacilityUISetActive(false);
                                        chatManager.executeFadeOutSimple();
                                        chatManager.SetTime();
                                    }
                                    break;
                                case "NO":
                                    // fade out
                                    FacilityUISetActive(false);
                                    chatManager.executeFadeOutSimple();
                                    chatManager.SetTime();
                                    break;
                            }
                        }
                    }

                    // 次の時間にjobEventを探せるように初期化
                    jobEventSearchSkip = false;

                              // ゲームロードをしながらイベントを繰り返す行為を防ぐために
                              // プレイヤーデータに時間をアプデ
                    playerData = playerSaveDataManager.LoadPlayerData();
                    playerData.time = "14:00";
                    playerSaveDataManager.SavePlayerData(playerData);

                    break;

                        // 14時ならレクリエーションの時間( -> 17:00)
                case "14:00":
                              // 発動できるメインイベントがあるならメインイベントを先にする
                    if (playerData.flag.completeMainEvent != true)
                    {
                        completeMainEvent = RunMainEvent();
                                    // メインイベントを実行したらこの時間にはjobEventを探さない
                        if (completeMainEvent == true)
                        {
                            chatManager.SetTime();
                            jobEventSearchSkip = true;
                        }
                    }

                              // メインイベントが実行されなかった場合
                    if (jobEventSearchSkip == false)
                    {
                                    // 前の時間にjobEventが実行されたらもう実行させない
                        if (jobEventDayCompletedBool == true)
                        {
                            // fade out
                            FacilityUISetActive(false);
                            LoadEventAndShow(CallRandomEvent(recreationEvent));
                            chatManager.SetTime();
                        }
                        else
                        {
                                          // jobEventを発動させるかを決める
                            callEventFlag = utilManager.GetYesOrNo();// 'YES' or 'NO'

                            switch (callEventFlag)
                            {
                                case "YES":
                                    // jobEventの中でactiveされているイベントをランダムで呼び出す
                                    JobEventModel[] jobeventModelArray = jobEventSetManager.GetJobEventJsonFile();
                                    JobEventModel jobEvent = jobEventManager.GetActiveJobEventRandom(jobeventModelArray);

                                    if (jobEvent != null)
                                    {
                                        LoadJobEventAndShow(jobEvent);
                                        menuBtnAndNextBtnInteractable(false);
                                                            // 他の時間にjobEventが発動しないようにフラグ処理
                                        jobEventDayCompletedBool = true;

                                        playerData = playerSaveDataManager.LoadPlayerData();
                                        playerData.flag.jobEventDayCompletedBool = jobEventDayCompletedBool;
                                        playerSaveDataManager.SavePlayerData(playerData);
                                    }
                                    else
                                    {
                                        // fade out
                                        FacilityUISetActive(false);
                                        LoadEventAndShow(CallRandomEvent(recreationEvent));
                                        chatManager.SetTime();
                                    }
                                    break;
                                case "NO":
                                    // fade out
                                    FacilityUISetActive(false);
                                    LoadEventAndShow(CallRandomEvent(recreationEvent));
                                    chatManager.SetTime();
                                    break;
                            }
                        }
                    }

                    // 次の時間にjobEventを探せるように初期化
                    jobEventSearchSkip = false;

                              // ゲームロードをしながらイベントを繰り返す行為を防ぐために
                              // プレイヤーデータに時間をアプデ
                    playerData = playerSaveDataManager.LoadPlayerData();
                    playerData.time = "17:00";
                    playerSaveDataManager.SavePlayerData(playerData);

                    break;

                        // 17時なら帰宅準備( -> 仕事終り)
                case "17:00":

                    FacilityUISetActive(false);
                    LoadEventAndShow(CallRandomEvent(afternoonEvent));

                    playerData = playerSaveDataManager.LoadPlayerData();

                              // event関連flag初期化
                    playerData.flag.completeMainEvent = false;
                    playerData.flag.jobEventSearchSkip = false;
                    playerData.flag.jobEventDayCompletedBool = false;

                    playerData.time = "17:20";
                    playerSaveDataManager.SavePlayerData(playerData);

                               // 時間が経つ
                    chatManager.SetTime();

                    break;
            }
        }
            // nextButtonのテキストが進行がないなら帰宅する
        else
        {
            ClickGoToHomeButton();
        }
    }

    // 17:20 -> 18:00
    public void ClickGoToAlertYesButton()
    {
        // プレイヤーデータに時間をセーブ
        PlayerData playerData = playerSaveDataManager.LoadPlayerData();
        playerData.time = "18:00";
        //playerSaveDataManager.SavePlayerData(playerData);

        canvasObj.transform.Find("AlertGoing").gameObject.SetActive(false);
        canvasObj.transform.Find("Panel").gameObject.SetActive(false);
        FacilityUISetActive(false);
        //chatManager.executeFadeOutSimple();
        chatManager.executeFadeOutPersist();
        chatManager.SetTime();
        timeCheckSW = true;
    }

    public void ClickGoToAlertNoButton()
    {
        canvasObj.transform.Find("AlertGoing").gameObject.SetActive(false);
        canvasObj.transform.Find("GoToCafeButton").gameObject.SetActive(true);
        canvasObj.transform.Find("GoToParkButton").gameObject.SetActive(true);
        canvasObj.transform.Find("menuButton").GetComponent<Button>().interactable = true;
        canvasObj.transform.Find("nextButton").GetComponent<Button>().interactable = true;
        canvasObj.transform.Find("useItemButton").GetComponent<Button>().interactable = true;
    }

    public void ClickGoToHomeButton()
    {
        SetGoToButton(false);
        canvasObj.transform.Find("AlertGoing").gameObject.SetActive(true);
        GameObject.Find("AlertGoing").transform.Find("DestinationValue").gameObject.SetActive(false);
        GameObject.Find("AlertGoing").transform.Find("alertMessage").GetComponent<Text>().text =
        "<color=#f54242>" + "帰宅" + "</color>"  + "しますか?";
        GameObject.Find("AlertGoing").transform.Find("DestinationValue").GetComponent<Text>().text = "帰宅";
        menuBtnAndNextBtnInteractable(false);
    }

    public void ClickGoToButton(string destination)
    {
        SetGoToButton(false);
        canvasObj.transform.Find("AlertGoing").gameObject.SetActive(true);
        GameObject.Find("AlertGoing").transform.Find("DestinationValue").gameObject.SetActive(false);
        GameObject.Find("AlertGoing").transform.Find("alertMessage").GetComponent<Text>().text =
        "<color=#f54242>" + destination + "</color>" + "に行って時間を過ごしますか?";
        GameObject.Find("AlertGoing").transform.Find("DestinationValue").GetComponent<Text>().text = destination;
        menuBtnAndNextBtnInteractable(false);
    }

    public void SetGoToButton(bool sw)
    {
        if (sw)
        {
            canvasObj.transform.Find("GoToCafeButton").gameObject.SetActive(sw);
            canvasObj.transform.Find("GoToParkButton").gameObject.SetActive(sw);
        }
        else
        {
            canvasObj.transform.Find("GoToCafeButton").gameObject.SetActive(sw);
            canvasObj.transform.Find("GoToParkButton").gameObject.SetActive(sw);
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

                chatManager.ShowDialogueForMainEvent(scriptList, mainEventCode, eventItem.script);

                playerData = playerSaveDataManager.LoadPlayerData();
                // addingprogress
                int addingProgress = mainEventManager.getAddingProgressFromMainEventJsonFile(mainEventCode);
                playerData.progress += addingProgress;

                // 終わったMainEventはプレイヤーデータに記録する
                string[] mainEventCodeArray = playerSaveDataManager.SaveCompletedEvent(playerData.eventCodeObject.completedMainEventArray, mainEventCode);
                playerData.eventCodeObject.completedMainEventArray = mainEventCodeArray;
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

        playerData = playerSaveDataManager.LoadPlayerData();
        playerData.flag.completeMainEvent = returnValue;
        playerSaveDataManager.SavePlayerData(playerData);

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
        // 2021.07.26 修正, キャライメージ追加されたrawScriptをparameterに渡す
        chatManager.ShowDialogue(scriptList, eventCode, eventItem.script);
    }

    public void menuBtnAndNextBtnInteractable(bool sw)
    {
        canvasObj.transform.Find("menuButton").GetComponent<Button>().interactable = sw;
        canvasObj.transform.Find("nextButton").GetComponent<Button>().interactable = sw;
        canvasObj.transform.Find("useItemButton").GetComponent<Button>().interactable = sw;
    }

    public void FacilityUISetActive(bool setActive)
    {
        if(GameObject.Find("Canvas").transform.Find("menuButton") != null) GameObject.Find("Canvas").transform.Find("menuButton").gameObject.SetActive(setActive);
        if(GameObject.Find("Canvas").transform.Find("nextButton") != null) GameObject.Find("Canvas").transform.Find("nextButton").gameObject.SetActive(setActive);
        if(GameObject.Find("Canvas").transform.Find("useItemButton") != null) GameObject.Find("Canvas").transform.Find("useItemButton").gameObject.SetActive(setActive);
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
