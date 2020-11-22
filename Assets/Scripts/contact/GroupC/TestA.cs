using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestA : MonoBehaviour
{
    public Text panelText;
    public string[] taleArray;
    public int index;

    public string stst;




    public void Start()
    {

        Debug.Log("TestA Start");

        StreetVariableManager.clickSwitch = true;
        

        TextAsset textAsset = Resources.Load("contact/groupC/testA", typeof(TextAsset)) as TextAsset;
        string[] dataArray = textAsset.text.Split('/');
        DialogTextManager.instance.SetScenarios(new string[] { dataArray[0] });


        //人物と最初に接触ここで初期設定、好感度や記憶の破片なで
        Tale1();
    }





    
    public void Tale1()
    {
        Debug.Log("Tale1 START");

        TextAsset textAsset = Resources.Load("contact/groupC/TestA/testA_tale1", typeof(TextAsset)) as TextAsset;
        taleArray = textAsset.text.Split('/');
        //話を始める最後に選択肢
    }


    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && StreetVariableManager.clickSwitch)
        {
            Debug.Log("INDEX:" + index);
            Debug.Log("taleArrayINDEX:" + taleArray.Length);
            DialogTextManager.instance.SetScenarios(new string[] { taleArray[index] });
            panelText = GameObject.Find("panelText").gameObject.GetComponent<Text>();

            // indexの増加位置を考えて最後のテキストのすぐ前で切る
            if (panelText.text.Equals("taleend"))
            {
                Debug.Log("END!!");
                //選択肢ボタンを作る、クリックスイッチをfalseにする
                StreetVariableManager.clickSwitch = false;
            }
            ++index;
        }
    }
}
