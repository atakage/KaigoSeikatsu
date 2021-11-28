using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVManager : MonoBehaviour
{
    public ConvenienceItemSetManager convenienceItemSetManager;
    public CafeItemSetManager cafeItemSetManager;
    public MainEventSetManager mainEventSetManager;
    public JobEventSetManager jobEventSetManager;
    // Start is called before the first frame update
    void Start()
    {
    }

    public Dictionary<string, Dictionary<string, object>> GetTxtItemList(string textFileName)
    {
        string split_re = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        string line_split_re = @"\r\n|\n\r|\n|\r";
        char[] trim_chars = { '\"' };

        string returnDicKey = "";
        Dictionary<string, Dictionary<string, object>> allItemDic = new Dictionary<string, Dictionary<string, object>>();
        
        TextAsset itemTextAsset = Resources.Load("excelData/"+ textFileName) as TextAsset;
        var lines = Regex.Split(itemTextAsset.text, line_split_re);

        if (1 > lines.Length) return allItemDic;

        var header = Regex.Split(lines[0], split_re);
        // アイテム数くらい繰り返す(lines[0]はheader, lines[1]からアイテム)
        for (var i = 1; i < lines.Length; i++)
        {
            var lineValues = Regex.Split(lines[i], split_re);
            if (lineValues.Length == 0 || lineValues[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < lineValues.Length; j++)
            {
                string lineValue = lineValues[j];
                Debug.Log("lineValue: "  + lineValue);
                lineValue = lineValue.TrimStart(trim_chars).TrimEnd(trim_chars).Replace("\\", "");
                object finalLineValue = lineValue;
                int n;
                float f;
                // もしデータが数字や素数ならそのまま入れる
                if (int.TryParse(lineValue, out n))
                {
                    finalLineValue = n;
                }
                else if (float.TryParse(lineValue, out f))
                {
                    finalLineValue = f;
                }
                entry[header[j]] = finalLineValue;

                // 'lineValues[0] = itemName'なら return Dictionaryのkeyとして込める
                if (j == 0) returnDicKey = lineValue;
            }
            allItemDic.Add(returnDicKey, entry);
            returnDicKey = "";
        }
        return allItemDic;
    }

    public void ReadJobEventInitFileAndCreateJson()
    {
        Debug.Log("call ReadJobEventInitFileAndCreateJson()");
        bool checkJsonSW = false;
        try
        {
            string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
            string filePath = folderPath + "jobEvent.json";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            File.ReadAllText(filePath);
            checkJsonSW = true;

            // jsonファイルを読み込む
            /*
            if ("window".Equals(buildMode))
            {
                File.ReadAllText(Application.dataPath + "/Resources/saveData/jobEvent.json");
                checkJsonSW = true;
            }
            else if ("android".Equals(buildMode))
            {
                // Application.dataPathはandroidで読み書きが不可能
                // Application.persistentDataPathを使う
                File.ReadAllText(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "jobEvent.json");
                checkJsonSW = true;
            }
            */
        }
        // jsonファイルがないと
        catch (Exception e)
        {
            checkJsonSW = false;
        }

        // jsonファイルがないと
        if (!checkJsonSW)
        {
            Debug.Log("craete jobEvent.json");
            // JobEventInit.txtからデータを読み込む
            Dictionary<string, Dictionary<string, object>> jobEventListDic = GetTxtItemList("JobEvent");
            // JobEvent.jsonを作る
            jobEventSetManager = new JobEventSetManager();
            jobEventSetManager.CreateJobEventJson(jobEventListDic);
        }
    }

    public void ReadMainEventInitFileAndCreateJson()
    {
        bool checkJsonSW = false;
        try
        {
            string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
            string filePath = folderPath + "mainEvent.json";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            File.ReadAllText(filePath);
            checkJsonSW = true;

            // jsonファイルを読み込む
            /*
            // 2021.10.30 修正
            // window
            if ("window".Equals(buildMode))
            {
                File.ReadAllText(Application.dataPath + "/Resources/saveData/mainEvent.json");
                checkJsonSW = true;

            // android
            }else if ("android".Equals(buildMode))
            {
                // Application.dataPathはandroidで読み書きが不可能
                // Application.persistentDataPathを使う
                File.ReadAllText(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "mainEvent.json");
                checkJsonSW = true;
            }
            */
        }
        // jsonファイルがないと
        catch (Exception e)
        {
            checkJsonSW = false;
        }

        // jsonファイルがないと
        if (!checkJsonSW)
        {
            // ConvenienceItemInit.txtからデータを読み込む
            Dictionary<string, Dictionary<string, object>> mainEventListDic = GetTxtItemList("MainEvent");
            // を作る
            mainEventSetManager = new MainEventSetManager();
            mainEventSetManager.CreateMainEventJson(mainEventListDic);
        }
    }

    public void ReadConvenienceInitFileAndCreateJson()
    {
        bool checkJsonSW = false;  
        try
        {
            string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
            string filePath = folderPath + "convenienceItem.json";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            File.ReadAllText(filePath);
            checkJsonSW = true;

            // jsonファイルを読み込む
            // 2021.10.30 修正
            // buildModeによる異なるdataPath処理
            /*
            // windowなら
            if ("window".Equals(buildMode))
            {
                File.ReadAllText(Application.dataPath + "/Resources/saveData/convenienceItem.json");
                checkJsonSW = true;
            }
            // androidなら
            else if ("android".Equals(buildMode))
            {
                // Application.dataPathはandroidで読み書きが不可能
                // Application.persistentDataPathを使う
                File.ReadAllText(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "convenienceItem.json");
                checkJsonSW = true;
            }
            */
        }
        // jsonファイルがないと
        catch (Exception e)
        {
            checkJsonSW = false;
        }

        // jsonファイルがないと
        if (!checkJsonSW)
        {
            // ConvenienceItemInit.txtからデータを読み込む
            Dictionary<string, Dictionary<string, object>> ConItemListDic = GetTxtItemList("ConvenienceItemInit");
            // を作る
            convenienceItemSetManager = new ConvenienceItemSetManager();
            convenienceItemSetManager.CreateConvenienceItem(ConItemListDic);
        }
    }

    public void CreateConvenienceJsonFile()
    {
        // ConvenienceItemInit.txtからデータを読み込む
        Dictionary<string, Dictionary<string, object>> ConItemListDic = GetTxtItemList("ConvenienceItemInit");
        // を作る
        convenienceItemSetManager = new ConvenienceItemSetManager();
        convenienceItemSetManager.CreateConvenienceItem(ConItemListDic);
    }
    
    public void ReadCafeItemInitFileAndCreateJson()
    {
        bool checkJsonSW = false;
        try
        {
            string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
            string filePath = folderPath + "cafeItem.json";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            File.ReadAllText(filePath);
            checkJsonSW = true;

            // jsonファイルを読み込む
            // 2021.10.30 修正
            // buildModeによる異なるdataPath処理
            /*
            // window
            if ("window".Equals(buildMode))
            {
                File.ReadAllText(Application.dataPath + "/Resources/saveData/cafeItem.json");
                checkJsonSW = true;
            // android
            }else if ("android".Equals(buildMode))
            {
                // Application.dataPathはandroidで読み書きが不可能
                // Application.persistentDataPathを使う
                File.ReadAllText(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "cafeItem.json");
                checkJsonSW = true;
            }
            */

        }
        // jsonファイルがないと
        catch (Exception e)
        {
            checkJsonSW = false;
        }

        // jsonファイルがないと
        if (!checkJsonSW)
        {
            // CafeItemInit.txtからデータを読み込む
            Dictionary<string, Dictionary<string, object>> CafeItemListDic = GetTxtItemList("CafeItemInit");
            // cafeItem.jsonを作る
            cafeItemSetManager = new CafeItemSetManager();
            cafeItemSetManager.CreateCafeItem(CafeItemListDic);
        }
    }
    
}
