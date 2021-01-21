using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// イベントコードの終了によるdisplayやuiを決める
public class EventCodeManager : MonoBehaviour
{
    public Dictionary<string, string> EventCodeDic;

    public string FindAfterEventByEventCode(string eventCode)
    {
        // Fade Out : fade out

        EventCodeDic = new Dictionary<string, string>();
        EventCodeDic.Add("EV001", "Fade Out");
        EventCodeDic.Add("EV002", "Fade Out");
        EventCodeDic.Add("EV003", "Fade Out");

        return EventCodeDic[eventCode];
    }
}
