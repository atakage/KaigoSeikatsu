﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class ConvenienceItemSetManager : MonoBehaviour
{
    public void CreateConvenienceItem(Dictionary<string, Dictionary<string, object>> ConItemListDic)
    {
        ConvenienceItemData convenienceItemData;
        List<ConvenienceItemData> convenienceList = new List<ConvenienceItemData>();

        try
        {
            // Dictionary数くらい繰り返す
            foreach (KeyValuePair<string, Dictionary<string, object>> convenienceDic in ConItemListDic)
            {
                convenienceItemData = new ConvenienceItemData();

                Dictionary<string, object> convenienceItemDic = convenienceDic.Value;
                convenienceItemData.itemName = (string)convenienceItemDic["itemName"];
                convenienceItemData.itemPrice = (int)convenienceItemDic["itemPrice"];
                convenienceItemData.itemDescription = (string)convenienceItemDic["itemDescription"];
                convenienceItemData.itemQuantity = (int)convenienceItemDic["itemQty"];
                convenienceItemData.itemSale = (string)convenienceItemDic["itemSale"];

                convenienceList.Add(convenienceItemData);
            }

            // jsonファイルを作る
            Debug.Log("convenienceList.Count: " + convenienceList.Count);
            CreateConvenienceJsonFile(convenienceList);
        }
        catch(Exception e)
        {
            Debug.Log("ERROR: " + e.ToString());
        }


    }

    public void CreateConvenienceJsonFile(List<ConvenienceItemData> convenienceList)
    {
        string jsonStr = JsonHelper.ToJson(convenienceList.ToArray(), true);
        Debug.Log("jsonStr: " + jsonStr);
        File.WriteAllText(Application.dataPath + "/Resources/saveData/convenienceItem.json", jsonStr);
        
    }
}