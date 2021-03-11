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

        cafeMenuCanvasPos = GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").position;
        detailOrderCanvasPos = GameObject.Find("Canvas").transform.Find("DetailOrderCanvas").position;

        playerData = playerSaveDataManager.LoadPlayerData();
        GameObject.Find("MoneyValue").GetComponent<Text>().text = playerData.money+"円";

        GameObject.Find("orderPanel").transform.Find("confirmButton").GetComponent<Button>().onClick.AddListener(ClickOrderPanelConfirmBtn);
        
        LoadEventAndShow("EV009");



    }

    private void Update()
    {
        // あかねさんとカフェメニューをセット
        if (GameObject.Find("Canvas").transform.Find("textEventEndSW").GetComponent<Text>().text.Equals("END"))
        {
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
    }

    public void ClickOrderPanelConfirmBtn()
    {
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
