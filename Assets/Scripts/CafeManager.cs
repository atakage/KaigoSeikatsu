using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CafeManager : MonoBehaviour
{
    public PlayerSaveDataManager playerSaveDataManager;
    public EventManager eventManager;
    public ChatManager chatManager;
    public Vector3 cafeMenuCanvasPos;

    // Start is called before the first frame update
    void Start()
    {
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
            GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").position = Vector3.SmoothDamp(GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").position, cafeMenuCanvasPos, ref velo, 0.02f);
            // メニューのセットが終わるとSW初期化
            if (GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").position.Equals(GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").position))
            {
                GameObject.Find("Canvas").transform.Find("textEventEndSW").GetComponent<Text>().text = "";
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
