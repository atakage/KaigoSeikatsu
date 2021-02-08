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
    public Button nextButton;
    public string[] morningrequiredEvent = null;
    public string[] careQuizEvent = null;
    public string[] lunchEvent = null;
    public string[] recreationEvent = null;
    public bool timeCheckSW;
    // Start is called before the first frame update
    void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;
        nextButton = GameObject.Find("Canvas").transform.Find("nextButton").GetComponent<Button>();
        nextButton.onClick.AddListener(ClickNextButton);

        // イベントコードセット
        morningrequiredEvent = new string[]{ "EV001", "EV002", "EV003" };  // 08:00 ~ 09:00
        careQuizEvent = new string[]{ "ET000","NO"}; // 9:00 ~ 11:50
        lunchEvent = new string[] {"EV004"}; // 11:50 ~ 12:50
        recreationEvent = new string[] { }; // 14:00 ~ 16:00

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
        }
        
        
    }

    public void ClickNextButton()
    {
        string timeStr = GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text;
        Debug.Log(timeStr);
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
            //case "12:50":

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
