using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public PlayerSaveDataManager playerSaveDataManager;
    public EventListData[] eventListData;
    // Start is called before the first frame update
    void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventListData = new EventListData[4];

        // 最初はすべてのeventを登録  
        eventListData[0] = new EventListData();
        eventListData[0].eventCode = "EE00";
        eventListData[0].script = "EESCRIPT";
        eventListData[1] = new EventListData();
        eventListData[1].eventCode = "EE01";
        eventListData[1].script = "EESCIPRT222modify";
        eventListData[2] = new EventListData();
        eventListData[2].eventCode = "EE03";
        eventListData[2].script = "EESCIPRT3333333modify";
        eventListData[3] = new EventListData();
        eventListData[3].eventCode = "EE04";
        eventListData[3].script = "EESCIPRT444";
        playerSaveDataManager.SaveEventListData(eventListData);

    }
}
