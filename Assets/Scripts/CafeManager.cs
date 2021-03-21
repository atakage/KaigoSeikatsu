﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class CafeManager : MonoBehaviour
{
    public Camera uiCamera;
    public PlayerSaveDataManager playerSaveDataManager;
    public EventManager eventManager;
    public ChatManager chatManager;
    public SceneTransitionManager sceneTransitionManager;
    public Vector3 cafeMenuCanvasPos;
    public Vector3 detailOrderCanvasPos;
    public PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {
        uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();

        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;
        sceneTransitionManager = new SceneTransitionManager();

        cafeMenuCanvasPos = GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").position;
        detailOrderCanvasPos = GameObject.Find("Canvas").transform.Find("DetailOrderCanvas").position;

        playerData = playerSaveDataManager.LoadPlayerData();
        GameObject.Find("time").GetComponent<Text>().text = playerData.time;
        GameObject.Find("MoneyValue").GetComponent<Text>().text = playerData.money+"円";

        GameObject.Find("Canvas").transform.Find("nextButton").GetComponent<Button>().onClick.AddListener(ClickNextButton);
        GameObject.Find("Canvas").transform.Find("NextAlertBox").transform.Find("goButton").GetComponent<Button>().onClick.AddListener(delegate { ClickNextAlertGoButton(playerData); });
        GameObject.Find("Canvas").transform.Find("NextAlertBox").transform.Find("cancleButton").GetComponent<Button>().onClick.AddListener(ClickNextAlertCancleButton);

        GameObject.Find("orderPanel").transform.Find("confirmButton").GetComponent<Button>().onClick.AddListener(ClickOrderPanelConfirmBtn);
        GameObject.Find("Canvas").transform.Find("ConfirmAlertBox").transform.Find("confirmButton").GetComponent<Button>().onClick.AddListener(delegate { ClickConfirmBtn(playerData); });
        GameObject.Find("Canvas").transform.Find("ConfirmAlertBox").transform.Find("cancleButton").GetComponent<Button>().onClick.AddListener(ClickConfirmCancleBtn);

        LoadEventAndShow("EV009");



    }

    private void Update()
    {
        // あかねさんとカフェメニューをセット
        if (GameObject.Find("Canvas").transform.Find("textEventEndSW").GetComponent<Text>().text.Equals("END"))
        {
            menuAndNextButtonInteractable(true);

            Vector3 velo = Vector3.zero;
            GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").position = Vector3.MoveTowards(GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").position, cafeMenuCanvasPos, 10f);
            GameObject.Find("Canvas").transform.Find("DetailOrderCanvas").transform.Find("DetailOrderScrollView").position = Vector3.MoveTowards(GameObject.Find("Canvas").transform.Find("DetailOrderCanvas").transform.Find("DetailOrderScrollView").position, detailOrderCanvasPos, 10f);
            // メニューのセットが終わるとSW初期化
            if (cafeMenuCanvasPos.Equals(GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").position))
            {
                GameObject.Find("Canvas").transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text = "ご注文は何にいたしますか?";
                GameObject.Find("Canvas").transform.Find("textEventEndSW").GetComponent<Text>().text = "";
            }
        }

        /*
            物体クリックイベントの準備
            1. unityで　カメラオブジェクトを追加する
            2. オブジェクトのTagを変更する
            3. オブジェクトの位置と大きさを調整する
            4. 物体にBox Collider 2Dコンポーネントを追加する
        */
        // メニューのアイテムクリックイベント
        if(GameObject.Find("Canvas").transform.Find("textEventEndSW").GetComponent<Text>().text.Equals("") && cafeMenuCanvasPos.Equals(GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").position))
        {
            // マウスクリックしたら
            if (Input.GetMouseButton(0))
            {
                // クリックした座標を取得する
                Ray ray = uiCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
                // その位置に物体があったら
                if (hit.collider != null)
                {
                    Debug.Log(hit.transform.gameObject.name);
                    // clean outline
                    CleanItemBoxOutLine();
                    // クリックしたアイテムにoutline追加
                    AddItemBoxOutLine(hit.transform.gameObject.name);
                              // panel textにアイテム説明を入れる
                    SetItemDes(hit.transform.gameObject.name);
                }
            }
        }

        // カフェを出るときの1次挨拶イベントチェック
        if(GameObject.Find("Canvas").transform.Find("greetingCheck").GetComponent<Text>().text.Equals("Y") && GameObject.Find("Canvas").transform.Find("fadeOutEventCheck").GetComponent<Text>().text.Equals("Y"))
        {
            Debug.Log("CALL 2");
            GameObject.Find("Canvas").transform.Find("fadeOutEventCheck").GetComponent<Text>().text = "";
            GameObject.Find("Canvas").transform.Find("greetingCheck").GetComponent<Text>().text = "C";
            // 2次挨拶イベントを呼び出す
            LoadEventAndShow("EV011");
            GameObject.Find("Canvas").transform.Find("greeting2Check").GetComponent<Text>().text = "Y";
        }
        // カフェを出るときの2次挨拶イベントの完了をチェック
        if (GameObject.Find("Canvas").transform.Find("greeting2Check").GetComponent<Text>().text.Equals("Y") && GameObject.Find("Canvas").transform.Find("fadeOutPersistEventCheck").GetComponent<Text>().text.Equals("Y"))
        {
            sceneTransitionManager.LoadTo("AtHomeScene");
        }
    }

    public void ClickNextAlertGoButton(PlayerData playerData)
    {
        GameObject.Find("Canvas").transform.Find("NextAlertBox").gameObject.SetActive(false);

        playerData.time = "19:00";
        playerSaveDataManager.SavePlayerData(playerData);

        GameObject.Find("Canvas").transform.Find("greetingCheck").GetComponent<Text>().text = "Y";
        GameObject.Find("Canvas").transform.Find("fadeOutEventCheck").GetComponent<Text>().text = "Y";

    }

    public void ClickNextAlertCancleButton()
    {
        menuAndNextButtonInteractable(true);
        menuAndOrderCanvasActive(true);

        GameObject.Find("Canvas").transform.Find("NextAlertBox").gameObject.SetActive(false);
    }

    public void ClickNextButton()
    {
        menuAndNextButtonInteractable(false);
        menuAndOrderCanvasActive(false);

        GameObject.Find("Canvas").transform.Find("NextAlertBox").gameObject.SetActive(true);
    }

    public void menuAndOrderCanvasActive(bool sw)
    {
        GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").gameObject.SetActive(sw);
        GameObject.Find("Canvas").transform.Find("DetailOrderCanvas").gameObject.SetActive(sw);
    }

    public void menuAndNextButtonInteractable(bool sw)
    {
        GameObject.Find("Canvas").transform.Find("menuButton").GetComponent<Button>().interactable = sw;
        GameObject.Find("Canvas").transform.Find("nextButton").GetComponent<Button>().interactable = sw;
    }

    public void ClickConfirmBtn(PlayerData playerData)
    {
        // +sound effect(money)

        GameObject.Find("Canvas").transform.Find("menuButton").GetComponent<Button>().interactable = false;
        GameObject.Find("ConfirmAlertBox").gameObject.SetActive(false);
        GameObject.Find("Money").gameObject.SetActive(false);
        GameObject.Find("MoneyValue").gameObject.SetActive(false);

        // プレイヤーデータをセーブ
        string resultMoney = GameObject.Find("Canvas").transform.Find("ConfirmAlertBox").transform.Find("resultMoney").GetComponent<Text>().text;
        playerData.money = resultMoney.Replace("円", "");
        playerData.time = "19:00";
        playerSaveDataManager.SavePlayerData(playerData);

        // fade out event(+ sound effect(dish))
        LoadEventAndShow("EV010");
        GameObject.Find("Canvas").transform.Find("greetingCheck").GetComponent<Text>().text = "Y";

    }

    public void ClickConfirmCancleBtn()
    {
        menuAndNextButtonInteractable(true);

        GameObject.Find("ConfirmAlertBox").transform.Find("confirmButton").GetComponent<Button>().interactable = true;
        GameObject.Find("Canvas").transform.Find("nextButton").GetComponent<Button>().interactable = true;
        GameObject.Find("ConfirmAlertBox").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("DetailOrderCanvas").transform.Find("DetailOrderScrollView").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text = "";
    }

    public void ClickOrderPanelConfirmBtn()
    {
        menuAndNextButtonInteractable(false);

        GameObject.Find("Canvas").transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text = "注文しますか?";
        Transform detailBackTransform = GameObject.Find("DetailOrderScrollView").transform.Find("Viewport").transform.Find("detailBack");
        // detailにアイテムが一つ以上いるのを確認する(detailSampleを除いて)
        if(detailBackTransform.childCount > 1)
        {
            GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("DetailOrderCanvas").transform.Find("DetailOrderScrollView").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("ConfirmAlertBox").gameObject.SetActive(true);

            GameObject.Find("ConfirmAlertBox").transform.Find("MoneyValue").GetComponent<Text>().text = playerData.money + "円";

            // detailItemの合計(detailSampleを除いて)
            int totalValue = 0;
            for(int i=1; i<detailBackTransform.childCount; i++)
            {
                Transform detailItem = detailBackTransform.GetChild(i);
                string itemPrice =  detailItem.transform.Find("itemPrice").GetComponent<Text>().text;
                totalValue += Int32.Parse(itemPrice.Replace("円", ""));
            }

            GameObject.Find("ConfirmAlertBox").transform.Find("TotalValue").GetComponent<Text>().text = totalValue.ToString() + "円";

            int resultMoney = Int32.Parse(playerData.money) - totalValue;
            GameObject.Find("ConfirmAlertBox").transform.Find("resultMoney").GetComponent<Text>().text = resultMoney.ToString() + "円";
            // resultMoneyがマイナスならボタンを隠す
            if (0 > resultMoney)
            {
                GameObject.Find("ConfirmAlertBox").transform.Find("confirmButton").GetComponent<Button>().interactable = false;
            }
        }
    }

    public void SetItemDes(string itemBoxName)
    {
        string itemDes = GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").transform.Find("Viewport").transform.Find("menuBack")
                            .transform.Find(itemBoxName).transform.Find("itemDescription").GetComponent<Text>().text;
        GameObject.Find("Canvas").transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text = itemDes;
    }

    public void AddItemBoxOutLine(string itemBoxName)
    {
        GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").transform.Find("Viewport").transform.Find("menuBack")
            .transform.Find(itemBoxName).GetComponent<Outline>().effectDistance = new Vector2(5, 5);
    }

    public void CleanItemBoxOutLine()
    {
        Transform menuBack = GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").transform.Find("Viewport").transform.Find("menuBack");
        int itemBoxCount = menuBack.childCount;

        for (int i=0; i<itemBoxCount; i++)
        {
            menuBack.transform.Find("itemBox" + i).GetComponent<Outline>().effectDistance = Vector2.zero;
        }
    }

    public void LoadEventAndShow(string eventCode)
    {
        EventListData[] loadedEventListData = playerSaveDataManager.LoadedEventListData();
        EventListData eventItem = eventManager.FindEventByCode(loadedEventListData, eventCode);
        List<string[]> scriptList = eventManager.ScriptSaveToList(eventItem);
        chatManager.ShowDialogue(scriptList, eventCode);
    }
}
