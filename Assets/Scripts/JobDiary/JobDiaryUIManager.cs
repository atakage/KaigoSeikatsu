using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobDiaryUIManager : MonoBehaviour
{
    //private JobDiarySetManager jobDiarySetManager;
    private JobDiarySharingVarManager jobDiarySharingVarManager;


    private void Awake()
    {
        Debug.Log("call JobDiarySharingVarManager");
        // jobDiarySceneで共有するgameObjectなどを一つに格納して置いたclassを取得
        jobDiarySharingVarManager = GameObject.Find("JobDiarySharingVarManager").GetComponent("JobDiarySharingVarManager") as JobDiarySharingVarManager;
    }

    // jobDiary数だけscrollSnapRect内に作る
    public void CreateJobDiaryContent()
    {
        Debug.Log("call CreateJobDiaryContent");
        JobDiarySetManager jobDiarySetManager = new JobDiarySetManager();
        JobDiaryModel[] jobDiaryModelArray = jobDiarySetManager.GetJobDiaryJsonFile();

        // jobDiaryに内容があると
        if (jobDiaryModelArray != null && jobDiaryModelArray.Length > 0)
        {
            //  内容だけ繰り返す
            for(int i=0; i<jobDiaryModelArray.Length; i++)
            {
                // 最初はdefaultObjectにセッティング
                if (i == 0)
                {
                    string[] scriptSplitedSlashArray = jobDiaryModelArray[i].eventScript.Split('/');
                    string script = null;
                    foreach(string scriptSplitedSlash in scriptSplitedSlashArray)
                    {
                        script += scriptSplitedSlash.Split('●')[1];
                    }
                    jobDiarySharingVarManager.containerGameObj.transform.GetChild(0).Find("jobEventScript").GetComponent<Text>().text = script;
                    jobDiarySharingVarManager.containerGameObj.transform.GetChild(0).Find("jobEventChoice").GetComponent<Text>().text = jobDiaryModelArray[i].choosingString.Replace("/", "");
                }
                // 二番目からはobjectを作る
                else
                {
                    // objectを作る
                    GameObject itemGameObj = Instantiate(jobDiarySharingVarManager.containerGameObj.transform.GetChild(0).gameObject);
                    itemGameObj.name = "item" + i;
                    itemGameObj.transform.SetParent(jobDiarySharingVarManager.containerGameObj.transform);

                    string[] scriptSplitedSlashArray = jobDiaryModelArray[i].eventScript.Split('/');
                    string script = null;
                    foreach (string scriptSplitedSlash in scriptSplitedSlashArray)
                    {
                        script += scriptSplitedSlash.Split('●')[1];
                    }

                    // objectに情報を格納
                    itemGameObj.transform.Find("jobEventScript").GetComponent<Text>().text = script;
                    itemGameObj.transform.Find("jobEventChoice").GetComponent<Text>().text = jobDiaryModelArray[i].choosingString.Replace("/", "");

                }
            }
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
