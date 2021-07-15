using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeManager : MonoBehaviour
{
    static SceneChangeManager instance;
    private void Awake()
    {
        /*
              DontDestroyOnLoadしたオブジェクトがsceneに戻るたびに増加するのを防ぐ(singleton)
        */

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        
    }
}
