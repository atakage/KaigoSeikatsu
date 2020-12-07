using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StreetButtonManager : MonoBehaviour
{
    public StreetManager streetManager;
    public GameObject walkButton;
    public GameObject returnButton;
    public GameObject shopButton;

    public void Start()
    {
        Debug.Log("StreetButtonManager START");
        GameObject.Find("Canvas").transform.Find("walkButton").GetComponent<Button>().onClick.AddListener(WalkButtonClick);
    }

    public void WalkButtonClick()
    {
        Debug.Log("WalkButtonClick START");
        SettingStageUI(false);
        DialogTextManager.instance.SetScenarios(new string[] { "街を歩く" });
        StreetManager.reUpdate = true;
    }


    //ステージのボタンUI setting
    public void SettingStageUI(bool hideOrDisplay)
    {
        Debug.Log("SettingStageUI() START");

        // unityではSetActive(false)状態のオブジェクトは探せないので親オブジェクトから接近する
        
        GameObject.Find("Canvas").transform.Find("walkButton").gameObject.SetActive(hideOrDisplay);
        GameObject.Find("Canvas").transform.Find("returnButton").gameObject.SetActive(hideOrDisplay);
        GameObject.Find("Canvas").transform.Find("shopButton").gameObject.SetActive(hideOrDisplay);
    }


    //行動力がゼロになったときボタンUI
    public void cannotActButtonUI()
    {
        Debug.Log("cannotActButtonUI START");

        
        

        // 歩くボタンの色を変える
        Button walkButtonObj = GameObject.Find("Canvas").transform.Find("walkButton").gameObject.GetComponent<Button>();
        var colors = walkButtonObj.colors;
        colors.normalColor = new Color(255,0,0,1);
        colors.highlightedColor = new Color(255, 0, 0, 1);
        colors.pressedColor = new Color(255, 0, 0, 1);
        colors.selectedColor = new Color(255, 0, 0, 1);
        colors.disabledColor = new Color(255, 0, 0, 1);
        walkButtonObj.colors = colors;


        // 歩くボタンの動作を止める
        walkButtonObj.interactable = false;

        SettingStageUI(true);

    }
}
