using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickManager : MonoBehaviour
{
    public StreetContactManager streetContactManager;

    public void ContactButtonClick()
    {

        Debug.Log("ContactButtonClick() START");
        StreetVariableManager.actPoint -= 1;

        Text actPointText = GameObject.Find("actPointText").gameObject.GetComponent<Text>();
        actPointText.text = string.Format("行動力: {0}/10", StreetVariableManager.actPoint);

        streetContactManager = new StreetContactManager();

        // 接触するものをランダムで決める
        string getOne = streetContactManager.ShuffleGroupCList();

        // 決めたものを作る
        streetContactManager.CreateGroupCList(getOne);



    }

    public void TurnAroundButtonClick()
    {
        Debug.Log("TurnAroundButtonClick() START");
    }
}
