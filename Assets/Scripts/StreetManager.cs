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
    public string textFileName;
    public string nextSceneName;
    public bool buttonOrContactStart; 
    public string buttonOrContact; // 次のパタンを決めるために
    
    private void Start()
    {
        // パタン始まりの信号
        if (buttonOrContactStart)
        {
            // ランダムでbuttonやcontactを決定
            List<string> btnConArrayList = new List<string>(); // 7(button):3(contact)
            btnConArrayList.Add("B");
            btnConArrayList.Add("B");
            btnConArrayList.Add("B");
            btnConArrayList.Add("B");
            btnConArrayList.Add("B");
            btnConArrayList.Add("B");
            btnConArrayList.Add("B");
            btnConArrayList.Add("C");
            btnConArrayList.Add("C");
            btnConArrayList.Add("C");

            List<string> shuffledbtnConArrayList = shuffleList(btnConArrayList);
            string result = string.Join(",", shuffledbtnConArrayList);
            Debug.Log(result);
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

            if (Input.GetMouseButtonDown(0))
            {
                // スクリプトをすべて読んだら && 次のシーンがあったら
                if (index > dataArray.Length-1 && nextSceneName != null)
                {
                    //DialogTextManager.instance.DestroyObject();
                    sceneTransitionManager  = new SceneTransitionManager();
                    sceneTransitionManager.LoadTo(nextSceneName);

                // スクリプトをすべて読んだら && パタンが決めたら 
                }else if (index > dataArray.Length - 1 && buttonOrContact != null)
                {

                }
                else{ 
                        DialogTextManager.instance.SetScenarios(new string[] { dataArray[index] });
                        ++index;
                }
            }
        
    }
    


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
