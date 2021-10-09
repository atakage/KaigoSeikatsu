﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    SceneTransitionManager sceneTransitionManager;
    PlayerSaveDataManager playerSaveDataManager;
    MenuInitVar menuInitVar;
    PlayTimeManager playTimeManager;
    //string goBackScene;
    // Start is called before the first frame update
    void Start()
    {
        sceneTransitionManager = new SceneTransitionManager();
        playerSaveDataManager = new PlayerSaveDataManager();
        menuInitVar = GameObject.Find("MenuInitVar").GetComponent("MenuInitVar") as MenuInitVar;
        playTimeManager = GameObject.Find("PlayTimeManager").GetComponent("PlayTimeManager") as PlayTimeManager;

        PlayerData playerData = playerSaveDataManager.LoadPlayerData();

            // 戻るボタンの目的地を設定
        if (GameObject.Find("SceneChangeManager") != null)
        {
            //goBackScene = GameObject.Find("SceneChangeManager").transform.Find("SceneChangeCanvas").transform.Find("destinationFrom-toItemCheckScene").GetComponent<Text>().text;
        }

        menuInitVar.menuGridGameObj.transform.Find("statusButton").GetComponent<Button>().onClick.AddListener(() => ClickStatusButton());
        menuInitVar.menuGridGameObj.transform.Find("titleButton").GetComponent<Button>().onClick.AddListener(() => ClickTitleButton());
        menuInitVar.menuGridGameObj.transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(() => ClickCloseButton(playerData.currentScene));
        menuInitVar.titleReturnAlertBoxGameObj.transform.Find("confirmButton").GetComponent<Button>().onClick.AddListener(ClickTitleReturnConfirmBtn);
        menuInitVar.titleReturnAlertBoxGameObj.transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(ClickTitleReturnCancelBtn);
    }

    public void ClickTitleReturnCancelBtn()
    {
        menuInitVar.menuGridGameObj.SetActive(true);
        menuInitVar.titleReturnAlertBoxGameObj.SetActive(false);
    }

    public void ClickTitleReturnConfirmBtn()
    {
        playTimeManager.SavePlayTimeToPlayerDataJsonFile();
        sceneTransitionManager.LoadTo("TitleScene");
    }

    public void ClickTitleButton()
    {
        menuInitVar.menuGridGameObj.SetActive(false);
        menuInitVar.titleReturnAlertBoxGameObj.SetActive(true);
    }

    public void ClickStatusButton()
    {
        SceneManager.LoadScene("StatusScene", LoadSceneMode.Additive);
    }

    public void ClickCloseButton(string goBackScene)
    {
        GameObject loadValueSW = new GameObject("loadValueSW");
        loadValueSW.AddComponent<Text>();
        loadValueSW.transform.GetComponent<Text>().text = "Y";
        DontDestroyOnLoad(loadValueSW);

        sceneTransitionManager.LoadTo(goBackScene);
    }
}
