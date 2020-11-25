using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

[System.Serializable]
public class TestPlayerData
{
    public int money;
    public int progress;

    public TestPlayerData(int money, int progress)
    {
        this.money = money;
        this.progress = progress;
    }

    public void printData()
    {
        Debug.Log("MONEY: " + money);
        Debug.Log("PROGRESS: " + progress);
    }
}


public class TestPlayerSave : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        TestPlayerData testPlayerData = new TestPlayerData(15000, 60);

        JsonData dataJson = JsonMapper.ToJson(testPlayerData);

        Debug.Log("SAVEDATA: " + dataJson.ToString());
        File.WriteAllText(Application.dataPath + "/Resources/saveData/testPlayerData.json", dataJson.ToString());


        string dataStr = File.ReadAllText(Application.dataPath + "/Resources/saveData/testPlayerData.json");

        Debug.Log("LOADDATA: " + dataStr);

        JsonData dataJsonCon = JsonMapper.ToObject(dataStr);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
