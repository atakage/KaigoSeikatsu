using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    public SceneTransitionManager sceneTransitionManager;
    public bool clickSwitch;
    // Start is called before the first frame update
    void Start()
    {

        DialogTextManager.instance.SetScenarios(new string[] {"あなたは今から街を歩きます"});

    }




    private void Update()
    {

        if (Input.GetMouseButtonDown(0) && DialogTextManager.instance.isEnd)
        {
            if (Input.GetMouseButtonDown(0) && clickSwitch) { 
            sceneTransitionManager = new SceneTransitionManager();
            sceneTransitionManager.LoadTo("ReadyToWorkScene");
            }

            // 文字をすべて読んだあとマウスクリックで画面を転換するために
            clickSwitch = true;
        }
    }
}
