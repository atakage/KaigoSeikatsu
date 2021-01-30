using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public EventCodeManager eventCodeManager;
    public EventManager eventManager;
    public List<string[]> textList;
    public string eventCode;
    public Dictionary<string, bool> completeEventSW; // イベントスクリプトの完了確認
    public Text panelText;
    public bool dialogueSW;
    public int clickCount;
    public int textCount;   // テキスト配列リストの配列の数
    // Start is called before the first frame update
    void Start()
    {
        eventCodeManager = new EventCodeManager();
        eventManager = new EventManager();

        Debug.Log("Start ChatManager");
        panelText = GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>();
        clickCount = 0;

        //string[] aa = {"T","E","S"," ","T","1" };
        //string[] bb = { "T", "E", "S", " ", "T", "2", "2" };
       // List<string[]> reqList = new List<string[]>();
        //reqList.Add(aa);
       // reqList.Add(bb);

       // ShowDialogue(reqList);

    }

    private void Update()
    {
        if (dialogueSW)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // マウスをクリックするとclickCountが増加して次の配列テキストを読み込む
                ++clickCount;
                if (clickCount == textCount)
                {
                    StopAllCoroutines();
                    ExitDialogue();
                    // イベントスクリプトが終わったことを示す
                    if (this.completeEventSW != null) this.completeEventSW[this.eventCode] = true;

                    //イベントスクリプト後にFade OutやUI変更
                    string afterEvent = eventCodeManager.FindAfterEventByEventCode(eventCode);
                    if(afterEvent.Equals("Fade Out"))
                    {
                        GameObject fadeObj = GameObject.Find("FadeInOutManager");
                        fadeObj.AddComponent<FadeInOutManager>();
                    }
                    else if (afterEvent.Equals("Choice"))
                    {
                        // チョイスイベントを読み込む
                        string choiceEvent = eventManager.GetChoiceEvent(eventCode);
                        // チョイスボタンをセット
                        SetChoiceButtonUI(choiceEvent);
                    }
                }
                else
                {
                    StopAllCoroutines();
                    StartCoroutine(StartDialogueCoroutine());
                }
            }
        }
    }

    public void SetChoiceButtonUI(string choiceEvent)
    {
        string[] choiceEventArray = choiceEvent.Split('/');

        GameObject.Find("Canvas").transform.Find("ChoiceButtonA").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("ChoiceButtonB").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("ChoiceButtonC").gameObject.SetActive(true);

        GameObject.Find("ChoiceButtonA").transform.Find("Text").GetComponent<Text>().text = choiceEventArray[0];
        GameObject.Find("ChoiceButtonB").transform.Find("Text").GetComponent<Text>().text = choiceEventArray[1];
        GameObject.Find("ChoiceButtonC").transform.Find("Text").GetComponent<Text>().text = choiceEventArray[2];
    }

    public void ShowDialogue(List<string[]> textList, string eventCode)
    {
        // イベントコードがあるなら(選択肢活用)
        if (!eventCode.Equals(""))
        {
            this.completeEventSW = new Dictionary<string, bool>();
            this.eventCode = eventCode;
            this.completeEventSW.Add(eventCode, false);
        }
        // リストにある配列の数を読み込む
        textCount = textList.Count;
        this.textList = textList;
        StartCoroutine(StartDialogueCoroutine());
    }

    public void ExitDialogue()
    {
        dialogueSW = false;
        clickCount = 0;
        panelText.text = "";

    }

    IEnumerator StartDialogueCoroutine()
    {
        Debug.Log("StartDialogueCoroutine");
        dialogueSW = true;
        panelText.text = "";

        for (int i=0; i< textList[clickCount].Length; i++)
        {
            panelText.text += textList[clickCount][i];
            Debug.Log("panelText.text: " + panelText.text);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
