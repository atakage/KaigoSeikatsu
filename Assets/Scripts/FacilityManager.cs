using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacilityManager : MonoBehaviour
{
    public PlayerSaveDataManager playerSaveDataManager;
    // Start is called before the first frame update
    void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        PlayerData playerData = playerSaveDataManager.LoadPlayerData();
        Debug.Log("playerData: " + playerData.ToString());

        // UI setting
        GameObject.Find("Canvas").transform.Find("time").GetComponent<Text>().text = playerData.time;
        GameObject.Find("Canvas").transform.Find("fatigueBar").GetComponent<Slider>().value = playerData.fatigue;
    }
}
