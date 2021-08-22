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

        playTimeManager = GameObject.Find("PlayTimeManager").GetComponent("PlayTimeManager") as PlayTimeManager;

        canvasObj = GameObject.Find("Canvas");
        newGameAlertBoxObj = canvasObj.transform.Find("newGameAlertBox").gameObject;
        msgCheckIntVal = 0;

        // コンビニで販売するアイテムをセットする(最初)
        // ConvenienceItemInit.txtにある情報をResource/saveData/ConvenienceItem.jsonに移す
        // itemSaleを変更したいときはそのSceneでConvenienceItem.jsonを読み込んで変更したあとセーブすればいい
        // ConvenienceItem.jsonがあると作らない(最初だけ作る)
        csvManager.ReadConvenienceInitFileAndCreateJson();

        // カフェで販売するアイテムをセットする
        csvManager.ReadCafeItemInitFileAndCreateJson();

        // MainEvent.jsonを作る
        csvManager.ReadMainEventInitFileAndCreateJson();

        // JobEvent.jsonを作る
        csvManager.ReadJobEventInitFileAndCreateJson();

       
        PlayerData playerData = playerSaveDataManager.LoadPlayerData();

        canvasObj.transform.Find("PlayButton").GetComponent<Button>().onClick.AddListener(() => ClickPlayButton(playerData));
        newGameAlertBoxObj.transform.Find("confirmButton").GetComponent<Button>().onClick.AddListener(() => ClickNewGameAlertBoxConfirmBtn(this.msgCheckIntVal));
        newGameAlertBoxObj.transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(() => ClickNewGameAlertBoxCancelBtn());

        // プレイヤーデータがある場合ロードボタンを表示する
        if (playerData != null)
        {
            loadButtonObj = canvasObj.transform.Find("loadButton").gameObject;
            loadButtonObj.GetComponent<Button>().interactable = true;
            loadButtonObj.GetComponent<Button>().onClick.AddListener(() => ClickLoadButton(playerData));
        }

        
    }

    public void ClickNewGameAlertBoxConfirmBtn(int msgCheckIntVal)
    {
        Debug.Log("msgCheckIntVal: " + msgCheckIntVal);

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
    }

    public void ClickPlayButton(PlayerData playerData)
    {
        // プレイヤーデータがあるとメッセージを表示する
        if (playerData != null)
        {
            newGameAlertBoxObj.SetActive(true);
        }
        // プレイヤーデータがないとnew game
        else
        {            
            // プレイヤーアイテムデータ初期化
            playerSaveDataManager.RemoveItemListDataJsonFile();

            sceneTransitionManager.LoadTo("IntroScene");
        }

    }

    public void ClickLoadButton(PlayerData playerData)
    {
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
