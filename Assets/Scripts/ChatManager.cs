using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class ChatManager : MonoBehaviour
{
    public EventCodeManager eventCodeManager;
    public EventManager eventManager;
    public PlayerSaveDataManager playerSaveDataManager;
    public List<string[]> textList;
    public string eventCode;
    public Dictionary<string, bool> completeEventSW; // イベントスクリプトの完了確認
    public Text panelText;
    public bool dialogueSW;
    public int clickCount;
    public int textCount;   // テキスト配列リストの配列の数
    public JobEventModel jobEvent;
    // Start is called before the first frame update
    void Start()
    {
        eventCodeManager = new EventCodeManager();
        eventManager = new EventManager();
        playerSaveDataManager = new PlayerSaveDataManager();

        Debug.Log("Start ChatManager");
        panelText = GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>();
        clickCount = 0;

    }

    private void Update()
    {
        if (dialogueSW)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // マウスをクリックするとclickCountが増加して次の配列テキストを読み込む
                ++clickCount;
                Debug.Log("clickCount: " + clickCount);
                Debug.Log("textCount: " + textCount);
                // スクリプト全部読んだら
                if (clickCount == textCount)
                {
                    StopAllCoroutines();
                    if(this.eventCode != null)
                    {
                        string afterEventStr = eventCodeManager.FindAfterEventByEventCode(this.eventCode);
                        if (!afterEventStr.Equals("Main Fade Out")) ExitDialogue();
                        else ExitMainEventDialogue();
                    }

                    // 普通のイベントのスクリプト終了のとき
                    if (eventCode != null)
                    {
                        // イベントスクリプトが終わったことを示す
                        if (this.completeEventSW != null) this.completeEventSW[this.eventCode] = true;

                        //イベントスクリプト後にFade OutやUI変更
                        string afterEvent = eventCodeManager.FindAfterEventByEventCode(eventCode);
                        if (afterEvent.Equals("Fade Out"))
                        {
                            executeFadeOut();
                            // 終わったイベントコードをつけるオブジェクトを作る(すでに存在すると削除する)
                            if (GameObject.Find("Canvas").transform.Find("endedEventCode")) Destroy(GameObject.Find("Canvas").transform.Find("endedEventCode").gameObject);
                            GameObject endedTextEventCode = new GameObject("endedEventCode");
                            endedTextEventCode.SetActive(false);
                            endedTextEventCode.AddComponent<Text>().text = eventCode;
                            endedTextEventCode.transform.SetParent(GameObject.Find("Canvas").transform);
                        }
                        else if (afterEvent.Equals("Choice"))
                        {
                            // チョイスイベントを読み込む
                            string choiceEvent = eventManager.GetChoiceEvent(eventCode);
                            // チョイスボタンをセット
                            SetChoiceButtonUI(choiceEvent, true);
                            // メニューボタンと進行ボタンを隠す
                            SetActiveMenuAndNextButton(false);
                            // チョイスボタンクリックイベント
                            ClickChoiceButton(eventCode);
                            // 終了信号イベントならFadeOut && UIをdisplay
                        }
                        else if (afterEvent.Equals("None"))
                        {
                            executeFadeOut();
                        }
                        else if (afterEvent.Equals("Text"))
                        {
                            GameObject.Find("Canvas").transform.Find("textEventEndSW").GetComponent<Text>().text = "END";
                            // 終わったイベントコードをつけるオブジェクトを作る(すでに存在すると削除する)
                            if (GameObject.Find("Canvas").transform.Find("endedTextEventCode")) Destroy(GameObject.Find("Canvas").transform.Find("endedTextEventCode").gameObject);
                            GameObject endedTextEventCode = new GameObject("endedTextEventCode");
                            endedTextEventCode.SetActive(false);
                            endedTextEventCode.AddComponent<Text>().text = eventCode;
                            endedTextEventCode.transform.SetParent(GameObject.Find("Canvas").transform);
                            

                        }
                        else if (afterEvent.Equals("Fade Out Persist"))
                        {
                            executeFadeOutPersist();
                            // 一つのイベントが終わったあと追加的なイベントやactionが発動
                        }
                        else if (afterEvent.Equals("Action"))
                        {
                            Debug.Log("Action Input Code: " + eventCode);
                            GameObject.Find("Canvas").transform.Find("endedActionEventCode").GetComponent<Text>().text = eventCode;
                        }
                        // メインイベントが終わると
                        else if (afterEvent.Equals("Main Fade Out"))
                        {
                            executeFadeOut();
                            // 終わったイベントコードをつけるオブジェクトを作る(すでに存在すると削除する)
                            if (GameObject.Find("Canvas").transform.Find("mainEventCompleteSW")) Destroy(GameObject.Find("Canvas").transform.Find("mainEventCompleteSW").gameObject);
                            GameObject mainEventCompleteSW = new GameObject("mainEventCompleteSW");
                            mainEventCompleteSW.SetActive(false);
                            mainEventCompleteSW.AddComponent<Text>().text = "Y";
                            mainEventCompleteSW.transform.SetParent(GameObject.Find("Canvas").transform);
                        }
                        // jobEventが終わると
                        else if (afterEvent.Equals("Job Event"))
                        {
                            Debug.Log("Job Event Next");
                        }
                    // イベントコードがなきスクリプトだけを読み込んだとき
                    }else if (eventCode == null)
                    {
                        ExitDialogue();
                        Debug.Log("only script completed");
                        GameObject.Find("Canvas").transform.Find("onlyScriptEventEnd").GetComponent<Text>().text = "END";
                    }
                }
                else
                {
                    
                    string afterEvent = eventCodeManager.FindAfterEventByEventCode(this.eventCode);
                               // メインイベントなら専用Coroutineを続ける
                    if (afterEvent.Equals("Main Fade Out"))
                    {
                        StopAllCoroutines();
                        StartCoroutine(StartMainEventDialogueCoroutine());
                    }
                               // メインイベントじゃないなら普通のCoroutineを続ける
                    else
                    {
                        StopAllCoroutines();
                        StartCoroutine(StartDialogueCoroutine());
                    }

                    
                }
            }
        }
    }

    public void ClickChoiceButton(string eventCode)
    {
        switch (eventCode)
        {
            case "ET000":
                GameObject.Find("Canvas").transform.Find("ChoiceButtonA").GetComponent<Button>().onClick.AddListener(delegate { ClickChoiceButtonAfter("satisfaction", 1, "そうか"); });
                GameObject.Find("Canvas").transform.Find("ChoiceButtonB").GetComponent<Button>().onClick.AddListener(delegate { ClickChoiceButtonAfter("fatigue", 3, "そうか/いいね/なにゃ"); });
                GameObject.Find("Canvas").transform.Find("ChoiceButtonC").GetComponent<Button>().onClick.AddListener(delegate { ClickChoiceButtonAfter("satisfaction", -1, "そうか/いいね"); });
                break;
        }
    }

    public void ClickChoiceButtonAfter(string parameter, float value, string panelText)
    {
        // チョイスボタンを隠す
        SetActiveChoiceButton(false);
        // プレイヤーデータにパラメータを適用する(parameter, value)
        PlayerData playerData = playerSaveDataManager.LoadPlayerData();
        if (parameter.Equals("progress"))
        {
            // 数値が0より大きいとプラス
            if(value > 0)
            {
                playerData.progress += (int)value;
            }
            else
            {
                value *= -1;
                playerData.progress -= (int)value;
            }
        }else if (parameter.Equals("fatigue"))
        {
            // 数値が0より大きいとプラス
            if (value > 0)
            {
                playerData.fatigue += (int)value;
            }
            else
            {
                value *= -1;
                playerData.fatigue -= (int)value;
            }
        }
        else if (parameter.Equals("satisfaction"))
        {
            // 数値が0より大きいとプラス
            if (value > 0)
            {
                playerData.satisfaction += (int)value;
            }
            else
            {
                value *= -1;
                playerData.satisfaction -= (int)value;
            }
        }
        // 時間が経つ
        SetTime();

        //プレイヤーデータをセーブ
        playerSaveDataManager.SavePlayerData(playerData);

        // スクリプトをディスプレイする
        List<string[]> scriptArrList = eventManager.SingleScriptSaveToList(panelText);

        ShowDialogue(scriptArrList, "");

        // イベントの重複呼びを防止
        this.eventCode = "EV999";
    }

    public void executeFadeOutPersist()
    {
        GameObject FadeInOutManager = new GameObject("FadeInOutManager");
        GameObject fadeObj = GameObject.Find("FadeInOutManager");
        fadeObj.AddComponent<FadeOutPersistManager>();
    }

    public void executeFadeOutSimple()
    {
        GameObject FadeInOutManager = new GameObject("FadeInOutManager");
        GameObject fadeObj = GameObject.Find("FadeInOutManager");
        fadeObj.AddComponent<SimpleFadeInOutManager>();
    }

    public void executeFadeOut()
    {
        GameObject FadeInOutManager = new GameObject("FadeInOutManager");
        GameObject fadeObj = GameObject.Find("FadeInOutManager");
        fadeObj.AddComponent<FadeInOutManager>();
    } 

    public void SetTime()
    {
        Text time = GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>();
        if (time.text.Equals("09:00"))
        {
            GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text = "11:50";
        }else if (time.text.Equals("11:50"))
        {
            GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text = "12:50";
        }else if (time.text.Equals("12:50"))
        {
            GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text = "14:00";
        }else if (time.text.Equals("14:00"))
        {
            GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text = "17:00";
        }else if (time.text.Equals("17:00"))
        {
            GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text = "17:20";
        }
        else if (time.text.Equals("17:20"))
        {
            GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text = "18:00";
        }
    }

    public void SetActiveUI(bool sw)
    {
        Debug.Log("SetactiveUI");
        GameObject.Find("Canvas").transform.Find("Image").gameObject.SetActive(sw);
        GameObject.Find("Canvas").transform.Find("time").gameObject.SetActive(sw);
        GameObject.Find("Canvas").transform.Find("fatigueText").gameObject.SetActive(sw);
        GameObject.Find("Canvas").transform.Find("fatigueBar").gameObject.SetActive(sw);
    }

    public void SetActiveMenuAndNextButton(bool sw)
    {
        GameObject.Find("Canvas").transform.Find("menuButton").gameObject.SetActive(sw);
        GameObject.Find("Canvas").transform.Find("nextButton").gameObject.SetActive(sw);
    }

    public void SetActiveChoiceButton(bool sw)
    {
        GameObject.Find("Canvas").transform.Find("ChoiceButtonA").gameObject.SetActive(sw);
        GameObject.Find("Canvas").transform.Find("ChoiceButtonB").gameObject.SetActive(sw);
        GameObject.Find("Canvas").transform.Find("ChoiceButtonC").gameObject.SetActive(sw);
    }

    public void SetChoiceButtonUI(string choiceEvent, bool sw)
    {
        string[] choiceEventArray = choiceEvent.Split('/');

        SetActiveChoiceButton(sw);
        SetActiveUI(false);

        GameObject.Find("ChoiceButtonA").transform.Find("Text").GetComponent<Text>().text = choiceEventArray[0];
        GameObject.Find("ChoiceButtonB").transform.Find("Text").GetComponent<Text>().text = choiceEventArray[1];
        GameObject.Find("ChoiceButtonC").transform.Find("Text").GetComponent<Text>().text = choiceEventArray[2];
    }

    public void ShowDialogueForJobEvent(List<string[]> textList, JobEventModel jobEvent)
    {
        Debug.Log("call ShowDialogueForJobEvent: " + jobEvent.eventCode);

        // イベントコードがあるなら(選択肢活用)
        this.completeEventSW = new Dictionary<string, bool>();
        this.eventCode = jobEvent.eventCode;
        this.completeEventSW.Add(jobEvent.eventCode, false);
        this.jobEvent = jobEvent;

        // リストにある配列の数を読み込む
        textCount = textList.Count;
        this.textList = textList;

        StartCoroutine(StartDialogueCoroutine());
    }

    public void ShowDialogueForMainEvent(List<string[]> textList, string eventCode)
    {
        Debug.Log("call ShowDialogueForMainEvent: " + eventCode);
        Debug.Log("event script line: " + textList.Count);

        // 画面にメインイベントeffect
        CreateMainEventBlackBox();

        
        // イベントコードがあるなら(選択肢活用)
        if (!eventCode.Equals(""))
        {
            this.completeEventSW = new Dictionary<string, bool>();
            this.eventCode = eventCode;
            this.completeEventSW.Add(eventCode, false);
        }
        // スクリプトだけなら
        else
        {
            // eventCodeが余ることを防止する
            this.eventCode = null;
        }
        // リストにある配列の数を読み込む
        textCount = textList.Count;
        this.textList = textList;

        StartCoroutine(StartMainEventDialogueCoroutine());
        
        
    }

    public void ShowDialogue(List<string[]> textList, string eventCode)
    {
        Debug.Log("call ShowDialogue: " + eventCode);
        Debug.Log("event script line: " + textList.Count);

        // イベントコードがあるなら(選択肢活用)
        if (!eventCode.Equals(""))
        {
            this.completeEventSW = new Dictionary<string, bool>();
            this.eventCode = eventCode;
            this.completeEventSW.Add(eventCode, false);
        }
        // スクリプトだけなら
        else
        {
            // eventCodeが余ることを防止する
            this.eventCode = null;
        }
        // リストにある配列の数を読み込む
        textCount = textList.Count;
        this.textList = textList;

        StartCoroutine(StartDialogueCoroutine());
    }

    public void ExitDialogue()
    {
        Debug.Log("ExitDialogue");
        dialogueSW = false;
        clickCount = 0;
        panelText.text = "";
    }

    public void ExitMainEventDialogue()
    {
        dialogueSW = false;
        clickCount = 0;
        GameObject.Find("mainEventText").GetComponent<Text>().text = "";
    }

    IEnumerator StartMainEventDialogueCoroutine()
    {
        Debug.Log("StartMainEventDialogueCoroutine");
        dialogueSW = true;
        GameObject.Find("mainEventLowerBlackBox").transform.Find("mainEventText").GetComponent<Text>().text = "";

        for (int i = 0; i < textList[clickCount].Length; i++)
        {
            GameObject.Find("mainEventLowerBlackBox").transform.Find("mainEventText").GetComponent<Text>().text += textList[clickCount][i];
            Debug.Log("mainEventText.text: " + GameObject.Find("mainEventLowerBlackBox").transform.Find("mainEventText").GetComponent<Text>().text);
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator StartDialogueCoroutine()
    {
        Debug.Log("StartDialogueCoroutine");
        dialogueSW = true;
        GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text = "";
        Debug.Log("textList.Count" + textList.Count);
        Debug.Log("clickCount" + clickCount);
        for (int i=0; i< textList[clickCount].Length; i++)
        {
            GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text += textList[clickCount][i];
            Debug.Log("panelText.text: " + GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void DestroyMainEventBlackBox()
    {
        GameObject canvasObj = GameObject.Find("Canvas");
        Destroy(canvasObj.transform.Find("mainEventUpperBlackBox").gameObject);
        Destroy(canvasObj.transform.Find("mainEventLowerBlackBox").gameObject);
    }

    public void CreateMainEventBlackBox()
    {
        GameObject canvasGameObj = GameObject.Find("Canvas");
        Vector2 canvasSizeVector = canvasGameObj.transform.GetComponent<RectTransform>().sizeDelta;

        GameObject mainEventUpperBlackBox = new GameObject("mainEventUpperBlackBox");
        GameObject mainEventLowerBlackBox = new GameObject("mainEventLowerBlackBox");
        GameObject mainEventText = new GameObject("mainEventText");

        mainEventUpperBlackBox.SetActive(true);
        mainEventLowerBlackBox.SetActive(true);
        mainEventText.SetActive(true);

        mainEventUpperBlackBox.transform.SetParent(canvasGameObj.transform);
        mainEventLowerBlackBox.transform.SetParent(canvasGameObj.transform);
        mainEventText.transform.SetParent(mainEventLowerBlackBox.transform);

        mainEventUpperBlackBox.AddComponent<Image>().color = new Color32(0,0,0,255);
        mainEventUpperBlackBox.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(canvasSizeVector.x, 380);
        mainEventUpperBlackBox.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, (Screen.height / 2) - (mainEventUpperBlackBox.transform.GetComponent<RectTransform>().sizeDelta.y / 2) );

        mainEventLowerBlackBox.AddComponent<Image>().color = new Color32(0, 0, 0, 255);
        mainEventLowerBlackBox.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(canvasSizeVector.x, 380);
        mainEventLowerBlackBox.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, ((Screen.height / 2) - (mainEventUpperBlackBox.transform.GetComponent<RectTransform>().sizeDelta.y / 2)) * -1);

        Vector2 mainEventLowerBlackBoxSize = mainEventLowerBlackBox.GetComponent<RectTransform>().sizeDelta;

        mainEventText.AddComponent<Text>();
        mainEventText.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        mainEventText.GetComponent<Text>().fontStyle = FontStyle.Bold;
        mainEventText.GetComponent<Text>().fontSize = 40;
        mainEventText.GetComponent<Text>().alignment = TextAnchor.UpperLeft;
        // 0.2f == 20%
        mainEventText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2((mainEventLowerBlackBoxSize.x) - (mainEventLowerBlackBoxSize.x * 0.2f), (mainEventLowerBlackBoxSize.y) - (mainEventLowerBlackBoxSize.y * 0.2f));
    }
}
