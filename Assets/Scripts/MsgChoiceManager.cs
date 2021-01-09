using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgChoiceManager : MonoBehaviour
{
    public void DisplayChoiceBoxes(string eventCode)
    {
        Debug.Log("START DisplayChoiceBoxes");
        if (eventCode.Equals("EV000"))
        {
            Debug.Log("CALL");
        }
    }
}
