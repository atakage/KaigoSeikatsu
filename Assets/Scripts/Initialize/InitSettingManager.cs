using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitSettingManager : MonoBehaviour
{
    public CSVManager csvManager;

    void Start()
    {
        csvManager = new CSVManager();

        // コンビニで販売するアイテムをセットする(最初)
        // ConvenienceItemInit.txtにある情報をResource/saveData/ConvenienceItem.jsonに移す
        // itemSaleを変更したいときはそのSceneでConvenienceItem.jsonを読み込んで変更したあとセーブすればいい
        // ConvenienceItem.jsonがあると作らない(最初だけ作る)
        csvManager.ReadConvenienceInitFileAndCreateJson();
    }


}
