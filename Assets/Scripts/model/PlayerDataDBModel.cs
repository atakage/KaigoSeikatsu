using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDataDBModel
{
    public string name;
    public string currentScene;
    public string money;
    public EventCodeObject eventCodeObject = new EventCodeObject();
    public int progress;
    public float fatigue; // 疲労、100になったらゲームオーバー
    public int satisfaction; // 仕事の満足度, endingに影響
    public int feeling; // 気分
    public float playTime;
    public string ending;
    public bool localMode;
    public string startDate;
    public string endDate;
    public string modifiedDate; 

    public List<string> reasonList;
}
