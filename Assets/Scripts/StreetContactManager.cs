﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetContactManager : MonoBehaviour
{
    public UtilManager utilManager;


    public void BeginingContact()
    {
        // 接触するものをランダムで決める
        string getOne = ShuffleGroupCList();

     // 決めたものを作る
        CreateGroupCList(getOne);
    }



    public void CreateGroupCList(string getOne)
    {

        if (getOne.Equals("TestA"))
        {
            TestA testA = new TestA();
            testA.StartContact();
        }
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
