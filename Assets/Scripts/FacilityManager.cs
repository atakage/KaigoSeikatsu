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
    public Button nextButton;
    public string[] morningrequiredEvent = null;
    public string[] careQuizEvent = null;
    public string[] lunchEvent = null;
    public string[] recreationEvent = null;
    public string[] afternoonEvent = null;
    public bool timeCheckSW;
    // Start is called before the first frame update
    void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;
        sceneTransitionManager = new SceneTransitionManager();
        nextButton = GameObject.Find("Canvas").transform.Find("nextButton").GetComponent<Button>();
        nextButton.onClick.AddListener(ClickNextButton);
        GameObject.Find("Canvas").transform.Find("GoToCafeButton").GetComponent<Button>().onClick.AddListener(delegate { ClickGoToButton("カフェ"); });
        GameObject.Find("Canvas").transform.Find("GoToParkButton").GetComponent<Button>().onClick.AddListener(delegate { ClickGoToButton("公園"); });
        GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("No").GetComponent<Button>().onClick.AddListener(ClickGoToAlertNoButton);
        GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("Yes").GetComponent<Button>().onClick.AddListener(ClickGoToAlertYesButton);

        // イベントコードセット
        morningrequiredEvent = new string[]{ "EV001", "EV002", "EV003" };  // 08:00 ~ 09:00
        careQuizEvent = new string[]{ "ET000","NO"}; // 9:00 ~ 11:50
        lunchEvent = new string[] {"EV004"}; // 11:50 ~ 12:50
        recreationEvent = new string[] {"EV005","EV006","EV007" }; // 14:00 ~ 17:00
        afternoonEvent = new string[] {"EV008"}; // 17:00

        timeCheckSW = false;
        // Panelを除いたUI Display off
        FacilityUISetActive(false);

        // ランダムで朝のイベント
        string morningEventCode = CallRandomEvent(morningrequiredEvent);
        LoadEventAndShow(morningEventCode);

        PlayerData playerData = playerSaveDataManager.LoadPlayerData();
        Debug.Log("playerData: " + playerData.ToString());

        // UI setting
        GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text = playerData.time;
        GameObject.Find("Canvas").transform.Find("fatigueBar").GetComponent<Slider>().value = playerData.fatigue;


    }

    private void Update()
    {
        if (timeCheckSW)
        {
            string timeStr = GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text;

            if (timeStr.Equals("12:50") && GameObject.Find("Canvas").transform.Find("time").gameObject.activeInHierarchy)
            {
                SetPanelText("休憩時間だ\n何をしようかな?");
                timeCheckSW = false;
            }
            else if (timeStr.Equals("11:50") && GameObject.Find("Canvas").transform.Find("time").gameObject.activeInHierarchy)
            {
                SetPanelText("もうすぐお昼の時間だ");
                timeCheckSW = false;
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
            else if (GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("FadeSwitchText").GetComponent<Text>().text.Equals("call"))
            {
                Debug.Log("18:00");
                sceneTransitionManager.LoadTo("AtHomeScene");
            }
        }
        
        
    }

    public void ClickNextButton()
    {
        string timeStr = GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text;
        Debug.Log(timeStr);

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
                    string eventCode = CallRandomEvent(careQuizEvent);
                    if (!eventCode.Equals("NO"))
                    {
                        LoadEventAndShow(eventCode);
                    }
                    // クイズイベントなかったら時間がたつ(->11:50)
                    else
                    {
                        // fade out
                    }
                    timeCheckSW = true;
                    break;
                // 11時なら食事に (-> 12:00(昼ご飯) -> 12:50)
                case "11:50":
                    FacilityUISetActive(false);
                    eventCode = CallRandomEvent(lunchEvent);
                    LoadEventAndShow(eventCode);

                    // 時間が経つ
                    chatManager.SetTime();
                    timeCheckSW = true;
                    break;
                // 12:50なら休憩時間( -> 14:00)
                case "12:50":
                    FacilityUISetActive(false);
                    SetPanelText("");
                    chatManager.SetTime();
                    chatManager.executeFadeOut();
                    timeCheckSW = true;
                    break;
                // 14時ならレクリエーションの時間( -> 16:00)
                case "14:00":
                    FacilityUISetActive(false);
                    eventCode = CallRandomEvent(recreationEvent);
                    LoadEventAndShow(eventCode);

                    // 時間が経つ
                    chatManager.SetTime();
                    timeCheckSW = true;
                    break;
                // 17時なら帰宅準備( -> 仕事終り)
                case "17:00":
                    FacilityUISetActive(false);
                    eventCode = CallRandomEvent(afternoonEvent);
                    LoadEventAndShow(eventCode);

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

    public void LoadEventAndShow(string eventCode)
    {
        EventListData[] loadedEventListData = playerSaveDataManager.LoadedEventListData();
        EventListData eventItem = eventManager.FindEventByCode(loadedEventListData, eventCode);
        List<string[]> scriptList = eventManager.ScriptSaveToList(eventItem);
        chatManager.ShowDialogue(scriptList, eventCode);
    }

    public void FacilityUISetActive(bool setActive)
    {
        GameObject.Find("Canvas").transform.Find("menuButton").gameObject.SetActive(setActive);
        GameObject.Find("Canvas").transform.Find("nextButton").gameObject.SetActive(setActive);
        GameObject.Find("Canvas").transform.Find("Image").gameObject.SetActive(setActive);
        GameObject.Find("Canvas").transform.Find("time").gameObject.SetActive(setActive);
        GameObject.Find("Canvas").transform.Find("fatigueText").gameObject.SetActive(setActive);
        GameObject.Find("Canvas").transform.Find("fatigueBar").gameObject.SetActive(setActive);
    }
    
    public string CallRandomEvent(string[] randomrequiredEvent)
    {
        System.Random random = new System.Random();
        int randomIndex = random.Next(0, randomrequiredEvent.Length);
        Debug.Log("Random EventCode: " + randomrequiredEvent[randomIndex]);
        return randomrequiredEvent[randomIndex];
    }
}
