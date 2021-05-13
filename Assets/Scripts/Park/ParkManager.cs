using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ParkManager : MonoBehaviour
{
    public PlayerSaveDataManager playerSaveDataManager;
    public EventManager eventManager;
    public ChatManager chatManager;
    public GameObject canvasGameObj;
    void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;

        canvasGameObj = GameObject.Find("Canvas");
        PlayerData playerData = playerSaveDataManager.LoadPlayerData();
        canvasGameObj.transform.Find("time").GetComponent<Text>().text = playerData.time;
        canvasGameObj.transform.Find("fatigueBar").GetComponent<Slider>().value = playerData.fatigue;

        canvasGameObj.transform.Find("walkButton").GetComponent<Button>().onClick.AddListener(() => ClickWalkAndExerciseButton("散歩"));
        canvasGameObj.transform.Find("exerciseButton").GetComponent<Button>().onClick.AddListener(() => ClickWalkAndExerciseButton("運動"));

        LoadEventAndShow("EV014");
    }

    private void Update()
    {
        // 最初のTextイベント終了後
        if ("END".Equals(canvasGameObj.transform.Find("textEventEndSW").GetComponent<Text>().text))
        {
            SetButtonUI(true);
            canvasGameObj.transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text = "何をしようか?";
            canvasGameObj.transform.Find("textEventEndSW").GetComponent<Text>().text = "";
        }
    }

    public void ClickWalkAndExerciseButton(string action)
    {
        SetButtonUI(false);
        canvasGameObj.transform.Find("actionReadyAlertBox").transform.Find("Text").GetComponent<Text>().text = action + "をしながら時間を過ごしますか?";
        canvasGameObj.transform.Find("actionReadyAlertBox").gameObject.SetActive(true);

        canvasGameObj.transform.Find("actionReadyAlertBox").transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(() => ClickActionReadyAlertCancelButton());
        
    }

    public void ClickActionReadyAlertCancelButton()
    {
        Debug.Log("CALL ClickActionReadyAlertCancelButton");
        canvasGameObj.transform.Find("actionReadyAlertBox").gameObject.SetActive(false);
        SetButtonUI(true);
        // ボタンのlistenerが積もるのを防止するためにイベント後呼び出されたもとのボタンのlistenerをすべて削除する
        canvasGameObj.transform.Find("actionReadyAlertBox").transform.Find("cancelButton").GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void SetButtonUI(bool sw)
    {
        canvasGameObj.transform.Find("menuButton").GetComponent<Button>().interactable = sw;
        canvasGameObj.transform.Find("nextButton").GetComponent<Button>().interactable = sw;
        canvasGameObj.transform.Find("walkButton").gameObject.SetActive(sw);
        canvasGameObj.transform.Find("exerciseButton").gameObject.SetActive(sw);
    }
    public void LoadEventAndShow(string eventCode)
    {
        EventListData[] loadedEventListData = playerSaveDataManager.LoadedEventListData();
        EventListData eventItem = eventManager.FindEventByCode(loadedEventListData, eventCode);
        List<string[]> scriptList = eventManager.ScriptSaveToList(eventItem);
        chatManager.ShowDialogue(scriptList, eventCode);
    }
}
