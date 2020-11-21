using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestA : MonoBehaviour
{
    public Text panelText;
    public string[] taleArray;

    public void StartContact()
    {
        Debug.Log("TestA Start");

        TextAsset textAsset = Resources.Load("contact/groupC/TestA", typeof(TextAsset)) as TextAsset;
        string[] dataArray = textAsset.text.Split('/');
        DialogTextManager.instance.SetScenarios(new string[] { dataArray[0] });


        //人物と最初に接触ここで初期設定、好感度や記憶の破片なで
        Tale1();
    }


    
    public void Tale1()
    {
        Debug.Log("Tale1 START");

        TextAsset textAsset = Resources.Load("/contact/groupC/TestA/testA_tale1", typeof(TextAsset)) as TextAsset;
        taleArray = textAsset.text.Split('/');
        
        //話を始める最後に選択肢
    }


    public void Update()
    {

        //INTROスクリプトを始める

        if (Input.GetMouseButtonDown(0))
        {
            panelText = GameObject.Find("panelText").gameObject.GetComponent<Text>();
            if (panelText.text.Equals("#END"))
            {
                Debug.Log("END!!");
            }
        }
    }
}
