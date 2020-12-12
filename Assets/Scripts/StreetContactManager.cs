using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StreetContactManager : MonoBehaviour
{
    public UtilManager utilManager;
    public SceneTransitionManager sceneTransitionManager;


    void Start()
    {
        Debug.Log("StreetContactManager START");
        GameObject.Find("Canvas").transform.Find("turnAroundButton").GetComponent<Button>().onClick.AddListener(TurnAroundButtonClick);
        GameObject.Find("Canvas").transform.Find("contactButton").GetComponent<Button>().onClick.AddListener(TurnAroundButtonClick);
    }

    public void BeginingContact()
    {
        // ものに接近するかを決める
        DecideContacting();

        // 接触するものをランダムで決める
        // string getOne = ShuffleGroupCList();

        // 決めたものを作る
        //   CreateGroupCList(getOne);
    }



    public void CreateGroupCList(string getOne)
    {
        if (getOne.Equals("TestA"))
        {
            TestA testA = new TestA();
            GameObject.Find("Canvas").gameObject.AddComponent<TestA>();

        }
    }


    public void DecideContacting()
    {

        Debug.Log("DecideContacting() START");
        DialogTextManager.instance.SetScenarios(new string[] { "近づきますか？" });

        GameObject.Find("Canvas").transform.Find("contactButton").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("turnAroundButton").gameObject.SetActive(true);

    }

    public void ContactButtonClick()
    {
        Debug.Log("ContactButtonClick() START");

        StreetVariableManager.actPoint -= 1;

        if (StreetVariableManager.actPoint > 0)
        {
            sceneTransitionManager = new SceneTransitionManager();
            sceneTransitionManager.LoadTo("ContactingScene");
        }

    }


    public void TurnAroundButtonClick()
    {
        Debug.Log("TurnAroundButtonClick() START");
        GameObject.Find("Canvas").transform.Find("contactButton").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("turnAroundButton").gameObject.SetActive(false);

        DialogTextManager.instance.SetScenarios(new string[] { "近づくのをやめた" });

        // クリック遅延
        Invoke("ClickSwitchAvailable", 1f);

    }

    public void ClickSwitchAvailable()
    {
        Debug.Log("ClickSwitchAvailable() START");
        StreetVariableManager.clickSwitch = true;
        StreetManager.reUpdate = true;
    }

    public string ShuffleGroupCList()
    {
        Debug.Log("ShuffleGroupCList START");
        List<string> groupCList = new List<string>();

        groupCList.Add("TestA");
        groupCList.Add("TestA");

        utilManager = new UtilManager();

        List<string> shuffledgroupCList= utilManager.shuffleList(groupCList);

        string[] groupCListArray = shuffledgroupCList.ToArray();
        string getOne = (string)groupCListArray.GetValue(0);

        Debug.Log("DECIDED GROUPC: " + getOne);
        return getOne;
    }
}
