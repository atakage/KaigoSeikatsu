using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UtilManager : MonoBehaviour
{
    public string GetYesOrNo()
    {
        System.Random random = new System.Random();

        // 50%確率で決める
        int randomValue = random.Next(0, 2);
        Debug.Log("GetYesOrNo() randomValue: " + randomValue);
        if (randomValue == 0)
        {
            return "NO";
        }
        else
        {
            return "YES";
        }
    }

    public DateTime TimeCal(string playerDataTime, int addingMinute)
    {
        string[] timeArray = playerDataTime.Split(':');

        DateTime dt = new DateTime();
        TimeSpan ts = new TimeSpan(Int32.Parse(timeArray[0]), Int32.Parse(timeArray[1]), 0);
        dt = dt.Date + ts;

        dt = dt.AddMinutes(addingMinute);

        return dt;
    }

    public List<string> shuffleList(List<string> list)
    {
        System.Random rnd = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            string value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }
}
