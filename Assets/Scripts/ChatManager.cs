﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                    ExitDialogue();
                    // イベントスクリプトが終わったことを示す
                    if (this.completeEventSW != null) this.completeEventSW[this.eventCode] = true;

                    //イベントスクリプト後にFade OutやUI変更
                    string afterEvent = eventCodeManager.FindAfterEventByEventCode(eventCode);
                    if(afterEvent.Equals("Fade Out"))
                    {
                        GameObject FadeInOutManager = new GameObject("FadeInOutManager");
                        GameObject fadeObj = GameObject.Find("FadeInOutManager");
                        fadeObj.AddComponent<FadeInOutManager>();
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
                    }else if (afterEvent.Equals("None"))
                    {
                        GameObject FadeInOutManager = new GameObject("FadeInOutManager"); 
                        GameObject fadeObj = GameObject.Find("FadeInOutManager");
                        fadeObj.AddComponent<FadeInOutManager>();
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

    public void SetTime()
    {
        Text time = GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>();
        if (time.text.Equals("09:00"))
        {
            GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text = "11:50";
        }else if (time.text.Equals("11:50"))
        {
            GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text = "12:50";
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

    public void ShowDialogue(List<string[]> textList, string eventCode)
    {
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
