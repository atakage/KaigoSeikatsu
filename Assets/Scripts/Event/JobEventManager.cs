using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobEventManager : MonoBehaviour
{
    public JobEventSetManager jobEventSetManager;

    public List<JobEventModel> SetEventActiveAndReturnAll(JobEventModel[] jobEventModelArray, string eventCode, bool sw)
    {
        List<JobEventModel> newJobEventModelList = new List<JobEventModel>();

        foreach (JobEventModel jobEvent in jobEventModelArray)
        {
            // 特定イベントのactiveをセットする
            if (jobEvent.eventCode.Equals(eventCode))
            {
                jobEvent.eventActive = sw;
            }
            newJobEventModelList.Add(jobEvent);
        }
        return newJobEventModelList;
    }

    public JobEventModel GetActiveJobEventRandom(JobEventModel[] jobEventModelArray)
    {
        List<JobEventModel> activeJobEventList = new List<JobEventModel>();

        // すべてのjobEventからActiveされているイベントリストを作る
        foreach(JobEventModel jobEvent in jobEventModelArray)
        {
            if (jobEvent.eventActive == true) activeJobEventList.Add(jobEvent);
        }

        if(activeJobEventList.Count > 0)
        {
            System.Random random = new System.Random();
            int randomIndex = random.Next(0, activeJobEventList.Count);
            Debug.Log("Random EventCode: " + activeJobEventList.ToArray()[randomIndex].eventCode);
            return activeJobEventList.ToArray()[randomIndex];
        }
        else
        {
            return null;
        }
    }
}
