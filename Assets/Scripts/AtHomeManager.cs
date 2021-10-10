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
    public UtilManager utilManager;
    public FirebaseManager firebaseManager;
    public PlayerDataToPlayerDataDBModelManager playerDataToPlayerDataDBModelManager;
    public PlayerData playerData = null;
    public GameObject canvasGameObj;
    public Boolean timeCheckResult;
    public string loadValueSW;
    private void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        sceneTransitionManager = new SceneTransitionManager();
        convenienceItemSetManager = new ConvenienceItemSetManager();
        firebaseManager = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();
        playerDataToPlayerDataDBModelManager = new PlayerDataToPlayerDataDBModelManager();
        utilManager = new UtilManager();

        
        if (GameObject.Find("SceneChangeManager") != null) GameObject.Find("SceneChangeManager").transform.Find("SceneChangeCanvas").transform.Find("destinationFrom-toItemCheckScene").GetComponent<Text>().text = SceneManager.GetActiveScene().name;

        // TitleSceneからロードした時やMenuSceneからもどる時についてくるvalue
        if (GameObject.Find("loadValueSW") != null) loadValueSW = GameObject.Find("loadValueSW").transform.GetComponent<Text>().text;
        else loadValueSW = "N";

        canvasGameObj = GameObject.Find("Canvas").gameObject;

        playerData = playerSaveDataManager.LoadPlayerData();
        string time = playerData.time;
        canvasGameObj.transform.Find("time").GetComponent<Text>().text = time;

        // 現在外出禁止時間なのか確認する
        timeCheckResult = CheckBanTime(playerData.time);

        // 朝なら出勤する、夜なら寝るにボタン変更
        GameObject.Find("nextButton").transform.Find("Text").GetComponent<Text>().text 
                           = (time.Equals("08:00")) ? "出勤する" : "寝る";

        canvasGameObj.transform.Find("nextButton").GetComponent<Button>().onClick.AddListener(ClickNextButton);
        canvasGameObj.transform.Find("AlertGoing").transform.Find("No").GetComponent<Button>().onClick.AddListener(delegate { ActiveAlert(false); });
        canvasGameObj.transform.Find("AlertGoing").transform.Find("Yes").GetComponent<Button>().onClick.AddListener(ClickGoToAlertYesButton);
        canvasGameObj.transform.Find("itemCheckButton").GetComponent<Button>().onClick.AddListener(ClickItemCheckButton);
        canvasGameObj.transform.Find("statusButton").GetComponent<Button>().onClick.AddListener(ClickStatusButton);
        canvasGameObj.transform.Find("jobDiaryButton").GetComponent<Button>().onClick.AddListener(ClickJobDiaryButton);
        canvasGameObj.transform.Find("goOutButton").GetComponent<Button>().onClick.AddListener(delegate { ClickGoOutButton(time); });
        canvasGameObj.transform.Find("GoOutBox").transform.Find("goToConvenienceButton").GetComponent<Button>().onClick.AddListener(ClickGoToConvenienceBtn);
        canvasGameObj.transform.Find("GoOutBox").transform.Find("closeButton").GetComponent<Button>().onClick.AddListener(ClickGoOutCloseBtn);
        canvasGameObj.transform.Find("GoOutAlertNoBox").transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(ClickGoOutCancelBtn);
    }

    private void Update()
    {
        // 出勤する
        if (GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("FadeSwitchText").GetComponent<Text>().text.Equals("call") &&
            GameObject.Find("Canvas").transform.Find("nextButton").transform.Find("Text").GetComponent<Text>().text.Equals("出勤する"))
        {
            // 次のシーンをプレイヤーデータにセーブ
            playerData.currentScene = "FacilityScene";
            playerData.time = "08:50";
            playerSaveDataManager.SavePlayerData(playerData);
            sceneTransitionManager.LoadTo("FacilityScene");

        // 寝て朝になる
        }else if (GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("FadeSwitchText").GetComponent<Text>().text.Equals("call") &&
                  GameObject.Find("Canvas").transform.Find("nextButton").transform.Find("Text").GetComponent<Text>().text.Equals("寝る"))
        {
            // 2021.10.09 追加
            // fatigue(つかれ)が20以上ならEnding'3'(goToReadyForEndingScene)
            playerData = playerSaveDataManager.LoadPlayerData();
            // つかれが20未満なら
            if (playerData.fatigue < 20)
            {
                GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("FadeSwitchText").GetComponent<Text>().text = "";
                GameObject.Find("Canvas").transform.Find("nextButton").transform.Find("Text").GetComponent<Text>().text = "出勤する";
                playerData.time = "08:00";
                playerSaveDataManager.SavePlayerData(playerData);
                GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text = playerData.time;
                MenuButtonActive(true);
                UIDisplay(true);
            }
            // つかれが20以上なら
            else
            {
                playerData.ending = "endingC";
                playerData.currentScene = "ReadyForEndingScene";
                playerSaveDataManager.SavePlayerData(playerData);
                sceneTransitionManager.LoadTo("ReadyForEndingScene");
            }
        // コンビニへ行く
        }else if (GameObject.Find("Canvas").transform.Find("FadeCompleteValue").GetComponent<Text>().text.Equals("convenience"))
        {
            // 現在プレイヤーデータの時間を変更する(add minute)
            DateTime addedDateTime = utilManager.TimeCal(playerData.time, 20);
            playerData.time = addedDateTime.Hour.ToString("D2") + ":" + addedDateTime.Minute.ToString("D2");
            playerSaveDataManager.SavePlayerData(playerData);

            sceneTransitionManager.LoadTo("ConvenienceScene");
        }

        // 朝は外出禁止
        if ("08:00".Equals(playerData.time)) canvasGameObj.transform.Find("goOutButton").GetComponent<Button>().interactable = false;

        // 現在時間が23:00以上なら外出禁止
        if(timeCheckResult) canvasGameObj.transform.Find("goOutButton").GetComponent<Button>().interactable = false;

        // 初期化------------------------------------------------------------------------------------------------------------------
        if (loadValueSW.Equals("Y"))
        {
            loadValueSW = "N";
            if (GameObject.Find("loadValueSW") != null) Destroy(GameObject.Find("loadValueSW"));
        }
    }

    public void ClickStatusButton()
    {
        SceneManager.LoadScene("StatusScene", LoadSceneMode.Additive);
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

    public void ClickJobDiaryButton()
    {
        sceneTransitionManager.LoadTo("JobDiaryScene");
    }

    public void ClickGoToConvenienceBtn()
    {
        playerData = playerSaveDataManager.LoadPlayerData();
        playerData.currentScene = "ConvenienceScene";
        playerSaveDataManager.SavePlayerData(playerData);

        canvasGameObj.transform.Find("AlertGoing").gameObject.SetActive(false);
        canvasGameObj.transform.Find("nextButton").gameObject.SetActive(false);
        canvasGameObj.transform.Find("goOutButton").gameObject.SetActive(false);
        canvasGameObj.transform.Find("itemCheckButton").gameObject.SetActive(false);
        canvasGameObj.transform.Find("statusButton").gameObject.SetActive(false);
        canvasGameObj.transform.Find("jobDiaryButton").gameObject.SetActive(false);
        canvasGameObj.transform.Find("time").gameObject.SetActive(false);
        canvasGameObj.transform.Find("GoOutBox").gameObject.SetActive(false);
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
        playerData = playerSaveDataManager.LoadPlayerData();
        // 2021.10.10 追加
        // プレイヤーが疲れ果ての状態(fatigue20以上)なら
        if (playerData.fatigue > 19)
        {
            canvasGameObj.transform.Find("AlertGoing").transform.Find("alertMessage").GetComponent<Text>().text =
            "体の具合が悪い、\n疲れが取れない状態寝ますか?";
            canvasGameObj.transform.Find("AlertGoing").transform.Find("DestinationValue").GetComponent<Text>().text =
            "ending";
        }
        // プレイヤーが疲れ果ての状態(fatigue20未満なら)なら
        else
        {
            canvasGameObj.transform.Find("AlertGoing").transform.Find("alertMessage").GetComponent<Text>().text =
            "今日はもう寝ますか?";
            canvasGameObj.transform.Find("AlertGoing").transform.Find("DestinationValue").GetComponent<Text>().text =
            "夢";
        }
       
    }

    public void UIDisplay(bool sw)
    {
        canvasGameObj.transform.Find("nextButton").gameObject.SetActive(sw);
        canvasGameObj.transform.Find("goOutButton").gameObject.SetActive(sw);
        canvasGameObj.transform.Find("itemCheckButton").gameObject.SetActive(sw);
        canvasGameObj.transform.Find("time").gameObject.SetActive(sw);
        canvasGameObj.transform.Find("statusButton").gameObject.SetActive(sw);
        canvasGameObj.transform.Find("jobDiaryButton").gameObject.SetActive(sw);
    }

    public void MenuButtonActive(bool sw)
    {
        canvasGameObj.transform.Find("nextButton").GetComponent<Button>().interactable = sw;
        canvasGameObj.transform.Find("goOutButton").GetComponent<Button>().interactable = sw;
        canvasGameObj.transform.Find("itemCheckButton").GetComponent<Button>().interactable = sw;
        canvasGameObj.transform.Find("statusButton").GetComponent<Button>().interactable = sw;
        canvasGameObj.transform.Find("jobDiaryButton").GetComponent<Button>().interactable = sw;
    }

    public void ExecuteFadeInOut()
    {
        GameObject FadeInOutManager = new GameObject("FadeInOutManager");
        GameObject fadeObj = GameObject.Find("FadeInOutManager");
        fadeObj.AddComponent<SimpleFadeInOutManager>();
    }

    public async void ClickGoToAlertYesButton()
    {
        if (GameObject.Find("nextButton").transform.Find("Text").GetComponent<Text>().text.Equals("寝る"))
        {
            // 2021.10.09追加
            // 次の日になるたびお金+100円
            playerData.money = (Int32.Parse(playerData.money)+100).ToString();

            // 2021.10.14追加
            // 次の日になる時データ収集ためDBにデータセーブ
            bool connectionResult = await firebaseManager.FireBaseConnection();
            if (connectionResult) await firebaseManager.InsertUpdateToDB(playerDataToPlayerDataDBModelManager.PlayerDataToDBModel(playerData));
        }
        playerSaveDataManager.SavePlayerData(playerData);

        canvasGameObj.transform.Find("AlertGoing").gameObject.SetActive(false);
        canvasGameObj.transform.Find("nextButton").gameObject.SetActive(false);
        canvasGameObj.transform.Find("goOutButton").gameObject.SetActive(false);
        canvasGameObj.transform.Find("itemCheckButton").gameObject.SetActive(false);
        canvasGameObj.transform.Find("statusButton").gameObject.SetActive(false);
        canvasGameObj.transform.Find("jobDiaryButton").gameObject.SetActive(false);
        canvasGameObj.transform.Find("time").gameObject.SetActive(false);
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
