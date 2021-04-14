using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvenienceUIManager : MonoBehaviour
{
    public void FirstUISetting(ConvenienceItemData[] convenienceItemDataArray)
    {
        GameObject canvasGameObj = GameObject.Find("Canvas");

        GameObject itemBox = canvasGameObj.transform.Find("menuBox")
            .transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content")
            .transform.Find("itemBox0").gameObject;

        itemBox = Instantiate(itemBox);
        itemBox.transform.SetParent(canvasGameObj.transform.Find("menuBox").transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content"));

    }
}
