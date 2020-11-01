using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class StreetManager : MonoBehaviour
{

    int index = 0;
    public SceneTransitionManager sceneTransitionManager;
    public string textFileName;
    public string nextSceneName;
    
    private void Start()
    { 
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
                // スクリプトをすべて読んだら
                if (index > dataArray.Length-1)
                {
                    //DialogTextManager.instance.DestroyObject();
                    sceneTransitionManager  = new SceneTransitionManager();
                    sceneTransitionManager.LoadTo(nextSceneName);

            }
            else { 
                    DialogTextManager.instance.SetScenarios(new string[] { dataArray[index] });
                    ++index;
                }
            }
        
    }



}
