using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

// https://stackoverflow.com/questions/53916533/setactive-can-only-be-called-from-the-main-thread
internal class UnityMainThread : MonoBehaviour
{
    
    internal static UnityMainThread wkr;
    Queue<Action> jobs = new Queue<Action>();

    void Awake()
    {
        wkr = this;
    }

    void Update()
    {
        while (jobs.Count > 0)
            // Dequeue(): データを排除
            // Invoke(): ほかのthreadでは接近できないコントロール作業はInvokeを使ってMainThreadにわたす 
            jobs.Dequeue().Invoke();
    }

    internal void AddJob(Action newJob)
    {
        jobs.Enqueue(newJob);
    }
}
