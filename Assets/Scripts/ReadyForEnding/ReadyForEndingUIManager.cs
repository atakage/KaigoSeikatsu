using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyForEndingUIManager : MonoBehaviour
{
    public ReadyForEndingSharingObjectManager readyForEndingSharingObjectManager;

    private void Start()
    {
        readyForEndingSharingObjectManager = GameObject.Find("ReadyForEndingSharingObjectManager").GetComponent("ReadyForEndingSharingObjectManager") as ReadyForEndingSharingObjectManager;
        InitializationUI();
    }
    public void InitializationUI()
    {
        readyForEndingSharingObjectManager.causeDropDownBox0GameObj.transform.Find("Dropdown").GetComponent<Dropdown>().ClearOptions();
        readyForEndingSharingObjectManager.causeDropDownBox0GameObj.transform.Find("Dropdown").GetComponent<Dropdown>().AddOptions(AddOptionData());
    }

    public List<string> AddOptionData()
    {
        List<Dropdown.OptionData> optionDataList = new List<Dropdown.OptionData>();
        Dropdown.OptionData optionData = new Dropdown.OptionData();
        optionData.text = "繰り返す毎日がつまらなかった";
        Dropdown.OptionData optionData2 = new Dropdown.OptionData();
        optionData.text = "やることがなかった";
        Dropdown.OptionData optionData3 = new Dropdown.OptionData();
        optionData.text = "人間関係が欲しかった";
        Dropdown.OptionData optionData4 = new Dropdown.OptionData();
        optionData.text = "介護の仕事に興味がなかった";
        Dropdown.OptionData optionData5 = new Dropdown.OptionData();
        optionData.text = "特に不満はないが契約終了になってしまった";

        optionDataList.Add(optionData);
        optionDataList.Add(optionData2);
        optionDataList.Add(optionData3);
        optionDataList.Add(optionData4);
        optionDataList.Add(optionData5);



        List<string> optionList = new List<string>();
        optionList.Add("繰り返す毎日がつまらなかった");
        optionList.Add("やることがなかった");
        optionList.Add("人間関係が欲しかった");
        optionList.Add("介護の仕事に興味がなかった");
        optionList.Add("特に不満はないが契約終了になってしまった");


        return optionList;
    }
}
