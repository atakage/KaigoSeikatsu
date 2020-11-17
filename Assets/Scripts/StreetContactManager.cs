using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetContactManager : MonoBehaviour
{
    public UtilManager utilManager;


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
            testA.StartContact();
        }
    }


    public void DecideContacting()
    {
        Debug.Log("DecideContacting() START");
        DialogTextManager.instance.SetScenarios(new string[] {"向こうから何か感じる"});
        DialogTextManager.instance.SetScenarios(new string[] { "近づきますか？" });

        GameObject.Find("Canvas").transform.Find("contactButton").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("turnAroundButton").gameObject.SetActive(true);
    }




    public string ShuffleGroupCList()
    {
        Debug.Log("ShuffleGroupCList START");
        List<string> groupCList = new List<string>();

        groupCList.Add("TestA");
        groupCList.Add("TestB");

        utilManager = new UtilManager();

        List<string> shuffledgroupCList= utilManager.shuffleList(groupCList);

        string[] groupCListArray = shuffledgroupCList.ToArray();
        string getOne = (string)groupCListArray.GetValue(0);

        Debug.Log("DECIDED GROUPC: " + getOne);
        return getOne;
    }
}
