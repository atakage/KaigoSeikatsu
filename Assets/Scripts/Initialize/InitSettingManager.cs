using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitSettingManager : MonoBehaviour
{
    public CSVManager csvManager;
    public PlayerSaveDataManager playerSaveDataManager;
    public SceneTransitionManager sceneTransitionManager;
    public JobDiarySetManager jobDiarySetManager;
    public PlayTimeManager playTimeManager;
    public BuildManager buildManager;
    public GameClearFileManager gameClearFileManager;
    public JobEventSetManager jobEventSetManager;
    public GameObject canvasObj;
    public GameObject loadButtonObj;
    public GameObject newGameAlertBoxObj;
    public PlayerData playerData;
    public int msgCheckIntVal;
    void Start()
    {
        csvManager = new CSVManager();
        playerSaveDataManager = new PlayerSaveDataManager();
        sceneTransitionManager = new SceneTransitionManager();
        jobDiarySetManager = new JobDiarySetManager();
        gameClearFileManager = new GameClearFileManager();
        jobEventSetManager = new JobEventSetManager();

        playTimeManager = GameObject.Find("PlayTimeManager").GetComponent("PlayTimeManager") as PlayTimeManager;
        buildManager = GameObject.Find("BuildManager").GetComponent("BuildManager") as BuildManager;

        canvasObj = GameObject.Find("Canvas");
        newGameAlertBoxObj = canvasObj.transform.Find("newGameAlertBox").gameObject;
        msgCheckIntVal = 0;

        // コンビニで販売するアイテムをセットする(最初)
        // ConvenienceItemInit.txtにある情報をResource/saveData/に移す
        // itemSaleを変更したいときはそのSceneでを読み込んで変更したあとセーブすればいい
        // があると作らない(最初だけ作る)
        csvManager.ReadConvenienceInitFileAndCreateJson();

        // カフェで販売するアイテムをセットする
        csvManager.ReadCafeItemInitFileAndCreateJson();

        // MainEvent.jsonを作る
        csvManager.ReadMainEventInitFileAndCreateJson();

        // JobEvent.jsonを作る
        csvManager.ReadJobEventInitFileAndCreateJson();

        canvasObj.transform.Find("careGiverListButton").gameObject.SetActive(true);

        /*
        // clearFileが存在すると
        if (gameClearFileManager.CheckExistClearFile(buildManager.buildMode))
        {
            // clearFileをロード
            ClearData clearData = gameClearFileManager.LoadClearData(buildManager.buildMode);
            // clear後なら
            if (clearData.clear == true)
            {
                canvasObj.transform.Find("careGiverListButton").gameObject.SetActive(true);
            }
        }
        */

        PlayerData playerData = playerSaveDataManager.LoadPlayerData();

        canvasObj.transform.Find("PlayButton").GetComponent<Button>().onClick.AddListener(() => ClickPlayButton(playerData));
        newGameAlertBoxObj.transform.Find("confirmButton").GetComponent<Button>().onClick.AddListener(() => ClickNewGameAlertBoxConfirmBtn(this.msgCheckIntVal));
        newGameAlertBoxObj.transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(() => ClickNewGameAlertBoxCancelBtn());
        canvasObj.transform.Find("careGiverListButton").GetComponent<Button>().onClick.AddListener(ClickCareGiverListButton);

        // プレイヤーデータがある場合ロードボタンを表示する
        if (playerData != null)
        {
            loadButtonObj = canvasObj.transform.Find("loadButton").gameObject;
            loadButtonObj.GetComponent<Button>().interactable = true;
            loadButtonObj.GetComponent<Button>().onClick.AddListener(() => ClickLoadButton(playerData));
        }
    }

    public void ClickCareGiverListButton()
    {
        // 2021.10.20 追加 ボタン音
        GameObject.Find("SoundManager").GetComponent<AudioSource>().Play();

        sceneTransitionManager.LoadTo("CareGiverListScene");
    }

    public void ClickNewGameAlertBoxConfirmBtn(int msgCheckIntVal)
    {
        // 2021.10.20 追加 ボタン音
        GameObject.Find("SoundManager").GetComponent<AudioSource>().Play();

        Debug.Log("msgCheckIntVal: " + msgCheckIntVal);

        // JobEvent.json初期化
        Dictionary<string, Dictionary<string, object>> jobEventListDic = csvManager.GetTxtItemList("JobEvent");
        // JobEvent.jsonを作る
        jobEventSetManager = new JobEventSetManager();
        jobEventSetManager.CreateJobEventJson(jobEventListDic);

        // 追加メッセージ
        if (msgCheckIntVal == 0)
        {
            string msg = "ゲームを始めますか?";
            newGameAlertBoxObj.transform.Find("message").GetComponent<Text>().text = msg;
            this.msgCheckIntVal = 1;
        }
        // ゲームが始まるnew game
        else if (msgCheckIntVal == 1)
        {
            // JobDiary.jsonを作る
            jobDiarySetManager.CreateJobDiaryJsonFile(new List<JobDiaryModel>());
            
            // プレイヤーアイテムデータ初期化
            playerSaveDataManager.RemoveItemListDataJsonFile();

            sceneTransitionManager.LoadTo("IntroScene");
        }
    }

    public void ClickNewGameAlertBoxCancelBtn()
    {
        newGameAlertBoxObj.SetActive(false);
        string msg = "新しいデータが作成されると\n現在セーブデータは削除されます";
        newGameAlertBoxObj.transform.Find("message").GetComponent<Text>().text = msg;
        this.msgCheckIntVal = 0;
        canvasObj.transform.Find("careGiverListButton").GetComponent<Button>().interactable = true;
    }

    public void ClickPlayButton(PlayerData playerData)
    {
        // 2021.10.20 追加 ボタン音
        GameObject.Find("SoundManager").GetComponent<AudioSource>().Play();

        // プレイヤーデータがあるとメッセージを表示する
        if (playerData != null)
        {
            newGameAlertBoxObj.SetActive(true);
            canvasObj.transform.Find("careGiverListButton").GetComponent<Button>().interactable = false;
        }
        // プレイヤーデータがないとnew game
        else
        {
            // JobEvent.json初期化
            Dictionary<string, Dictionary<string, object>> jobEventListDic = csvManager.GetTxtItemList("JobEvent");
            // JobEvent.jsonを作る
            jobEventSetManager = new JobEventSetManager();
            jobEventSetManager.CreateJobEventJson(jobEventListDic);

            // プレイヤーアイテムデータ初期化
            playerSaveDataManager.RemoveItemListDataJsonFile();

            sceneTransitionManager.LoadTo("IntroScene");
        }
    }

    public void ClickLoadButton(PlayerData playerData)
    {
        // 2021.10.20 追加 ボタン音
        GameObject.Find("SoundManager").GetComponent<AudioSource>().Play();

        // プレイ時間記録を始める
        playTimeManager.playTime = playerData.playTime;
        playTimeManager.countPlayTime = true;

        GameObject loadValueSW = new GameObject("loadValueSW");
        loadValueSW.AddComponent<Text>();
        loadValueSW.transform.GetComponent<Text>().text = "Y";
        DontDestroyOnLoad(loadValueSW);

        sceneTransitionManager.LoadTo(playerData.currentScene);
    }
}
