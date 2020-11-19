using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickManager : MonoBehaviour
{
    public StreetContactManager streetContactManager;
    public SceneTransitionManager sceneTransitionManager;

    public void ContactButtonClick()
    {


        Debug.Log("ContactButtonClick() START");

        StreetVariableManager.actPoint -= 1;

        if(StreetVariableManager.actPoint > 0)
        {
            sceneTransitionManager = new SceneTransitionManager();
            sceneTransitionManager.LoadTo("ContactingScene");
        }

    }

    public void TurnAroundButtonClick()
    {
        Debug.Log("TurnAroundButtonClick() START");
        GameObject.Find("Canvas").transform.Find("contactButton").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("turnAroundButton").gameObject.SetActive(false);
        
        DialogTextManager.instance.SetScenarios(new string[] { "近づくのをやめた" });

        // クリック遅延
        Invoke("ClickSwitchAvailable", 1f);
  
    }




    public void ClickSwitchAvailable()
    {
        Debug.Log("ClickSwitchAvailable() START");
        StreetManager.clickSwitch = true;
    }
}
