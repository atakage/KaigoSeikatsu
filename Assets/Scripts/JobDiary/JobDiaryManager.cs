using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class JobDiaryManager : MonoBehaviour
{
    public SceneTransitionManager sceneTransitionManager;
    public GameObject canvasGameObj;

    private void Start()
    {
        sceneTransitionManager = new SceneTransitionManager();
        canvasGameObj = GameObject.Find("Canvas");
        // 戻るボタンの目的地を設定
        if (GameObject.Find("SceneChangeManager") != null)
        {
            string goBackScene = GameObject.Find("SceneChangeManager").transform.Find("SceneChangeCanvas").transform.Find("destinationFrom-toItemCheckScene").GetComponent<Text>().text;
            canvasGameObj.transform.Find("returnButton").GetComponent<Button>().onClick.AddListener(delegate { ClickReturnButton(goBackScene); });
        }
    }

    public void ClickReturnButton(string goBackScene)
    {
        // 破壊しない場合増加する
        Destroy(GameObject.Find("SceneChangeManager"));
        sceneTransitionManager.LoadTo(goBackScene);
    }

    public List<JobDiaryModel> AddEventToJobDiary(JobDiaryModel[] jobDiaryModelArray, string eventCode, string eventScript, string choosingText)
    {
        List<JobDiaryModel> jobDiaryModelList = new List<JobDiaryModel>();

        JobDiaryModel jobDiaryModel = new JobDiaryModel();
        jobDiaryModel.eventCode = eventCode;
        jobDiaryModel.eventScript = eventScript;
        jobDiaryModel.choosingString = choosingText;

        // jobDiaryがnullなら既存イベントをリストに追加せずに続ける
        if (jobDiaryModelArray == null || jobDiaryModelArray.Length < 1)
        {
            jobDiaryModelList.Add(jobDiaryModel);
        }
        // jobDiaryにイベントがあるなら既存リストに追加
        else
        {
            jobDiaryModelList = jobDiaryModelArray.OfType<JobDiaryModel>().ToList();
            jobDiaryModelList.Add(jobDiaryModel);
        }
        return jobDiaryModelList;
    }
}
