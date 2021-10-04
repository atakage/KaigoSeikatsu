using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataToPlayerDataDBModelManager : MonoBehaviour
{
    public PlayerDataDBModel PlayerDataToDBModel(PlayerData playerData)
    {
        PlayerDataDBModel playerDataDBModel = new PlayerDataDBModel();

        playerDataDBModel.name = playerData.name;
        playerDataDBModel.currentScene = playerData.currentScene;
        playerDataDBModel.money = playerData.money;
        playerDataDBModel.eventCodeObject = playerData.eventCodeObject;
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
