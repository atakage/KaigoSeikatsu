using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class CafeItemSetManager : MonoBehaviour
{
    public void CreateCafeItem(Dictionary<string, Dictionary<string, object>> cafeItemListDic)
    {
        ConvenienceItemData cafeItemData;
        List<ConvenienceItemData> cafeList = new List<ConvenienceItemData>();

        try
        {
            // Dictionary数くらい繰り返す
            foreach (KeyValuePair<string, Dictionary<string, object>> convenienceDic in cafeItemListDic)
            {
                cafeItemData = new ConvenienceItemData();

                Dictionary<string, object> convenienceItemDic = convenienceDic.Value;
                cafeItemData.itemName = (string)convenienceItemDic["itemName"];
                cafeItemData.itemPrice = (int)convenienceItemDic["itemPrice"];
                cafeItemData.itemDescription = (string)convenienceItemDic["itemDescription"];
                cafeItemData.itemQuantity = (int)convenienceItemDic["itemQty"];
                cafeItemData.itemSale = (string)convenienceItemDic["itemSale"];

                // アイテムネームでイメージファイルがあるかチェックしてそのpathを獲得しておく
                cafeItemData.itemImagePath = GetItemImagePath(cafeItemData.itemName);

                cafeList.Add(cafeItemData);
            }

            // jsonファイルを作る
            Debug.Log("cafeList.Count: " + cafeList.Count);
            CreateCafeJsonFile(cafeList);
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.ToString());
        }
    }

    public void CreateCafeJsonFile(List<ConvenienceItemData> cafeList)
    {
        string jsonStr = JsonHelper.ToJson(cafeList.ToArray(), true);
        Debug.Log("jsonStr: " + jsonStr);

        string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
        string filePath = folderPath + "cafeItem.json";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        File.Create(filePath).Close();
        File.WriteAllText(filePath, jsonStr);

        /*
        if ("window".Equals(buildMode))
        {
            File.WriteAllText(Application.dataPath + "/Resources/saveData/cafeItem.json", jsonStr);
        }
        else if ("android".Equals(buildMode))
        {
            string androidFolderPath = Application.persistentDataPath + "/Resources/saveData/";
            string androidFilePath = androidFolderPath + "cafeItem.json";
            if (!Directory.Exists(androidFolderPath))
            {
                Directory.CreateDirectory(androidFolderPath);
            }
            
            Debug.Log("androidFolderPath: " + androidFolderPath);
            File.WriteAllText(androidFilePath, jsonStr);
        }
        */
    }

    public string GetItemImagePath(string itemName)
    {
        string imagePath = "img/item/" + itemName;
        Texture2D texture = Resources.Load(imagePath, typeof(Texture2D)) as Texture2D;

        // イメージがないならdefaultイメージを設定
        if (texture == null)
        {
            imagePath = "img/item/unity";
        }

        return imagePath;
    }
}
