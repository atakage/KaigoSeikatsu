using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataToPlayerDataDBModelManager : MonoBehaviour
{
    public BuildManager buildManager;

    private void Start()
    {
        buildManager = GameObject.Find("BuildManager").GetComponent("BuildManager") as BuildManager;
    }

    public PlayerDataDBModel PlayerDataToDBModel(PlayerData playerData)
    {
        PlayerDataDBModel playerDataDBModel = new PlayerDataDBModel();
        JobDiarySetManager jobDiarySetManager = new JobDiarySetManager();
        PlayerSaveDataManager playerSaveDataManager = new PlayerSaveDataManager();

        playerDataDBModel.name = playerData.name;
        playerDataDBModel.currentScene = playerData.currentScene;
        playerDataDBModel.money = playerData.money;
        playerDataDBModel.eventCodeObject = playerData.eventCodeObject;

        JobDiaryModel[] jobDiaryModelArray = jobDiarySetManager.GetJobDiaryJsonFile();
        playerDataDBModel.jobDiaryModelArray = new JobDiaryModel[jobDiaryModelArray.Length];
        playerDataDBModel.jobDiaryModelArray = jobDiaryModelArray;

        ItemListData[] itemListDataArray = playerSaveDataManager.LoadItemListData(buildManager.buildMode);
        playerDataDBModel.itemListDataArray = new ItemListData[itemListDataArray.Length];
        playerDataDBModel.itemListDataArray = itemListDataArray;

        playerDataDBModel.progress = playerData.progress;
        playerDataDBModel.fatigue = playerData.fatigue;
        playerDataDBModel.satisfaction = playerData.satisfaction;
        playerDataDBModel.feeling = playerData.feeling;
        playerDataDBModel.playTime = playerData.playTime;
        playerDataDBModel.ending = playerData.ending;
        playerDataDBModel.localMode = playerData.localMode;
        playerDataDBModel.startDate = playerData.startDate;
        playerDataDBModel.endDate = playerData.endDate;
        playerDataDBModel.modifiedDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        return playerDataDBModel;
    }
}
