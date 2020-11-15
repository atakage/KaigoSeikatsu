using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestA : MonoBehaviour
{

    public void StartContact()
    {
        Debug.Log("TestA Start");

        TextAsset textAsset = Resources.Load("contact/groupC/TestA", typeof(TextAsset)) as TextAsset;
        string[] dataArray = textAsset.text.Split('/');
        DialogTextManager.instance.SetScenarios(new string[] { dataArray[0] });
    }
}
