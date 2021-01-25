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
    public string[] morningrequiredEvent = {"EV001","EV002","EV003"};
    // Start is called before the first frame update
    void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;
        nextButton = GameObject.Find("Canvas").transform.Find("nextButton").GetComponent<Button>();
        nextButton.onClick.AddListener(ClickNextButton);

        // Panelを除いたUI Display off
        FacilityUISetActive(false);

        // ランダムで朝のイベント
        string morningEventCode = CallMorningReqEvent(morningrequiredEvent);
        EventListData[] loadedEventListData = playerSaveDataManager.LoadedEventListData();
        EventListData eventItem = eventManager.FindEventByCode(loadedEventListData, morningEventCode);
        List<string[]>scriptList = eventManager.ScriptSaveToList(eventItem);
        chatManager.ShowDialogue(scriptList, morningEventCode);
        
        PlayerData playerData = playerSaveDataManager.LoadPlayerData();
        Debug.Log("playerData: " + playerData.ToString());

        // UI setting
        GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text = playerData.time;
        GameObject.Find("Canvas").transform.Find("fatigueBar").GetComponent<Slider>().value = playerData.fatigue;


    }

    public void ClickNextButton()
    {
        string timeStr = GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text;
        Debug.Log(timeStr);
        //時間による次のイベント(switch)
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
    
    public string CallMorningReqEvent(string[] morningrequiredEvent)
    {
        System.Random random = new System.Random();
        int randomIndex = random.Next(0, morningrequiredEvent.Length);
        Debug.Log("Random morning EventCode: " + morningrequiredEvent[randomIndex]);
        return morningrequiredEvent[randomIndex];
    }
}
