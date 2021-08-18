using System.Collections;
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

    //public DatabaseReference databaseReference;
    private void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        sceneTransitionManager = new SceneTransitionManager();

        IntroSharingObjectManager = GameObject.Find("IntroSharingObjectManager").GetComponent("IntroSharingObjectManager") as IntroSharingObjectManager;
        IntroSharingObjectManager.checkNameButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickCheckNameButton);

        FirebaseManager = GameObject.Find("FirebaseManager").GetComponent("FirebaseManager") as FirebaseManager;


    }

    private void Update()
    {
        // TouchScreenkeyBoard
    }

    public void ActivingTestPaperBox(bool sw)
    {
        IntroSharingObjectManager.testPaperBoxGameObj.gameObject.SetActive(sw);
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
