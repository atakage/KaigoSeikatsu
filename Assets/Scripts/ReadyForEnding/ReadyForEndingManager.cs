using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ReadyForEndingManager : MonoBehaviour
{
    public PlayerSaveDataManager playerSaveDataManager;
    public PlayerData playerData;
    public ReadyForEndingSharingObjectManager readyForEndingSharingObjectManager;
    public EventManager eventManager;
    public ChatManager chatManager;
    public SceneTransitionManager sceneTransitionManager;
    public GameClearFileManager gameClearFileManager;
    //public int dropdownCount = 0;
    private void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        eventManager = new EventManager();
        readyForEndingSharingObjectManager = GameObject.Find("ReadyForEndingSharingObjectManager").GetComponent("ReadyForEndingSharingObjectManager") as ReadyForEndingSharingObjectManager;
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;
        sceneTransitionManager = new SceneTransitionManager();
        gameClearFileManager = new GameClearFileManager();

        readyForEndingSharingObjectManager.plusButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickPlusButton);
        readyForEndingSharingObjectManager.confirmButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickConfirmButton);
        readyForEndingSharingObjectManager.alertBoxCancelButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickAlertBoxCancelBtn);
        readyForEndingSharingObjectManager.confirmAlertBoxCancelButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickConfirmAlertBoxCancelBtn);
        readyForEndingSharingObjectManager.confirmAlertBoxConfirmButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickConfirmAlertBoxConfirmBtn);
        readyForEndingSharingObjectManager.continueAlertCancelButtonBoxGameObj.GetComponent<Button>().onClick.AddListener(ClickContinueAlertBoxCancelBtn);
        readyForEndingSharingObjectManager.continueAlertConfirmButtonBoxGameObj.GetComponent<Button>().onClick.AddListener(ClickContinueAlertBoxConfirmBtn);

        playerData = playerSaveDataManager.LoadPlayerData();

        // localModeだったら
        if (playerData.localMode)
        {
            // DB作業しなくて進行
            if ("endingA".Equals(playerData.ending))
            {

            }
            // DB作業しなくて進行
            else if ("endingB".Equals(playerData.ending))
            {
                // continueAlertBoxを表示
            }
        }
        // localModeじゃなかったら(online)
        else
        {
            // endingによる進行(DB作業)
            if ("endingA".Equals(playerData.ending))
            {
                CallEndingAProcess();
                // surveyBox表示
                readyForEndingSharingObjectManager.surveyBoxGameObj.SetActive(true);
            }
            
            else if ("endingB".Equals(playerData.ending))
            {
                // continueAlertBoxを表示
                SetActiveContinueAlertBox(true);
            }
        }


    }

    private void Update()
    {
        // dropdownが３つ以上なら原因追加ボタンを防ぐ
        if (readyForEndingSharingObjectManager.dropDownBoxGameObj.transform.childCount > 2)
        {
            readyForEndingSharingObjectManager.plusButtonGameObj.GetComponent<Button>().interactable = false;
        }
        else
        {
            readyForEndingSharingObjectManager.plusButtonGameObj.GetComponent<Button>().interactable = true;
        }

        // for ending
        if(readyForEndingSharingObjectManager.canvasGameObj.transform.Find("fadeOutPersistEventCheck") != null
        && readyForEndingSharingObjectManager.canvasGameObj.transform.Find("fadeOutPersistEventCheck").GetComponent<Text>().text.Equals("Y")
        && readyForEndingSharingObjectManager.canvasGameObj.transform.Find("endedEventCode") != null
        && readyForEndingSharingObjectManager.canvasGameObj.transform.Find("endedEventCode").GetComponent<Text>().text.Equals("EV028"))
        {
            sceneTransitionManager.LoadTo("EndingScene");
        }
    }

    public void AddDropdown()
    {
        // dropdownオブジェクトをコピー
        GameObject copiedCauseDropDownBoxGameObj = Instantiate(readyForEndingSharingObjectManager.causeDropDownBox0GameObj);


        copiedCauseDropDownBoxGameObj.name = "causeDropDownBox";

        Debug.Log("final: " + readyForEndingSharingObjectManager.dropDownBoxGameObj.transform.GetChild(readyForEndingSharingObjectManager.dropDownBoxGameObj.transform.childCount-1).name);
        Debug.Log("final: " + readyForEndingSharingObjectManager.dropDownBoxGameObj.transform.GetChild(readyForEndingSharingObjectManager.dropDownBoxGameObj.transform.childCount - 1).name.Substring(16));

        copiedCauseDropDownBoxGameObj.transform.Find("causeMinusButton").GetComponent<Button>().interactable = true;
        // dropdown削除イベント追加
        copiedCauseDropDownBoxGameObj.transform.Find("causeMinusButton").GetComponent<Button>().onClick.AddListener(() => ClickCauseMinusButton(copiedCauseDropDownBoxGameObj));
        copiedCauseDropDownBoxGameObj.transform.SetParent(readyForEndingSharingObjectManager.dropDownBoxGameObj.transform);

    }

    public bool CheckDropdown()
    {
        bool optionDataCheckResult = true;

        List<string> optionList = new List<string>();
        for (int i=0; i< readyForEndingSharingObjectManager.dropDownBoxGameObj.transform.childCount; i++)
        {
            string optionData = readyForEndingSharingObjectManager.dropDownBoxGameObj.transform.GetChild(i).transform.Find("Dropdown").transform.Find("Label").GetComponent<Text>().text;

            // リストにoptionDataがあると重複
            if (optionList.Contains(optionData))
            {
                optionDataCheckResult = false;
                break;
            }
            else
            {
                optionList.Add(optionData);
            }
            optionData = null;
        }
        return optionDataCheckResult;
    }

    public void SetActiveContinueAlertBox(bool sw)
    {
        readyForEndingSharingObjectManager.continueAlertBoxGameObj.SetActive(sw);
    }

    public void SetActiveSurveyBox(bool sw)
    {
        readyForEndingSharingObjectManager.surveyBoxGameObj.SetActive(sw);
    }

    public void SetActiveAlertBox(bool sw)
    {
        readyForEndingSharingObjectManager.alertBoxGameObj.SetActive(sw);
    }

    public void SetActiveConfirmAlertBox(bool sw)
    {
        readyForEndingSharingObjectManager.confirmAlertBoxGameObj.SetActive(sw);
    }

    public void ClickContinueAlertBoxConfirmBtn()
    {
        // ゲームエンディングを記録するファイルを作る
        gameClearFileManager.SaveGameClearFile();
        SetActiveContinueAlertBox(false);
        LoadEventAndShow("EV028");
    }

    public void ClickContinueAlertBoxCancelBtn()
    {
        SetActiveContinueAlertBox(false);
        CallEndingAProcess();
        SetActiveSurveyBox(true);
    }

    public void ClickConfirmAlertBoxConfirmBtn()
    {
        // DB作業

    }

    public void ClickConfirmAlertBoxCancelBtn()
    {
        SetActiveSurveyBox(true);
        SetActiveConfirmAlertBox(false);
    }

    public void ClickAlertBoxCancelBtn()
    {
        SetActiveSurveyBox(true);
        SetActiveAlertBox(false);
    }

    public void ClickConfirmButton()
    {
        // dropdownのoptionDataをチェックする(重複禁止)
        bool optionDataCheckResult = CheckDropdown();
        // optionDataが重複なら
        if (!optionDataCheckResult)
        {
            SetActiveSurveyBox(false);
            SetActiveAlertBox(true);
        }
        // optionDataが重複じゃないなら
        else
        {
            SetActiveSurveyBox(false);
            SetActiveConfirmAlertBox(true);
        }
    }

    public void ClickCauseMinusButton(GameObject copiedCauseDropDownBoxGameObj)
    {
        Destroy(copiedCauseDropDownBoxGameObj);
    }

    public void ClickPlusButton()
    {
        AddDropdown();
    }

    public void CallEndingAProcess()
    {
        readyForEndingSharingObjectManager.panelTextGameObj.GetComponent<Text>().text = "今回の仕事に向いていないと感じた理由は?";
    }

    public void LoadEventAndShow(string eventCode)
    {
        EventListData[] loadedEventListData = playerSaveDataManager.LoadedEventListData();
        EventListData eventItem = eventManager.FindEventByCode(loadedEventListData, eventCode);
        List<string[]> scriptList = eventManager.ScriptSaveToList(eventItem);
        // 2021.07.26 修正, キャライメージ追加されたrawScriptをparameterに渡す
        chatManager.ShowDialogue(scriptList, eventCode, eventItem.script);
    }
}
