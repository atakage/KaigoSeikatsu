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

    void Start()
    {
        csvManager = new CSVManager();
        playerSaveDataManager = new PlayerSaveDataManager();
        sceneTransitionManager = new SceneTransitionManager();

        canvasObj = GameObject.Find("Canvas");

        // コンビニで販売するアイテムをセットする(最初)
        // ConvenienceItemInit.txtにある情報をResource/saveData/ConvenienceItem.jsonに移す
        // itemSaleを変更したいときはそのSceneでConvenienceItem.jsonを読み込んで変更したあとセーブすればいい
        // ConvenienceItem.jsonがあると作らない(最初だけ作る)
        csvManager.ReadConvenienceInitFileAndCreateJson();

        // カフェで販売するアイテムをセットする
        csvManager.ReadCafeItemInitFileAndCreateJson();

        // プレイヤーデータがある場合ロードボタンを表示する
        PlayerData playerData = playerSaveDataManager.LoadPlayerData();
        if (playerData != null)
        {
            loadButtonObj = canvasObj.transform.Find("loadButton").gameObject;
            loadButtonObj.SetActive(true);
            loadButtonObj.GetComponent<Button>().onClick.AddListener(() => ClickLoadButton(playerData.currentScene));
        }
    }

    public void ClickLoadButton(string currentScene)
    {
        sceneTransitionManager.LoadTo(currentScene);
    }
}
