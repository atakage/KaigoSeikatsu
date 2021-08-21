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
    public bool inputFieldFocusBool;
    private TouchScreenKeyboard touchScreenKeyboard;
    private PlayerDataDBModel playerDataDBModel;

    //public DatabaseReference databaseReference;
    private void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        sceneTransitionManager = new SceneTransitionManager();

        IntroSharingObjectManager = GameObject.Find("IntroSharingObjectManager").GetComponent("IntroSharingObjectManager") as IntroSharingObjectManager;
        IntroSharingObjectManager.checkNameButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickCheckNameButton);
        IntroSharingObjectManager.checkNameConfirmButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickCheckNameConfirmButton);
        IntroSharingObjectManager.checkNameCancelButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickCheckNameCancelButton);
        IntroSharingObjectManager.offLinePlayAlertBoxOffLineButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickOffLinePlayButton);
        IntroSharingObjectManager.offLinePlayAlertBoxOnLineButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickOnLinePlayButton);
        IntroSharingObjectManager.alertBoxCancelButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickAlertBoxCancelButton);

        FirebaseManager = GameObject.Find("FirebaseManager").GetComponent("FirebaseManager") as FirebaseManager;


    }

    private void Update()
    {
        // TouchScreenkeyBoard
    }

    public void ClickOffLinePlayButton()
    {
        ActivingTestPaperBox(true);
        ActivingOffLinePlayAlertBox(false);
    }

    public async void ClickOnLinePlayButton()
    {

        IntroSharingObjectManager.alertBoxTextGameObj.GetComponent<Text>().text = "名前を確認しています...";

        ActivingOffLinePlayAlertBox(false);
        ActivingAlertBox(true);

        // DB用プレイヤーデータを作る
        playerDataDBModel = new PlayerDataDBModel();
        playerDataDBModel.name = IntroSharingObjectManager.nameValueGameObj.GetComponent<InputField>().text;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        bool connectionResult = false;
        // 最大3秒Firebaseに接続を試みる
        while (stopwatch.Elapsed < TimeSpan.FromMilliseconds(3000))
        {
            connectionResult = await FirebaseManager.FireBaseConnection(playerDataDBModel);
            if (connectionResult) break;
        }
        
        UnityEngine.Debug.Log("connectionResult: " + connectionResult);
        // DB接続に成功する
        if (connectionResult)
        {
            //★変換methodにawaitをつけないとFindDataToDBの中でreturnValueがすぐにreturnされてしまう
            string findDataDBResult = await FirebaseManager.FindDataToDB(playerDataDBModel);
            UnityEngine.Debug.Log("findDataDBResult: " + findDataDBResult);

            // FindDataToDBの結果がnullならサーバーにプレイヤーデータ作成
            if (string.IsNullOrEmpty(findDataDBResult))
            {

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
        ActivingCheckNameAlertBox(false);
        ActivingOffLinePlayAlertBox(true);
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
            // default name 'ゆかり'
            IntroSharingObjectManager.nameValueGameObj.GetComponent<InputField>().text = "ゆかり";
        }

        IntroSharingObjectManager.checkNameAlertBoxTextGameObj.GetComponent<Text>().text =
          "名前は" + "<b>" + IntroSharingObjectManager.nameValueGameObj.GetComponent<InputField>().text + "</b>" + "ですか?";
        ActivingTestPaperBox(false);
        ActivingCheckNameAlertBox(true);

        /*
        FirebaseManager.FireBaseConnection();
        PlayerDataDBModel playerDataDBModel = new PlayerDataDBModel();
        playerDataDBModel.name = "BBB";
        playerDataDBModel.ending = "endingB";
        FirebaseManager.FindDataToDB(playerDataDBModel);
        */
    }
}
