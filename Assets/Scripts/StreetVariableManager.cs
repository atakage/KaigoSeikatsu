using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StreetVariableManager : MonoBehaviour
{
    public Text actPointText;
    public int actPoint;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("StreetVariableManager START");
        actPoint = 0;
        actPointText.text = string.Format("行動力: {0}/10", actPoint);

    }

}
