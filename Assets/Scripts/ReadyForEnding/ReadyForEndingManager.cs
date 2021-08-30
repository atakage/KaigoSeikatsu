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
    //public int dropdownCount = 0;
    private void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        readyForEndingSharingObjectManager = GameObject.Find("ReadyForEndingSharingObjectManager").GetComponent("ReadyForEndingSharingObjectManager") as ReadyForEndingSharingObjectManager;

        readyForEndingSharingObjectManager.plusButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickPlusButton);
        readyForEndingSharingObjectManager.confirmButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickConfirmButton);
        readyForEndingSharingObjectManager.alertBoxCancelButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickAlertBoxCancelBtn);
        readyForEndingSharingObjectManager.confirmAlertBoxCancelButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickConfirmAlertBoxCancelBtn);
        readyForEndingSharingObjectManager.confirmAlertBoxConfirmButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickConfirmAlertBoxConfirmBtn);

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
            // DB作業しなくて進行
            else if ("endingB".Equals(playerData.ending))
            {

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
        readyForEndingSharingObjectManager.panelTextGameObj.GetComponent<Text>().text = "今回の仕事に向いていないと感じた原因は?";
    }
}
