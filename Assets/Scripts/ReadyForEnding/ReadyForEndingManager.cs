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

        playerData = playerSaveDataManager.LoadPlayerData();

        // endingによる進行
        if ("endingA".Equals(playerData.ending))
        {
            CallEndingAProcess();
        }
        else if ("endingB".Equals(playerData.ending))
        {

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
