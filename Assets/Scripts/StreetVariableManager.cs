using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StreetVariableManager : MonoBehaviour
{
    public Text actPointText;
    public static int actPoint;
    public static bool decideContactingBool;
    public static bool clickSwitch; // update()内でクリックイベントが重なることを防ぐために

    public void AssignInitialVariable()
    {

        if(actPoint < 1)
        {
            actPoint = 10;
        }

        Debug.Log("StreetVariableManager START");
        
        actPointText = GameObject.Find("actPointText").gameObject.GetComponent<Text>();
        actPointText.text = string.Format("行動力: {0}/10", actPoint);
        
    }



}
