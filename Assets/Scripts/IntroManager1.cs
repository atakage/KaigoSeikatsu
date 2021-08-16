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
    public GameObject canvasObj;
    public IntroSharingObjectManager IntroSharingObjectManager;
    public DatabaseReference databaseReference;
    private void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        sceneTransitionManager = new SceneTransitionManager();

        IntroSharingObjectManager = GameObject.Find("IntroSharingObjectManager").GetComponent("IntroSharingObjectManager") as IntroSharingObjectManager;
        IntroSharingObjectManager.checkNameButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickCheckNameButton);


    }

    public void InsertUpdateToDB()
    {
        try
        {
            PlayerDataDBModel playerDataDBModel = new PlayerDataDBModel();
            playerDataDBModel.name = "FFF";
            playerDataDBModel.ending = "endingB";

            databaseReference.Child(playerDataDBModel.name).SetRawJsonValueAsync(JsonUtility.ToJson(playerDataDBModel))
                .ContinueWith(task => {
                    if (task.IsFaulted)
                    {
                        Debug.Log("insert update fail: " + task.Exception);
                    }
                    else
                    {
                        Debug.Log("insert update success");
                    }
                });
        }
        catch
        {

        }
    }

    public void FireBaseConnection()
    {
        try
        {
            // FirebaseDatabase.DefaultInstance.GetReference("player_data"): DB名にaccess
            databaseReference = FirebaseDatabase.DefaultInstance.GetReference("player_data");
        }
        catch
        {

        }
    }

    public void FindDataToDB()
    {
        try
        {
            PlayerDataDBModel playerDataDBModel = new PlayerDataDBModel();
            playerDataDBModel.name = "FFF";
            playerDataDBModel.ending = "endingB";

            databaseReference.Child(playerDataDBModel.name).ValueChanged += HandleValueChanged;
        }
        catch
        {

        }
    }

    public void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log("args.Snapshot.Key: " + args.Snapshot.Key);
        // DBにデータががないならempty
        Debug.Log("args.Snapshot.GetRawJsonValue(): " + args.Snapshot.GetRawJsonValue());
    }

    public void ClickCheckNameButton()
    {
        FireBaseConnection();
        FindDataToDB();
        //InsertUpdateToDB();
        
    }
}
