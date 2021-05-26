using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class AtHomeManager : MonoBehaviour
{
    public SceneTransitionManager sceneTransitionManager;
    public PlayerSaveDataManager playerSaveDataManager;
    public ConvenienceItemSetManager convenienceItemSetManager;
    public PlayerData playerData = null;
    public GameObject canvasGameObj;
    public Boolean timeCheckResult;
    private void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        sceneTransitionManager = new SceneTransitionManager();
        convenienceItemSetManager = new ConvenienceItemSetManager();

        canvasGameObj = GameObject.Find("Canvas").gameObject;

        playerData = playerSaveDataManager.LoadPlayerData();
        string time = playerData.time;
        canvasGameObj.transform.Find("time").GetComponent<Text>().text = time;

        // 現在外出禁止時間なのか確認する
        timeCheckResult = CheckBanTime(playerData.time);

        // 朝なら出勤する、夜なら寝るにボタン変更
        GameObject.Find("nextButton").transform.Find("Text").GetComponent<Text>().text 
                           = (time.Equals("08:00")) ? "出勤する" : "寝る";



        GameObject.Find("Canvas").transform.Find("nextButton").GetComponent<Button>().onClick.AddListener(ClickNextButton);
        GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("No").GetComponent<Button>().onClick.AddListener(delegate { ActiveAlert(false); });
        GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("Yes").GetComponent<Button>().onClick.AddListener(ClickGoToAlertYesButton);
        GameObject.Find("Canvas").transform.Find("itemCheckButton").GetComponent<Button>().onClick.AddListener(ClickItemCheckButton);
        GameObject.Find("Canvas").transform.Find("goOutButton").GetComponent<Button>().onClick.AddListener(delegate { ClickGoOutButton(time); });
        GameObject.Find("Canvas").transform.Find("GoOutBox").transform.Find("goToConvenienceButton").GetComponent<Button>().onClick.AddListener(ClickGoToConvenienceBtn);
        GameObject.Find("Canvas").transform.Find("GoOutBox").transform.Find("closeButton").GetComponent<Button>().onClick.AddListener(ClickGoOutCloseBtn);
        GameObject.Find("Canvas").transform.Find("GoOutAlertNoBox").transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(ClickGoOutCancelBtn);
    }

    private void Update()
    {
        // 出勤する
        if (GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("FadeSwitchText").GetComponent<Text>().text.Equals("call") &&
            GameObject.Find("Canvas").transform.Find("nextButton").transform.Find("Text").GetComponent<Text>().text.Equals("出勤する"))
        {
            // 次のシーンをプレイヤーデータにセーブ
            playerData.currentScene = "FacilityScene";
            playerData.time = "09:00";
            playerSaveDataManager.SavePlayerData(playerData);
            sceneTransitionManager.LoadTo("FacilityScene");
        // 寝て朝になる
        }else if (GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("FadeSwitchText").GetComponent<Text>().text.Equals("call") &&
                  GameObject.Find("Canvas").transform.Find("nextButton").transform.Find("Text").GetComponent<Text>().text.Equals("寝る"))
        {
            GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("FadeSwitchText").GetComponent<Text>().text = "";
            GameObject.Find("Canvas").transform.Find("nextButton").transform.Find("Text").GetComponent<Text>().text = "出勤する";
            playerData.time = "08:00";
            playerSaveDataManager.SavePlayerData(playerData);
            GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text = playerData.time;
            MenuButtonActive(true);
            UIDisplay(true);
        // コンビニへ行く
        }else if (GameObject.Find("Canvas").transform.Find("FadeCompleteValue").GetComponent<Text>().text.Equals("convenience"))
        {
            // 時間増加後scene転換
            string[] playerTimeArray = playerData.time.Split(':');
            int timeGoesMinuteInt = Int32.Parse(playerTimeArray[1]) + 10;
            // 次のシーンをプレイヤーデータにセーブ
            playerData.currentScene = "ConvenienceScene";
            playerData.time = playerTimeArray[0] + ":" + timeGoesMinuteInt.ToString();
            playerSaveDataManager.SavePlayerData(playerData);

            sceneTransitionManager.LoadTo("ConvenienceScene");
        }

        // 朝は外出禁止
        if ("08:00".Equals(playerData.time)) canvasGameObj.transform.Find("goOutButton").GetComponent<Button>().interactable = false;

        // 現在時間が23:00以上なら外出禁止
        if(timeCheckResult) canvasGameObj.transform.Find("goOutButton").GetComponent<Button>().interactable = false;
    }

    public bool CheckBanTime(string playerDataTime)
    {
        // 23:00 default time setting
        DateTime dateTimeDefault = new DateTime();
        TimeSpan timeSpanDefault = new TimeSpan(23,0,0);
        dateTimeDefault = dateTimeDefault + timeSpanDefault;

        // プレイヤー時間
        string[] timeArray = playerDataTime.Split(':');
        DateTime dateTime = new DateTime();
        TimeSpan timeSpan = new TimeSpan(Int32.Parse(timeArray[0]), Int32.Parse(timeArray[1]), 0);
        dateTime = dateTime + timeSpan;

        // 23:00 - プレイヤー時間
        // 現在時間が23:00以上ならtrue
        TimeSpan gapTime = dateTimeDefault - dateTime;
        if(gapTime.TotalMinutes < 1)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public void ClickGoToConvenienceBtn()
    {
        GameObject.Find("Canvas").transform.Find("AlertGoing").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("nextButton").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("goOutButton").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("itemCheckButton").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("time").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("GoOutBox").gameObject.SetActive(false);
        ExecuteFadeInOutV2();
    }

    public void ExecuteFadeInOutV2()
    {
        GameObject FadeInOutManager = new GameObject("FadeInOutManager");
        GameObject fadeObj = GameObject.Find("FadeInOutManager");
        SimpleFadeInOutManagerV2 simpleFadeInOutManagerV2 = fadeObj.AddComponent<SimpleFadeInOutManagerV2>();
        simpleFadeInOutManagerV2.PassParameter(true, "convenience");
    }

    public void ClickGoOutButton(string time)
    {
        // 朝(08:00)なら外出ボタンを防ぐ
        if (time.Equals("08:00"))
        {
            MenuButtonActive(false);
            GameObject.Find("Canvas").transform.Find("GoOutAlertNoBox").gameObject.SetActive(true);
        }
        else
        {
            MenuButtonActive(false);
            GameObject.Find("Canvas").transform.Find("GoOutBox").gameObject.SetActive(true);
        }
    }

    public void ClickGoOutCloseBtn()
    {
        MenuButtonActive(true);
        GameObject.Find("Canvas").transform.Find("GoOutBox").gameObject.SetActive(false);
    }

    public void ClickGoOutCancelBtn()
    {
        MenuButtonActive(true);
        GameObject.Find("Canvas").transform.Find("GoOutAlertNoBox").gameObject.SetActive(false);
    }

    public void ClickNextButton()
    {
        

        // nextButton名によるシーン変更
        if (GameObject.Find("nextButton").transform.Find("Text").GetComponent<Text>().text.Equals("出勤する"))
        {
            SetAlertForGoWork();
            ActiveAlert(true);
            MenuButtonActive(false);
        }
        // 寝る
        else
        {
            // コンビニにのアイテム補充
            convenienceItemSetManager.ResetConvenienceQuantity();

            SetAlertForSleep();
            ActiveAlert(true);
            MenuButtonActive(false);
        }
    }

    public void SetAlertForGoWork()
    {
        GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("alertMessage").GetComponent<Text>().text =
            "<color=#f54242>" + "出勤" + "</color>" + "しますか?";
    }

    public void SetAlertForSleep()
    {
        GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("alertMessage").GetComponent<Text>().text =
            "今日はもう寝ますか?";
        GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("DestinationValue").GetComponent<Text>().text =
            "夢";
    }

    public void UIDisplay(bool sw)
    {
        GameObject.Find("Canvas").transform.Find("nextButton").gameObject.SetActive(sw);
        GameObject.Find("Canvas").transform.Find("goOutButton").gameObject.SetActive(sw);
        GameObject.Find("Canvas").transform.Find("itemCheckButton").gameObject.SetActive(sw);
        GameObject.Find("Canvas").transform.Find("time").gameObject.SetActive(sw);
    }

    public void MenuButtonActive(bool sw)
    {
        GameObject.Find("Canvas").transform.Find("nextButton").GetComponent<Button>().interactable = sw;
        GameObject.Find("Canvas").transform.Find("goOutButton").GetComponent<Button>().interactable = sw;
        GameObject.Find("Canvas").transform.Find("itemCheckButton").GetComponent<Button>().interactable = sw;
    }

    public void ExecuteFadeInOut()
    {
        GameObject FadeInOutManager = new GameObject("FadeInOutManager");
        GameObject fadeObj = GameObject.Find("FadeInOutManager");
        fadeObj.AddComponent<SimpleFadeInOutManager>();
    }

    public void ClickGoToAlertYesButton()
    {
        GameObject.Find("Canvas").transform.Find("AlertGoing").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("nextButton").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("goOutButton").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("itemCheckButton").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("time").gameObject.SetActive(false);
        ExecuteFadeInOut();
    }

    public void ActiveAlert(bool sw)
    {
        GameObject.Find("Canvas").transform.Find("AlertGoing").gameObject.SetActive(sw);
        MenuButtonActive(true);
    }

    public void ClickItemCheckButton()
    {
        sceneTransitionManager = new SceneTransitionManager();
        sceneTransitionManager.LoadTo("ItemCheckScene");
    }
}
