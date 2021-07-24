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

    // Start is called before the first frame update
    void Start()
    {
        statusInitVar = GameObject.Find("StatusInitVar").GetComponent("StatusInitVar") as StatusInitVar;
        playTimeManager = GameObject.Find("PlayTimeManager").GetComponent("PlayTimeManager") as PlayTimeManager;
        playerSaveDataManager = new PlayerSaveDataManager();

        statusInitVar.closeButtonGameObj.GetComponent<Button>().onClick.AddListener(() => ClickCloseButton());
    }

    private void Update()
    {
        statusInitVar.playTimeValue.GetComponent<Text>().text = TimeSpan.FromSeconds(playTimeManager.playTime).ToString("hh':'mm':'ss");
    }


    public void ClickCloseButton()
    {
        SceneManager.UnloadSceneAsync("StatusScene");
    }
}
