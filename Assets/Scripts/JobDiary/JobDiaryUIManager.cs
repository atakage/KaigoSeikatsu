using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobDiaryUIManager : MonoBehaviour
{
    private JobDiarySetManager jobDiarySetManager;
    private JobDiarySharingVarManager jobDiarySharingVarManager;

    // Start is called before the first frame update
    void Start()
    {
        jobDiarySetManager = new JobDiarySetManager();

        // jobDiarySceneで共有するgameObjectなどを一つに格納して置いたclassを取得
        jobDiarySharingVarManager = GameObject.Find("JobDiarySharingVarManager").GetComponent("JobDiarySharingVarManager") as JobDiarySharingVarManager;
        JobDiaryModel[] jobDiaryModelArray = jobDiarySetManager.GetJobDiaryJsonFile();

        // jobDiary数だけscrollSnapRect内に作る
        CreateJobDiaryContent(jobDiaryModelArray);
    }

    public void CreateJobDiaryContent(JobDiaryModel[] jobDiaryModelArray)
    {
        if (jobDiaryModelArray != null && jobDiaryModelArray.Length > 0)
        {

        }
            // jobDiaryに内容がないなら
        else
        {
            Debug.Log("call CreateJobDiaryContent");
            jobDiarySharingVarManager.containerGameObj.transform.GetChild(0).Find("jobEventScript").GetComponent<Text>().text = "記録がありません";
            jobDiarySharingVarManager.containerGameObj.transform.GetChild(0).Find("jobEventChoice").GetComponent<Text>().text = "";
        }
    }
}
