using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    public Stopwatch stopwatch;
    public bool timeCheck;
    public SceneTransitionManager sceneTransitionManager;
    public ChatManager chatManager;
    public EndingSharingObjectManager endingSharingObjectManager;
    public AdManager adManager;
    // Start is called before the first frame update
    void Start()
    {
        sceneTransitionManager = new SceneTransitionManager();
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;
        endingSharingObjectManager = GameObject.Find("EndingSharingObjectManager").GetComponent("EndingSharingObjectManager") as EndingSharingObjectManager;
        adManager = GameObject.Find("AdManager").GetComponent("AdManager") as AdManager;
        timeCheck = false;

        CallStopWatch();
    }

    private void Update()
    {
        // 4秒後にfade out
        if (stopwatch.Elapsed > TimeSpan.FromMilliseconds(3000) && timeCheck == false)
        {
            timeCheck = true;
            chatManager.executeFadeOutPersist();
        }

        // fade out後シーン転換
        if (endingSharingObjectManager.canvasGameObj.transform.Find("fadeOutPersistEventCheck") != null
         && endingSharingObjectManager.canvasGameObj.transform.Find("fadeOutPersistEventCheck").GetComponent<Text>().text.Equals("Y"))
        {
            // 2021.10.24 追加
            // titleSceneで広告on
            adManager.adSwitch = true;
            sceneTransitionManager.LoadTo("TitleScene");
        }
    }

    public void CallStopWatch()
    {
        stopwatch = new Stopwatch();
        stopwatch.Start();
    }
}
