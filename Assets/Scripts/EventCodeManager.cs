using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// イベントコードの終了によるdisplayやuiを決める
public class EventCodeManager : MonoBehaviour
{
    public Dictionary<string, string> EventCodeDic;
    public List<string> parkWalkEventCodeList;

    public string FindAfterEventByEventCode(string eventCode)
    {
        // Fade Out : fade out
        EventCodeDic = new Dictionary<string, string>();
        EventCodeDic.Add("EV001", "Fade Out");
        EventCodeDic.Add("EV002", "Fade Out");
        EventCodeDic.Add("EV003", "Fade Out");
        EventCodeDic.Add("EV004", "Fade Out");
        EventCodeDic.Add("EV005", "Fade Out");
        EventCodeDic.Add("EV006", "Fade Out");
        EventCodeDic.Add("EV007", "Fade Out");
        EventCodeDic.Add("EV008", "Fade Out");
        EventCodeDic.Add("EV009", "Text");
        EventCodeDic.Add("EV010", "Fade Out");
        EventCodeDic.Add("EV011", "Fade Out Persist");
        EventCodeDic.Add("EV012", "Text");
        EventCodeDic.Add("EV013", "Fade Out Persist");
        EventCodeDic.Add("EV014", "Text");
        EventCodeDic.Add("EV015", "Action");
        EventCodeDic.Add("EV016", "Fade Out Persist");
        EventCodeDic.Add("ET000", "Choice");
        EventCodeDic.Add("EV999", "None");

        return EventCodeDic[eventCode];
    }

    public string GetParkWalkEventCode()
    {
        System.Random random = new System.Random();

        parkWalkEventCodeList = new List<string>();
        parkWalkEventCodeList.Add("EV015");

        return parkWalkEventCodeList[random.Next(0, parkWalkEventCodeList.Count)];
    }
}
