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
    GameObject playerName;
    // Start is called before the first frame update
    void Start()
    {
        statusInitVar = GameObject.Find("StatusInitVar").GetComponent("StatusInitVar") as StatusInitVar;
        playTimeManager = GameObject.Find("PlayTimeManager").GetComponent("PlayTimeManager") as PlayTimeManager;
        playerSaveDataManager = new PlayerSaveDataManager();

        playerData = playerSaveDataManager.LoadPlayerData();
        statusInitVar.closeButtonGameObj.GetComponent<Button>().onClick.AddListener(() => ClickCloseButton());
        playerName = statusInitVar.playerNameBoxGameObj.transform.Find("Text").gameObject;
    }

    private void Update()
    {
        playerName.GetComponent<Text>().text = playerData.name;
        statusInitVar.playTimeValue.GetComponent<Text>().text = TimeSpan.FromSeconds(playTimeManager.playTime).ToString("hh':'mm':'ss");
        statusInitVar.progressValueGameObj.GetComponent<Text>().text = playerData.progress.ToString() + "%";
        statusInitVar.progressBarGameObj.GetComponent<Slider>().value = playerData.progress;
        statusInitVar.fatigueValueGameObj.GetComponent<Text>().text =  Math.Truncate(playerData.fatigue).ToString();
        statusInitVar.satisfactionValueGameObj.GetComponent<Text>().text = playerData.satisfaction.ToString();
        statusInitVar.feelingValueGameObj.GetComponent<Text>().text = playerData.feeling.ToString();
        statusInitVar.moneyValueGameObj.GetComponent<Text>().text = playerData.money+"円";
    }


    public void ClickCloseButton()
    {
        SceneManager.UnloadSceneAsync("StatusScene");
    }
}
