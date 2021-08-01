using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MainEventModel
{
    public string eventCode;
    public int requiredProgress;
    public int requiredSatisfaction;
    public string requiredScene;
    public string requiredCompletedMainEvent;  // EV000(一つだけ)
    public string requiredCompletedJobEvent;  // EV000:EV001:EV002
    public int addingProgress;
}
