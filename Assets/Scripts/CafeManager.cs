using System.Collections;
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
    public CSVManager csvManager;
    public ItemUseManager itemUseManager;
    public Vector3 cafeMenuCanvasPos;
    public Vector3 detailOrderCanvasPos;
    public PlayerData playerData;
    public GameObject canvasGameObj;
    public string loadValueSW;

    // Start is called before the first frame update
    void Start()
    {
        canvasGameObj = GameObject.Find("Canvas");
        uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();

        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;
        sceneTransitionManager = new SceneTransitionManager();
        csvManager = new CSVManager();
        itemUseManager = new ItemUseManager();

            // TitleSceneからロードした時やMenuSceneからもどる時についてくるvalue
        if (GameObject.Find("loadValueSW") != null) loadValueSW = GameObject.Find("loadValueSW").transform.GetComponent<Text>().text;
        else loadValueSW = "N";

        cafeMenuCanvasPos = canvasGameObj.transform.Find("CafeMenuCanvas").position;
        detailOrderCanvasPos = canvasGameObj.transform.Find("DetailOrderCanvas").position;

        playerData = playerSaveDataManager.LoadPlayerData();
        GameObject.Find("time").GetComponent<Text>().text = playerData.time;
        GameObject.Find("MoneyValue").GetComponent<Text>().text = playerData.money+"円";

        canvasGameObj.transform.Find("menuButton").GetComponent<Button>().onClick.AddListener(ClickMenuButton);
        canvasGameObj.transform.Find("nextButton").GetComponent<Button>().onClick.AddListener(ClickNextButton);
        canvasGameObj.transform.Find("NextAlertBox").transform.Find("goButton").GetComponent<Button>().onClick.AddListener(delegate { ClickNextAlertGoButton(playerData); });
        canvasGameObj.transform.Find("NextAlertBox").transform.Find("cancleButton").GetComponent<Button>().onClick.AddListener(ClickNextAlertCancleButton);

        GameObject.Find("orderPanel").transform.Find("confirmButton").GetComponent<Button>().onClick.AddListener(ClickOrderPanelConfirmBtn);
        canvasGameObj.transform.Find("ConfirmAlertBox").transform.Find("confirmButton").GetComponent<Button>().onClick.AddListener(delegate { ClickConfirmBtn(playerData); });
        canvasGameObj.transform.Find("ConfirmAlertBox").transform.Find("cancleButton").GetComponent<Button>().onClick.AddListener(ClickConfirmCancleBtn);

        if(!loadValueSW.Equals("Y"))LoadEventAndShow("EV009");
    }

    private void Update()
    {
        // MenuSceneから復帰した時
        if (loadValueSW.Equals("Y"))
        {
            menuAndNextButtonInteractable(true);

            canvasGameObj.transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").position = cafeMenuCanvasPos;
            canvasGameObj.transform.Find("DetailOrderCanvas").transform.Find("DetailOrderScrollView").position = detailOrderCanvasPos;

            canvasGameObj.transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text = "ご注文は何にいたしますか?";
        }

        // あかねさんとカフェメニューをセット
        if (canvasGameObj.transform.Find("textEventEndSW").GetComponent<Text>().text.Equals("END"))
        {
            menuAndNextButtonInteractable(true);

            Vector3 velo = Vector3.zero;
            canvasGameObj.transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").position = Vector3.MoveTowards(GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").position, cafeMenuCanvasPos, 10f);
            canvasGameObj.transform.Find("DetailOrderCanvas").transform.Find("DetailOrderScrollView").position = Vector3.MoveTowards(GameObject.Find("Canvas").transform.Find("DetailOrderCanvas").transform.Find("DetailOrderScrollView").position, detailOrderCanvasPos, 10f);
            // メニューのセットが終わるとSW初期化
            if (cafeMenuCanvasPos.Equals(canvasGameObj.transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").position))
            {
                canvasGameObj.transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text = "ご注文は何にいたしますか?";
                canvasGameObj.transform.Find("textEventEndSW").GetComponent<Text>().text = "";
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
        if(canvasGameObj.transform.Find("textEventEndSW").GetComponent<Text>().text.Equals("")
            && cafeMenuCanvasPos.Equals(canvasGameObj.transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").position))
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
        if(canvasGameObj.transform.Find("greetingCheck").GetComponent<Text>().text.Equals("Y")
            && canvasGameObj.transform.Find("fadeOutEndMomentSW") != null
            && canvasGameObj.transform.Find("fadeOutEndMomentSW").GetComponent<Text>().text.Equals("Y"))
        {
            Debug.Log("CALL 2");
            canvasGameObj.transform.Find("fadeOutEventCheck").GetComponent<Text>().text = "";
            canvasGameObj.transform.Find("greetingCheck").GetComponent<Text>().text = "C";
            // 2次挨拶イベントを呼び出す
            LoadEventAndShow("EV011");
            canvasGameObj.transform.Find("greeting2Check").GetComponent<Text>().text = "Y";
        }
        // カフェを出るときの2次挨拶イベントの完了をチェック
        if (canvasGameObj.transform.Find("greeting2Check").GetComponent<Text>().text.Equals("Y")
            && canvasGameObj.transform.Find("fadeOutPersistEventCheck") != null
            && canvasGameObj.transform.Find("fadeOutPersistEventCheck").GetComponent<Text>().text.Equals("Y"))
        {
            playerData = playerSaveDataManager.LoadPlayerData();
            playerData.currentScene = "AtHomeScene";
            playerSaveDataManager.SavePlayerData(playerData);
            sceneTransitionManager.LoadTo("AtHomeScene");
        }

        // 初期化------------------------------------------------------------------------------------------------------------------
        if (canvasGameObj.transform.Find("fadeOutEndMomentSW") != null)
        {
            Destroy(canvasGameObj.transform.Find("fadeOutEndMomentSW").gameObject);
        }

        if (loadValueSW.Equals("Y"))
        {
            loadValueSW = "N";
            if (GameObject.Find("loadValueSW") != null) Destroy(GameObject.Find("loadValueSW"));
        }
    }

    public void ClickMenuButton()
    {
        sceneTransitionManager.LoadTo("MenuScene");
    }

    public void ClickNextAlertGoButton(PlayerData playerData)
    {
        LoadEventAndShow("EV011");

        canvasGameObj.transform.Find("NextAlertBox").gameObject.SetActive(false);

        playerData.time = "19:00";
        playerSaveDataManager.SavePlayerData(playerData);

        canvasGameObj.transform.Find("greeting2Check").GetComponent<Text>().text = "Y";
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

        canvasGameObj.transform.Find("NextAlertBox").gameObject.SetActive(true);
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

        // 注文したアイテム効果適用
        // 注文したアイテムをリストに追加する
        Transform detailBackTransForm = GameObject.Find("Canvas").transform.Find("DetailOrderCanvas").transform.Find("DetailOrderScrollView").transform.Find("Viewport")
            .transform.Find("detailBack");
        int detailOrderChildCount = detailBackTransForm.childCount;
        List<string> orderedItemNameList = new List<string>();

        for(int i=1; i< detailOrderChildCount; i++)
        {
            Transform detailItemTransForm = detailBackTransForm.GetChild(i);
            orderedItemNameList.Add(detailItemTransForm.transform.Find("itemName").GetComponent<Text>().text);
        }

        // ゲーム内全体アイテムリストを読み出す(key=itemName, value=itemName,itemDescription,itemEffect,key)
        Dictionary<string, Dictionary<string, object>> allItemDic = csvManager.GetTxtItemList("AllItem");

        // 使うアイテムの効果を全体アイテムリストから探して適用する
        itemUseManager.UseItem(orderedItemNameList, allItemDic);

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
        Transform detailBackTransform = GameObject.Find("DetailOrderScrollView").transform.Find("Viewport").transform.Find("detailBack");
        // detailにアイテムが一つ以上いるのを確認する(detailSampleを除いて)
        if (detailBackTransform.childCount > 1)
        {
            menuAndNextButtonInteractable(false);

            GameObject.Find("Canvas").transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text = "注文しますか?";

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
        chatManager.ShowDialogue(scriptList, eventCode, eventItem.script);
    }
}
