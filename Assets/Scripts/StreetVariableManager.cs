using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StreetVariableManager : MonoBehaviour
{
    public Text actPointText;
    public int actPoint;

    public void AssignInitialVariable()
    {
        Debug.Log("StreetVariableManager START");
        actPoint = 10;
        actPointText = GameObject.Find("actPointText").gameObject.GetComponent<Text>();
        actPointText.text = string.Format("行動力: {0}/10", actPoint);
    }



}
