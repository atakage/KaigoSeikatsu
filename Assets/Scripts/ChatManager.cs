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
    public JobEventSetManager jobEventSetManager;
    public JobEventManager jobEventManager;
    public JobDiarySetManager jobDiarySetManager;
    public JobDiaryManager jobDiaryManager;
    public FlashEffectManager flashEffectManager;
    public List<string[]> textList;
    public string eventCode;
    public Dictionary<string, bool> completeEventSW; // イベントスクリプトの完了確認
    public Text panelText;
    public bool dialogueSW;
    public int clickCount;
    public int textCount;   // テキスト配列リストの配列の数
    public JobEventModel jobEvent;
    public GameObject canvasGameObj;
    public bool endTextLineBool;
    public int textOneLineLength;
    public GameObject scriptNextIconGameObj;
    public GameObject mainEventScriptNextIcon;
    public List<string> charImgFileNameList;
    public bool callCharImgFuncBool;
    // Start is called before the first frame update
    void Start()
    {
        eventCodeManager = new EventCodeManager();
        eventManager = new EventManager();
        playerSaveDataManager = new PlayerSaveDataManager();
        jobEventSetManager = new JobEventSetManager();
        jobEventManager = new JobEventManager();
        jobDiarySetManager = new JobDiarySetManager();
        jobDiaryManager = new JobDiaryManager();

        canvasGameObj = GameObject.Find("Canvas");
        panelText = GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>();

        clickCount = 0;

        if (GameObject.Find("FlashEffectManager") != null) flashEffectManager = GameObject.Find("FlashEffectManager").GetComponent("FlashEffectManager") as FlashEffectManager;
        if (canvasGameObj.transform.Find("Panel").transform.Find("scriptNextIcon") != null) scriptNextIconGameObj = canvasGameObj.transform.Find("Panel").transform.Find("scriptNextIcon").gameObject;

        Debug.Log("Start ChatManager");
        
       

    }

    private void Update()
    {
        if (dialogueSW)
        {
            // スクリプトライン一つをを全部読んだら
            if (endTextLineBool && Input.GetMouseButtonDown(0))
            {
                // Coroutine中止 && icon非表示
                StopAllCoroutines();
                if (scriptNextIconGameObj != null) scriptNextIconGameObj.SetActive(false);
                if (mainEventScriptNextIcon != null) mainEventScriptNextIcon.SetActive(false);

                Debug.Log("clickCount in nextLineBool: " + clickCount);
                Debug.Log("textCount in nextLineBool: " + textCount);
                // スクリプトを全部読んだら
                if (clickCount == textCount - 1)
                {
                    // subCharacterImageを隠す
                    if (callCharImgFuncBool) ActiveSubCharacterImage(false); 

                    if (this.eventCode != null)
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
                            executeFadeOutSimple();
                            // 終わったイベントコードをつけるオブジェクトを作る(すでに存在すると削除する)
                            CreateEndEventCodeGameObj();
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
                            CreateEndEventCodeGameObj();
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
                            executeFadeOutSimple();
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
                            // チョイスボタンをセット
                            SetChoiceButtonUIForJobEvent(true);
                            // チョイスボタンにaddListener
                            ClickChoiceButtonForJobEvent();

                            if (canvasGameObj.transform.Find("jobEventCompleteSW")) Destroy(canvasGameObj.transform.Find("jobEventCompleteSW").gameObject);
                            GameObject jobEventCompleteSW = new GameObject("jobEventCompleteSW");
                            jobEventCompleteSW.SetActive(false);
                            jobEventCompleteSW.AddComponent<Text>().text = "Y";
                            jobEventCompleteSW.transform.SetParent(canvasGameObj.transform);
                        }
                        // イベントコードがなきスクリプトだけを読み込んだとき
                    }
                    else if (eventCode == null)
                    {
                        ExitDialogue();
                        Debug.Log("only script completed");

                        if (canvasGameObj.transform.Find("onlyScriptEventEnd")) Destroy(canvasGameObj.transform.Find("onlyScriptEventEnd").gameObject);
                        GameObject onlyScriptEventEnd = new GameObject("onlyScriptEventEnd");
                        onlyScriptEventEnd.SetActive(false);
                        onlyScriptEventEnd.AddComponent<Text>().text = "END";
                        onlyScriptEventEnd.transform.SetParent(canvasGameObj.transform);
                    }
                }
                // 全体スクリプトがまだ終わってないならスクリプトCoroutineを続ける
                else
                {
                    string afterEvent = eventCodeManager.FindAfterEventByEventCode(this.eventCode);
                    // メインイベントなら専用Coroutineを続ける
                    if (afterEvent.Equals("Main Fade Out"))
                    {
                        ++clickCount;
                        StartCoroutine(StartMainEventDialogueCoroutine());
                    }
                    // メインイベントじゃないなら普通のCoroutineを続ける
                    else
                    {
                        ++clickCount;
                        StartCoroutine(StartDialogueCoroutine());
                    }

                    // 続いてcharacterimageを変える
                    if (callCharImgFuncBool) CallCharacterImage();
                }
            }
        }
    }

    public void CreateEndEventCodeGameObj()
    {
        if (GameObject.Find("Canvas").transform.Find("endedEventCode")) Destroy(GameObject.Find("Canvas").transform.Find("endedEventCode").gameObject);
        GameObject endedTextEventCode = new GameObject("endedEventCode");
        endedTextEventCode.SetActive(false);
        endedTextEventCode.AddComponent<Text>().text = eventCode;
        endedTextEventCode.transform.SetParent(GameObject.Find("Canvas").transform);
    }

    public void ClickChoiceButtonForJobEvent()
    {
        canvasGameObj.transform.Find("ChoiceButtonA").GetComponent<Button>().onClick.AddListener(delegate { ClickChoiceButtonAfterForJobEvent(this.jobEvent.choiceA, this.jobEvent.choiceAEffect, this.jobEvent.eventCode); });
        canvasGameObj.transform.Find("ChoiceButtonB").GetComponent<Button>().onClick.AddListener(delegate { ClickChoiceButtonAfterForJobEvent(this.jobEvent.choiceB, this.jobEvent.choiceBEffect, this.jobEvent.eventCode); });
        canvasGameObj.transform.Find("ChoiceButtonC").GetComponent<Button>().onClick.AddListener(delegate { ClickChoiceButtonAfterForJobEvent(this.jobEvent.choiceC, this.jobEvent.choiceCEffect, this.jobEvent.eventCode); });
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

    public void ClickChoiceButtonAfterForJobEvent(string choosingTextAndNumberValue, string choosingAfterEffect, string eventCode)
    {
        Debug.Log("Call ClickChoiceButtonAfterForJobEvent()");

        // プレイヤーデータ更新(progress, fatigue, satisfaction, feeling...)
        PlayerData playerData = playerSaveDataManager.LoadPlayerData();
        playerData.progress += 1;

        // SA-1
        string[] effectValueArray = choosingAfterEffect.Split(':');

        // 反映するeffect数くらい繰り返す
        foreach(string effectValue in effectValueArray)
        {
            Debug.Log("effectValue: " + effectValue);

            string effectName = effectValue.Substring(0, 2);
            string plusOrMinus = effectValue.Substring(2, 1);
            int effectValueInt = Int32.Parse(effectValue.Substring(3));

            Debug.Log("effectName: " + effectName + "\n" + "plusOrMinus: " + plusOrMinus + "\n" + "effectValueInt: " + effectValueInt);


            switch (effectName)
            {
                case "SA":
                    if ("+".Equals(plusOrMinus)) playerData.satisfaction += effectValueInt;
                    else playerData.satisfaction -= effectValueInt;
                    break;

                case "FA":
                    if ("+".Equals(plusOrMinus)) playerData.fatigue += effectValueInt;
                    else playerData.fatigue -= effectValueInt;
                    break;

                case "FL":
                    if ("+".Equals(plusOrMinus)) playerData.feeling += effectValueInt;
                    else playerData.feeling -= effectValueInt;
                    break;
            }
        }

        string[] choosingTextAndNumberArray = choosingTextAndNumberValue.Split(':');

        // 選択肢のvalue(-1, 0, 1)による画面effectを適用する
        switch (choosingTextAndNumberArray[1])
        {
            // -1: レッド
            case "-1":
                flashEffectManager.StartFlashEffect(new Color(255,0,0,0.1f));
                break;
            // 0: オレンジ
            case "0":
                flashEffectManager.StartFlashEffect(new Color(255, 125, 0, 0.1f));
                break;
            // 1: グリーン
            case "1":
                flashEffectManager.StartFlashEffect(new Color(0, 255, 0, 0.1f));
                break;
        }

        // JobEvent.jsonにイベントのactiveをfalse処理
        JobEventModel[] jobEventModelArray = jobEventSetManager.GetJobEventJsonFile();
        List<JobEventModel> newJobEventModelList = jobEventManager.SetEventActiveAndReturnAll(jobEventModelArray, eventCode, false);
        jobEventSetManager.CreateJobEventJsonFile(newJobEventModelList);

        // プレイヤーデータにクリアーイベントで追加する
        string[] jobEventCodeArray = playerSaveDataManager.SaveCompletedEvent(playerData.eventCodeObject.completedJobEventArray, eventCode);
        playerData.eventCodeObject.completedJobEventArray = jobEventCodeArray;

        // JobDiary.jsonファイルに記録
        // jobDiary.jsonを読み込む
        JobDiaryModel[] jobDiaryModelArray = jobDiarySetManager.GetJobDiaryJsonFile();
        // jobDiary.jsonにイベントを追加する
        List<JobDiaryModel> jobDiaryModelList = jobDiaryManager.AddEventToJobDiary(jobDiaryModelArray, eventCode, this.jobEvent.eventScript, choosingTextAndNumberArray[0]);
        jobDiarySetManager.CreateJobDiaryJsonFile(jobDiaryModelList);

        // panelに選択肢のテキストを表示する
        // スクリプトをディスプレイする
        List<string[]> scriptArrList = eventManager.SingleScriptSaveToList(choosingTextAndNumberArray[0]);
        ShowDialogue(scriptArrList, "", null);


        playerSaveDataManager.SavePlayerData(playerData);

        SetActiveChoiceButton(false);

        // 初期化
        jobEvent = null;
        canvasGameObj.transform.Find("ChoiceButtonA").GetComponent<Button>().onClick.RemoveAllListeners();
        canvasGameObj.transform.Find("ChoiceButtonB").GetComponent<Button>().onClick.RemoveAllListeners();
        canvasGameObj.transform.Find("ChoiceButtonC").GetComponent<Button>().onClick.RemoveAllListeners();
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

        ShowDialogue(scriptArrList, "", panelText);

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

    public void SetChoiceButtonUIForJobEvent(bool sw)
    {
        SetActiveChoiceButton(sw);
        SetActiveUI(false);

        GameObject.Find("ChoiceButtonA").transform.Find("Text").GetComponent<Text>().text = this.jobEvent.choiceA.Split(':')[0];
        GameObject.Find("ChoiceButtonB").transform.Find("Text").GetComponent<Text>().text = this.jobEvent.choiceB.Split(':')[0];
        GameObject.Find("ChoiceButtonC").transform.Find("Text").GetComponent<Text>().text = this.jobEvent.choiceC.Split(':')[0];
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

        // eventScriptがある場合
        if (jobEvent.eventScript != null)
        {
            charImgFileNameList = new List<string>();

            // キャライメージファイル名をリストに追加
            string[] scriptArrayPara = jobEvent.eventScript.Split('/');
            for (int i = 0; i < scriptArrayPara.Length; i++)
            {
                // characterImageファイル名がemptyならリストにnull追加
                if (string.IsNullOrEmpty(scriptArrayPara[i].Split('●')[0]))
                {
                    charImgFileNameList.Add(null);
                }
                // ファイル名があるならリストに追加する
                else
                {
                    charImgFileNameList.Add(scriptArrayPara[i].Split('●')[0]);
                    callCharImgFuncBool = true;
                }
            }
            // characterImageを変える
            if (callCharImgFuncBool) CallCharacterImage();
        }

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

    public void ShowDialogueForMainEvent(List<string[]> textList, string eventCode, string rawScript)
    {
        Debug.Log("call ShowDialogueForMainEvent: " + eventCode);
        Debug.Log("event script line: " + textList.Count);

        // 画面にメインイベントeffect
        CreateMainEventBlackBox();

        // eventScriptがある場合
        if (rawScript != null)
        {
            charImgFileNameList = new List<string>();

            // キャライメージファイル名をリストに追加
            string[] scriptArrayPara = rawScript.Split('/');
            for (int i = 0; i < scriptArrayPara.Length; i++)
            {
                // characterImageファイル名がemptyならリストにnull追加
                if (string.IsNullOrEmpty(scriptArrayPara[i].Split('●')[0]))
                {
                    charImgFileNameList.Add(null);
                }
                // ファイル名があるならリストに追加する
                else
                {
                    charImgFileNameList.Add(scriptArrayPara[i].Split('●')[0]);
                    callCharImgFuncBool = true;
                }
            }
            // characterImageを変える
            if (callCharImgFuncBool) CallCharacterImage();
        }

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

    public void ShowDialogue(List<string[]> textList, string eventCode, string rawScript)
    {
        Debug.Log("call ShowDialogue: " + eventCode);
        Debug.Log("event script line: " + textList.Count);

        // eventScriptがある場合
        if(rawScript != null)
        {
            charImgFileNameList = new List<string>();

            // キャライメージファイル名をリストに追加
            string[] scriptArrayPara = rawScript.Split('/');
            for (int i=0; i< scriptArrayPara.Length; i++)
            {
                // characterImageファイル名がemptyならリストにnull追加
                if (string.IsNullOrEmpty(scriptArrayPara[i].Split('●')[0]))
                {
                    charImgFileNameList.Add(null);
                }
                // ファイル名があるならリストに追加する
                else
                {
                    charImgFileNameList.Add(scriptArrayPara[i].Split('●')[0]);
                    callCharImgFuncBool = true;
                }
            }
            // characterImageを変える
            if(callCharImgFuncBool) CallCharacterImage();
        }
        

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

    public void ActiveDefaultCharacterImage(bool sw)
    {
        canvasGameObj.transform.Find("charaterImageBox").transform.Find("defaultCharacterImage").gameObject.SetActive(sw);
    }
    public void ActiveSubCharacterImage(bool sw)
    {
        canvasGameObj.transform.Find("charaterImageBox").transform.Find("characterImage").gameObject.SetActive(sw);
    }

    public void CallCharacterImage()
    {
        Debug.Log("call CallCharacterImage");
        // defaultとsubCharacterImage on-off
        ActiveDefaultCharacterImage(false);
        ActiveSubCharacterImage(true);

        Debug.Log("CallCharacterImage");
        Debug.Log("clickCount: " + clickCount);
        Debug.Log("charImgFileNameList.ToArray()[clickCount]: " + charImgFileNameList.ToArray()[clickCount]);
        // characterImageFileNameがないならイメージコンポーネント初期化
        if (charImgFileNameList.ToArray()[clickCount] == null)
        {
            canvasGameObj.transform.Find("charaterImageBox").transform.Find("characterImage").GetComponent<Image>().color = new Color(255, 255, 255, 0);
            canvasGameObj.transform.Find("charaterImageBox").transform.Find("characterImage").GetComponent<Image>().sprite = null;

            // fade out coroutine add
        }
        // characterImageFileNameがあるならイメージ表示
        else
        {
            canvasGameObj.transform.Find("charaterImageBox").transform.Find("characterImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("img/character/" + charImgFileNameList.ToArray()[clickCount]);
            canvasGameObj.transform.Find("charaterImageBox").transform.Find("characterImage").GetComponent<Image>().color = new Color(255, 255, 255, 255);

            // fade in coroutine add
        }
    }

    IEnumerator StartMainEventDialogueCoroutine()
    {
        Debug.Log("StartMainEventDialogueCoroutine");
        dialogueSW = true;
        endTextLineBool = false;
        GameObject.Find("mainEventLowerBlackBox").transform.Find("mainEventText").GetComponent<Text>().text = "";

        for (int i = 0; i < textList[clickCount].Length; i++)
        {
            GameObject.Find("mainEventLowerBlackBox").transform.Find("mainEventText").GetComponent<Text>().text += textList[clickCount][i];
            Debug.Log("mainEventText.text: " + GameObject.Find("mainEventLowerBlackBox").transform.Find("mainEventText").GetComponent<Text>().text);
            if (i == textList[clickCount].Length - 1)
            {
                endTextLineBool = true;
                textOneLineLength = textList[clickCount].Length;

                // Panel-TextにnextIcon(Coroutine)
                StartCoroutine(StartScriptNextIconCoroutine(true));

            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator StartDialogueCoroutine()
    {
        Debug.Log("StartDialogueCoroutine");
        dialogueSW = true;
        endTextLineBool = false;
        GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text = "";
        Debug.Log("textList.Count" + textList.Count);
        Debug.Log("clickCount in StartDialogueCoroutine" + clickCount);
        for (int i=0; i< textList[clickCount].Length; i++)
        {
            GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text += textList[clickCount][i];
            Debug.Log("panelText.text: " + GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text);
            // スクリプトの一つラインを全部読んだら
            if (i == textList[clickCount].Length - 1)
            {
                endTextLineBool = true;
                textOneLineLength = textList[clickCount].Length;

                // Panel-TextにnextIcon(Coroutine)
                StartCoroutine(StartScriptNextIconCoroutine(false));
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator StartScriptNextIconCoroutine(bool mainEvent)
    {
        switch (mainEvent)
        {
            case true:
                if (mainEventScriptNextIcon != null)
                {
                    while (true)
                    {
                        if (mainEventScriptNextIcon.activeSelf) mainEventScriptNextIcon.SetActive(false);
                        else mainEventScriptNextIcon.SetActive(true);
                        yield return new WaitForSeconds(0.6f);
                    }
                }
                break;

            case false:
                if (scriptNextIconGameObj != null)
                {
                    while (true)
                    {
                        if (scriptNextIconGameObj.activeSelf) scriptNextIconGameObj.SetActive(false);
                        else scriptNextIconGameObj.SetActive(true);
                        yield return new WaitForSeconds(0.6f);
                    }
                }
                break;
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
        mainEventScriptNextIcon = new GameObject("mainEventScriptNextIcon");

        mainEventUpperBlackBox.SetActive(true);
        mainEventLowerBlackBox.SetActive(true);
        mainEventText.SetActive(true);

        mainEventUpperBlackBox.transform.SetParent(canvasGameObj.transform);
        mainEventLowerBlackBox.transform.SetParent(canvasGameObj.transform);
        mainEventText.transform.SetParent(mainEventLowerBlackBox.transform);
        mainEventScriptNextIcon.transform.SetParent(mainEventText.transform);

        mainEventUpperBlackBox.AddComponent<Image>().color = new Color32(0,0,0,255);
        mainEventUpperBlackBox.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(canvasSizeVector.x, 380);
        mainEventUpperBlackBox.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, (Screen.height / 2) - (mainEventUpperBlackBox.transform.GetComponent<RectTransform>().sizeDelta.y / 2) );

        mainEventLowerBlackBox.AddComponent<Image>().color = new Color32(0, 0, 0, 255);
        mainEventLowerBlackBox.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(canvasSizeVector.x, 380);
        mainEventLowerBlackBox.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, ((Screen.height / 2) - (mainEventUpperBlackBox.transform.GetComponent<RectTransform>().sizeDelta.y / 2)) * -1);

        Vector2 mainEventLowerBlackBoxSize = mainEventLowerBlackBox.GetComponent<RectTransform>().sizeDelta;

        mainEventText.AddComponent<RectTransform>();
        RectTransform mainEventTextRect = mainEventText.GetComponent<RectTransform>();
        mainEventText.AddComponent<Text>();
        mainEventText.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        mainEventText.GetComponent<Text>().fontStyle = FontStyle.Bold;
        mainEventText.GetComponent<Text>().fontSize = 40;
        mainEventText.GetComponent<Text>().alignment = TextAnchor.UpperLeft;
        // 0.2f == 20%
        mainEventText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2((mainEventLowerBlackBoxSize.x) - (mainEventLowerBlackBoxSize.x * 0.2f), (mainEventLowerBlackBoxSize.y) - (mainEventLowerBlackBoxSize.y * 0.2f));

        mainEventScriptNextIcon.AddComponent<RectTransform>();
        mainEventScriptNextIcon.SetActive(false);
        RectTransform mainEventScriptNextIconRect = mainEventScriptNextIcon.GetComponent<RectTransform>();
        /*
         * Quaternion.Euler: Euler Angleの改善、Euler Angleは x,y,zの順番通り計算することになっているため
         * 2つが重なる現象が発生する場合がある重なったpositionでは正確な角度の計算が不可能(2Dは問題なし)
         * これを解決するためにQuaternion.Eulerを活用
         *
        */
        mainEventScriptNextIconRect.rotation = Quaternion.Euler(0,0,150);
        mainEventScriptNextIconRect.sizeDelta = new Vector2(35, 35);
        mainEventScriptNextIconRect.anchoredPosition = new Vector2((mainEventTextRect.rect.width/2)-10, ((mainEventTextRect.rect.height / 2)+6)*-1);
        mainEventScriptNextIcon.AddComponent<Image>();
        mainEventScriptNextIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("img/etc/icon_play");
    }
}
