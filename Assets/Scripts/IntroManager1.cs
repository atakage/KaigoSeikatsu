using System.Collections;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;


public class IntroManager1 : MonoBehaviour
{
    public ChatManager chatManager;
    public MsgChoiceManager msgChoiceManager;
    public SceneTransitionManager sceneTransitionManager;
    public EventManager eventManager;
    public PlayerSaveDataManager playerSaveDataManager;
    public PlayerData playerData;
    public IntroSharingObjectManager IntroSharingObjectManager;
    public FirebaseManager FirebaseManager;
    public PlayTimeManager playTimeManager;
    public BuildManager buildManager;
    public JobDiarySetManager jobDiarySetManager;
    public CSVManager csvManager;
    public bool inputFieldFocusBool;
    private TouchScreenKeyboard touchScreenKeyboard;
    private PlayerDataDBModel playerDataDBModel;

    //public DatabaseReference databaseReference;
    private void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        sceneTransitionManager = new SceneTransitionManager();
        buildManager = GameObject.Find("BuildManager").GetComponent("BuildManager") as BuildManager;
        jobDiarySetManager = new JobDiarySetManager();
        csvManager = new CSVManager();

        IntroSharingObjectManager = GameObject.Find("IntroSharingObjectManager").GetComponent("IntroSharingObjectManager") as IntroSharingObjectManager;
        IntroSharingObjectManager.checkNameButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickCheckNameButton);
        IntroSharingObjectManager.checkNameConfirmButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickCheckNameConfirmButton);
        IntroSharingObjectManager.checkNameCancelButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickCheckNameCancelButton);
        IntroSharingObjectManager.alertBoxCancelButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickAlertBoxCancelButton);
        IntroSharingObjectManager.titleButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickTitleButton);

        FirebaseManager = GameObject.Find("FirebaseManager").GetComponent("FirebaseManager") as FirebaseManager;
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;
        playTimeManager = GameObject.Find("PlayTimeManager").GetComponent("PlayTimeManager") as PlayTimeManager;
    }

    private void Update()
    {
        // TouchScreenkeyBoard


        // ユーザーデータを作ったあとScene転換
        if (IntroSharingObjectManager.canvasGameObj.transform.Find("fadeOutPersistEventCheck") != null
           && IntroSharingObjectManager.canvasGameObj.transform.Find("fadeOutPersistEventCheck") != null
           && IntroSharingObjectManager.canvasGameObj.transform.Find("fadeOutPersistEventCheck").GetComponent<Text>().text.Equals("Y"))
        {
            sceneTransitionManager.LoadTo("AtHomeScene");
        }

        // 初期化------------------------------------------------------------------------------------------------------------------
        if (IntroSharingObjectManager.canvasGameObj.transform.Find("fadeOutPersistEventCheck") != null)
        {
            IntroSharingObjectManager.canvasGameObj.transform.Find("fadeOutPersistEventCheck").GetComponent<Text>().text = "N";
            //Destroy(canvasObj.transform.Find("fadeOutEndMomentSW").gameObject);
        }
    }

    public void ClickTitleButton()
    {
        sceneTransitionManager.LoadTo("TitleScene");
    }

    public void ClickOffLinePlayButton()
    {
        ActivingAlertBox(false);
        ActivingTestPaperBox(false);
        ActivingOffLinePlayAlertBox(false);

        // プレイヤーデータセーブ
        // プレイヤーアイテムデータ初期化
        playerSaveDataManager.RemoveItemListDataJsonFile();
        ItemListData[] itemListData = new ItemListData[1];
        itemListData[0] = new ItemListData();
        itemListData[0].itemName = "名刺";
        itemListData[0].itemDescription = "介護福祉士の名刺だ";
        itemListData[0].quantity = 1;
        itemListData[0].keyItem = "Y";
        playerSaveDataManager.SaveItemListData(itemListData);

        // 新しいプレイヤーデータを作成
        playerData = new PlayerData();
        playerData.name = IntroSharingObjectManager.nameValueGameObj.GetComponent<InputField>().text;
        playerData.money = "15000"; // 円
        playerData.time = "08:00";
        playerData.progress = 0;
        playerData.fatigue = 0;
        playerData.currentScene = "AtHomeScene";
        playerData.localMode = true;
        playerData.startDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        playerSaveDataManager.SavePlayerData(playerData);

        // プレイ時間カウント
        playTimeManager.countPlayTime = true;

        LoadEventAndShow("EV027");
    }

    public async void ClickOnLinePlayButton()
    {

        IntroSharingObjectManager.alertBoxTextGameObj.GetComponent<Text>().text = "名前を確認しています...";

        ActivingOffLinePlayAlertBox(false);
        ActivingAlertBox(true);

        // DB用プレイヤーデータを作る
        playerDataDBModel = new PlayerDataDBModel();
        playerDataDBModel.name = IntroSharingObjectManager.nameValueGameObj.GetComponent<InputField>().text;
        playerDataDBModel.startDate = DateTime.Now.ToString("yyyyMMddHHmmss");

        /*
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        bool connectionResult = false;
        // 最大5秒Firebaseに接続を試みる
        while (stopwatch.Elapsed < TimeSpan.FromMilliseconds(5000))
        {
            connectionResult = await FirebaseManager.FireBaseConnection();
            if (connectionResult) break;
        }
        
        UnityEngine.Debug.Log("connectionResult: " + connectionResult);
        */
        bool connectionResult = true;
        // DB接続に成功する
        if (connectionResult)
        {
            //★変換methodにawaitをつけないとFindDataToDBの中でreturnValueがすぐにreturnされてしまう
            string findDataDBResult = await FirebaseManager.FindDataToDB(playerDataDBModel);
            UnityEngine.Debug.Log("findDataDBResult: " + findDataDBResult);

            // FindDataToDBの結果がnullなら
            if (string.IsNullOrEmpty(findDataDBResult))
            {
                // サーバーにプレイヤーデータ作成
                string insertUpdateResult = await FirebaseManager.InsertUpdateToDB(playerDataDBModel);

                // insertUpdateResultが'success'なら
                if ("success".Equals(insertUpdateResult))
                {
                    ActivingAlertBox(false);

                    // 2021.11.28 修正
                    // IntroSceneでプレイヤーデータが作れると既存ユーザー情報を削除する

                    // JobDiary.jsonを作る
                    jobDiarySetManager.CreateJobDiaryJsonFile(new List<JobDiaryModel>());
                    // プレイヤーアイテムデータ初期化
                    playerSaveDataManager.RemoveItemListDataJsonFile();
                    // コンビニデータを初期化
                    csvManager.CreateConvenienceJsonFile();

                    // プレイヤーデータセーブ
                    // プレイヤーアイテムデータ初期化
                    playerSaveDataManager.RemoveItemListDataJsonFile();
                    ItemListData[] itemListData = new ItemListData[1];
                    itemListData[0] = new ItemListData();
                    itemListData[0].itemName = "名刺";
                    itemListData[0].itemDescription = "介護福祉士の名刺だ";
                    itemListData[0].quantity = 1;
                    itemListData[0].keyItem = "Y";
                    playerSaveDataManager.SaveItemListData(itemListData);

                    // 新しいプレイヤーデータを作成
                    playerData = new PlayerData();
                    playerData.name = playerDataDBModel.name;
                    playerData.money = "1000"; // 円
                    playerData.time = "08:00";
                    playerData.progress = 0;
                    playerData.fatigue = 0;
                    playerData.currentScene = "AtHomeScene";
                    playerData.localMode = false;
                    playerData.startDate = playerDataDBModel.startDate;
                    playerSaveDataManager.SavePlayerData(playerData);

                    // プレイ時間カウント
                    playTimeManager.countPlayTime = true;

                    LoadEventAndShow("EV027");
                }
                // insertUpdateResultが'success'じゃないなら
                else
                {
                    IntroSharingObjectManager.alertBoxTextGameObj.GetComponent<Text>().text = insertUpdateResult;
                    ActivingAlertCancelButton(true);
                }

            }else if (findDataDBResult.Equals("timeOut"))
            {
                IntroSharingObjectManager.alertBoxTextGameObj.GetComponent<Text>().text = "サーバーとの通信に失敗しました";
                ActivingAlertCancelButton(true);
            }
            // FindDataToDBの結果がnullじゃないならすでに使用されている名前
            else
            {
                IntroSharingObjectManager.alertBoxTextGameObj.GetComponent<Text>().text = findDataDBResult;
                ActivingAlertCancelButton(true);
            }
        }
        // DB接続に失敗すると
        else
        {
            IntroSharingObjectManager.alertBoxTextGameObj.GetComponent<Text>().text = "サーバーとの通信に失敗しました";
            ActivingAlertCancelButton(true);
        }

    }

    public void ClickAlertBoxCancelButton()
    {
        ActivingAlertBox(false);
        ActivingTestPaperBox(true);
    }

    public void ClickCheckNameConfirmButton()
    {
        // 2021.10.20 追加 ボタン音
        GameObject.Find("SoundManager").GetComponent<AudioSource>().Play();

        ActivingCheckNameAlertBox(false);
        //ActivingOffLinePlayAlertBox(true);
        ClickOnLinePlayButton();

    }

    public void ClickCheckNameCancelButton()
    {
        ActivingTestPaperBox(true);
        ActivingCheckNameAlertBox(false);
    }

    public void ActivingAlertBox(bool sw)
    {
        IntroSharingObjectManager.alertBoxGameObj.SetActive(sw);
        IntroSharingObjectManager.alertBoxCancelButtonGameObj.SetActive(false);
    }

    public void ActivingAlertCancelButton(bool sw)
    {
        IntroSharingObjectManager.alertBoxCancelButtonGameObj.SetActive(sw);
    }

    public void ActivingOffLinePlayAlertBox(bool sw)
    {
        IntroSharingObjectManager.offLinePlayAlertBoxGameObj.SetActive(sw);
    }

    public void ActivingTestPaperBox(bool sw)
    {
        IntroSharingObjectManager.testPaperBoxGameObj.SetActive(sw);
    }

    public void ActivingCheckNameAlertBox(bool sw)
    {
        IntroSharingObjectManager.checkNameAlertBoxGameObj.gameObject.SetActive(sw);
    }

    public void ClickCheckNameButton()
    {

        // input fieldにネームが入力されなかったら
        if (string.IsNullOrEmpty(IntroSharingObjectManager.nameValueGameObj.GetComponent<InputField>().text))
        {
            UnityEngine.Debug.Log("null");
            // default name 'ゆあ'
            IntroSharingObjectManager.nameValueGameObj.GetComponent<InputField>().text = "ゆあ";
        }

        IntroSharingObjectManager.checkNameAlertBoxTextGameObj.GetComponent<Text>().text =
          "名前は" + "<b>" + IntroSharingObjectManager.nameValueGameObj.GetComponent<InputField>().text + "</b>" + "ですか?";
        ActivingTestPaperBox(false);
        ActivingCheckNameAlertBox(true);
    }

    public void LoadEventAndShow(string eventCode)
    {
        EventListData[] loadedEventListData = playerSaveDataManager.LoadedEventListData();
        EventListData eventItem = eventManager.FindEventByCode(loadedEventListData, eventCode);
        List<string[]> scriptList = eventManager.ScriptSaveToList(eventItem);
        // 2021.07.26 修正, キャライメージ追加されたrawScriptをparameterに渡す
        chatManager.ShowDialogue(scriptList, eventCode, eventItem.script);
    }
}
