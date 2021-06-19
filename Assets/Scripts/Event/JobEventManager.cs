using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobEventManager : MonoBehaviour
{
    public JobEventSetManager jobEventSetManager;

    public JobEventModel GetActiveJobEventRandom(JobEventModel[] jobEventModelArray)
    {
        List<JobEventModel> activeJobEventList = new List<JobEventModel>();

        // すべてのjobEventからActiveされているイベントリストを作る
        foreach(JobEventModel jobEvent in jobEventModelArray)
        {
            if (jobEvent.eventActive == true) activeJobEventList.Add(jobEvent);
        }

        System.Random random = new System.Random();
        int randomIndex = random.Next(0, activeJobEventList.Count);
        Debug.Log("Random EventCode: " + activeJobEventList.ToArray()[randomIndex].eventCode);

        return activeJobEventList.ToArray()[randomIndex];
    }
}
