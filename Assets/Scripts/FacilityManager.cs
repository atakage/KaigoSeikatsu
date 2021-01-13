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
    public string[] morningrequiredEvent = {"EV001","EV002","EV003"};
    // Start is called before the first frame update
    void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;

        // Panelを除いたUI Display off
        GameObject.Find("Canvas").transform.Find("menuButton").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Image").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("time").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("fatigueText").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("fatigueBar").gameObject.SetActive(false);

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
    
    public string CallMorningReqEvent(string[] morningrequiredEvent)
    {
        System.Random random = new System.Random();
        int randomIndex = random.Next(0, morningrequiredEvent.Length);
        Debug.Log("Random morning EventCode: " + morningrequiredEvent[randomIndex]);
        return morningrequiredEvent[randomIndex];
    }
}
