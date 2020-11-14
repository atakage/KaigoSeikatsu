using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetButtonManager : MonoBehaviour
{
    public GameObject walkButton;
    public GameObject returnButton;
    public GameObject shopButton;

    public void Start()
    {

        Debug.Log("StreetButtonManager START");
        walkButton = GameObject.Find("walkButton");
        returnButton = GameObject.Find("returnButton");
        shopButton = GameObject.Find("shopButton");
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

}
