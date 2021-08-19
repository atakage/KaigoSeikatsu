using System.Collections;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
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
        ActivingOffLinePlayAlertBox(false);
        ActivingAlertBox(true);

        // DB用プレイヤーデータを作る
        playerDataDBModel = new PlayerDataDBModel();
        playerDataDBModel.name = IntroSharingObjectManager.nameValueGameObj.GetComponent<InputField>().text;

        FirebaseManager.FireBaseConnection();
        // ★変換methodにawaitをつけないとFindDataToDBの中でreturnValueがすぐにreturnされてしまう
        string findDataDBResult = await FirebaseManager.FindDataToDB(playerDataDBModel);
        Debug.Log("findDataDBResult: " + findDataDBResult);

        // FindDataToDBの結果がnullならサーバーにプレイヤーデータ作成
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
            Debug.Log("null");
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
