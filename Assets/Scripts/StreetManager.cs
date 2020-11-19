using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;


public class StreetManager : MonoBehaviour
{

    int index = 0;
    public SceneTransitionManager sceneTransitionManager;
    public StreetButtonManager streetButtonManager;
    public StreetVariableManager streetVariableManager;
    public StreetContactManager streetContactManager;
    
    public string textFileName;
    public string nextSceneName;
    public bool buttonOrContactStart; 
    public string buttonOrContact; // 次のパタンを決めるために
    public static bool clickSwitch; // update()内でクリックイベントが重なることを防ぐために
    
    private void Start()
    {
        
        clickSwitch = true;

        //最初はステージのボタンUIを隠す
        streetButtonManager = new StreetButtonManager();
        //streetButtonManager.SettingStageUI(false);

        //行動力を初期化
        streetVariableManager = new StreetVariableManager();
        streetVariableManager.AssignInitialVariable();


             // パタン始まりの信号
        if (buttonOrContactStart)
        {
            DecidePattern();
        }

        TextAsset textAsset = Resources.Load(textFileName, typeof(TextAsset)) as TextAsset;
        string[] dataArray = textAsset.text.Split('/');
        Debug.Log(dataArray.Length);
        DialogTextManager.instance.SetScenarios(new string[] { dataArray[index] });
        ++index;
 
    }

    private void Update()
    {
        
        TextAsset textAsset = Resources.Load(textFileName, typeof(TextAsset)) as TextAsset;
        string[] dataArray = textAsset.text.Split('/');

            if (Input.GetMouseButtonDown(0) && clickSwitch)
            {
                // スクリプトをすべて読んだら && 次のシーンがあったら
            if (index > dataArray.Length-1 && !nextSceneName.Trim().Equals(""))
                {
                 
                    //DialogTextManager.instance.DestroyObject();
                    sceneTransitionManager  = new SceneTransitionManager();
                    sceneTransitionManager.LoadTo(nextSceneName);

                // スクリプトをすべて読んだら && パタンが決めたら 
                }else if (index > dataArray.Length - 1 && !buttonOrContact.Trim().Equals(""))
                {
                    // 次のシーンがB(Button)なら
                    if (buttonOrContact.Equals("B"))
                    {
                    // 画面に歩く（進行）ボタンを作る
                    streetButtonManager = new StreetButtonManager();
                    streetButtonManager.SettingStageUI(true);

                    }
                    else//次のシーンがC(Contact)なら
                    {
                    clickSwitch = false;
                    // ランダムに何かと出会う
                    streetContactManager = new StreetContactManager();
                    streetContactManager.BeginingContact();

                    }
                // 行動力がゼロなら
                }else if (StreetVariableManager.actPoint < 1)
                    {
                        streetButtonManager = new StreetButtonManager();
                        streetButtonManager.cannotActButtonUI();

                     }
                else{ 
                        DialogTextManager.instance.SetScenarios(new string[] { dataArray[index] });
                        ++index;
                }
            }
        
    }

    // 画面に歩くボタンを作る関数
    
    // 次のシーンを決める関数
    public void DecidePattern()
    {
        // ランダムでbuttonやcontactを決定
        List<string> btnConArrayList = new List<string>(); // 7(button):3(contact)
        btnConArrayList.Add("C");
        btnConArrayList.Add("C");
        btnConArrayList.Add("C");
        btnConArrayList.Add("C");
        btnConArrayList.Add("C");
        btnConArrayList.Add("C");
        btnConArrayList.Add("C");
        btnConArrayList.Add("C");
        btnConArrayList.Add("C");
        btnConArrayList.Add("C");

        List<string> shuffledbtnConArrayList = shuffleList(btnConArrayList);
        string result = string.Join(",", shuffledbtnConArrayList);
        buttonOrContact = result.Substring(0, 1);

        Debug.Log(result);
        Debug.Log(buttonOrContact);
    }

    // 何かと出会う関数


    public List<string> shuffleList(List<string> list)
    {
        System.Random rnd = new System.Random();

        int n = list.Count;
        while(n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            string value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }



}
