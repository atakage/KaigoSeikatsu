using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayTimeManager : MonoBehaviour
{
    public bool countPlayTime;
    public float playTime;
    static PlayTimeManager instance;
    PlayerSaveDataManager playerSaveDataManager;
    public BuildManager buildManager;
    private void Awake()
    {
        /*
            DontDestroyOnLoadしたオブジェクトがsceneに戻るたびに増加するのを防ぐ(singleton)
      */

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        buildManager = GameObject.Find("BuildManager").GetComponent("BuildManager") as BuildManager;
    }

    // Update is called once per frame
    void Update()
    {
        if(countPlayTime)
        {
            //Debug.Log(TimeSpan.FromSeconds(playTime).ToString("hh':'mm':'ss"));
            playTime += Time.deltaTime;
        }
    }

    // アプリケーションが終了されるときにプレイ時間をセーブする
    private void OnApplicationQuit()
    {
        SavePlayTimeToPlayerDataJsonFile();
    }

    public void SavePlayTimeToPlayerDataJsonFile()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        PlayerData playerData = playerSaveDataManager.LoadPlayerData();
        if(playerData != null)
        {
            playerData.playTime = playTime;
            playerSaveDataManager.SavePlayerData(playerData, buildManager.buildMode);
        }
        
    }
}
