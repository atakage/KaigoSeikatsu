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






}
