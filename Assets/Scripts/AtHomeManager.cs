using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtHomeManager : MonoBehaviour
{
    public SceneTransitionManager sceneTransitionManager;
    public string morningOrNight;

    private void Start()
    {
        morningOrNight = "morning";
        

        // 朝なら出勤する、夜なら寝るにボタン変更
        GameObject.Find("nextButton").transform.Find("Text").GetComponent<Text>().text 
                           = (morningOrNight.Equals("morning")) ? "出勤する" : "寝る";

        GameObject.Find("Canvas").transform.Find("nextButton").GetComponent<Button>().onClick.AddListener(ClickNextButton);
        GameObject.Find("Canvas").transform.Find("itemCheckButton").GetComponent<Button>().onClick.AddListener(ClickItemCheckButton);



    }

    public void ClickNextButton()
    {
        sceneTransitionManager = new SceneTransitionManager();

        // nextButton名によるシーン変更
        if (GameObject.Find("nextButton").transform.Find("Text").GetComponent<Text>().text.Equals("出勤する"))
        {
            sceneTransitionManager.LoadTo("FacilityScene");
        }
        else
        {
            sceneTransitionManager.LoadTo("");
        }
    }

    public void ClickItemCheckButton()
    {
        sceneTransitionManager = new SceneTransitionManager();
        sceneTransitionManager.LoadTo("ItemCheckScene");
    }
}
