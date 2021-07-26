using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class StatusManager : MonoBehaviour
{
    StatusInitVar statusInitVar;
    PlayTimeManager playTimeManager;
    PlayerSaveDataManager playerSaveDataManager;
    PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {
        statusInitVar = GameObject.Find("StatusInitVar").GetComponent("StatusInitVar") as StatusInitVar;
        playTimeManager = GameObject.Find("PlayTimeManager").GetComponent("PlayTimeManager") as PlayTimeManager;
        playerSaveDataManager = new PlayerSaveDataManager();

        playerData = playerSaveDataManager.LoadPlayerData();
        statusInitVar.closeButtonGameObj.GetComponent<Button>().onClick.AddListener(() => ClickCloseButton());
    }

    private void Update()
    {
        statusInitVar.playTimeValue.GetComponent<Text>().text = TimeSpan.FromSeconds(playTimeManager.playTime).ToString("hh':'mm':'ss");
        statusInitVar.progressValueGameObj.GetComponent<Text>().text = playerData.progress.ToString();
        statusInitVar.fatigueValueGameObj.GetComponent<Text>().text =  Math.Truncate(playerData.fatigue).ToString();
        statusInitVar.satisfactionValueGameObj.GetComponent<Text>().text = playerData.satisfaction.ToString();
        statusInitVar.feelingValueGameObj.GetComponent<Text>().text = playerData.feeling.ToString();
        statusInitVar.moneyValueGameObj.GetComponent<Text>().text = playerData.money;
    }


    public void ClickCloseButton()
    {
        SceneManager.UnloadSceneAsync("StatusScene");
    }
}
