using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Dictionary<string, Dictionary<string, object>> GetGameItemList()
    {
        string split_re = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        string line_split_re = @"\r\n|\n\r|\n|\r";
        char[] trim_chars = { '\"' };

        string returnDicKey = "";
        Dictionary<string, Dictionary<string, object>> allItemDic = new Dictionary<string, Dictionary<string, object>>();
        
        TextAsset itemTextAsset = Resources.Load("excelData/AllItem") as TextAsset;
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
}
