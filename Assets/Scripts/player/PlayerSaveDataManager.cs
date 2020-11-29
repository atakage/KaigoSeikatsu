using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;


[System.Serializable]
public class PlayerData
{
    public int money;
    public int progressWithTestA;

    public PlayerData(int money, int progressWithTestA)
    {
        this.money = money;
        this.progressWithTestA = progressWithTestA;
    }

    public void printData()
    {
        Debug.Log("MONEY: " + money);
        Debug.Log("progressWithTestA: " + progressWithTestA);
    }
}


public class PlayerSaveDataManager : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        LoadPlayerData();
    }

    public void SavePlayerData(PlayerData playerData)
    {
        string strPlayerData = JsonConvert.SerializeObject(playerData);
        Debug.Log("SAVEDATA: " + strPlayerData.ToString());
        File.WriteAllText(Application.dataPath + "/Resources/saveData/testPlayerData.json", strPlayerData);
    }

    public PlayerData LoadPlayerData()
    {
        string dataStr = File.ReadAllText(Application.dataPath + "/Resources/saveData/testPlayerData.json");
        PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(dataStr);
        Debug.Log("LOADDATA: " + playerData.money);

        return playerData;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
