using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CafeManager : MonoBehaviour
{
    public Camera uiCamera;
    public PlayerSaveDataManager playerSaveDataManager;
    public EventManager eventManager;
    public ChatManager chatManager;
    public Vector3 cafeMenuCanvasPos;

    // Start is called before the first frame update
    void Start()
    {
        uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();

        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;

        cafeMenuCanvasPos = GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").position;

        LoadEventAndShow("EV009");

    }

    private void Update()
    {
        // あかねさんとカフェメニューをセット
        if (GameObject.Find("Canvas").transform.Find("textEventEndSW").GetComponent<Text>().text.Equals("END"))
        {
            Vector3 velo = Vector3.zero;
            GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").position = Vector3.MoveTowards(GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").position, cafeMenuCanvasPos, 10f);
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

                    // クリックしたアイテムにoutline追加
                }
            }
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
