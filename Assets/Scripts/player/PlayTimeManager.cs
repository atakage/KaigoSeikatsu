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
        playerData.playTime = playTime;
        playerSaveDataManager.SavePlayerData(playerData);
    }
}
