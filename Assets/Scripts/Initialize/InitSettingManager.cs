using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitSettingManager : MonoBehaviour
{
    public CSVManager csvManager;
    public PlayerSaveDataManager playerSaveDataManager;
    public SceneTransitionManager sceneTransitionManager;
    public GameObject canvasObj;
    public GameObject loadButtonObj;
    public GameObject newGameAlertBoxObj;
    public int msgCheckIntVal;
    void Start()
    {
        csvManager = new CSVManager();
        playerSaveDataManager = new PlayerSaveDataManager();
        sceneTransitionManager = new SceneTransitionManager();

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

        
        PlayerData playerData = playerSaveDataManager.LoadPlayerData();

        canvasObj.transform.Find("PlayButton").GetComponent<Button>().onClick.AddListener(() => ClickPlayButton(playerData));
        newGameAlertBoxObj.transform.Find("confirmButton").GetComponent<Button>().onClick.AddListener(() => ClickNewGameAlertBoxConfirmBtn(this.msgCheckIntVal));
        newGameAlertBoxObj.transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(() => ClickNewGameAlertBoxCancelBtn());

        // プレイヤーデータがある場合ロードボタンを表示する
        if (playerData != null)
        {
            loadButtonObj = canvasObj.transform.Find("loadButton").gameObject;
            loadButtonObj.SetActive(true);
            loadButtonObj.GetComponent<Button>().onClick.AddListener(() => ClickLoadButton(playerData.currentScene));
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

        }

    }

    public void ClickLoadButton(string currentScene)
    {
        sceneTransitionManager.LoadTo(currentScene);
    }
}
