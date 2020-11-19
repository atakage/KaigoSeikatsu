using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilManager : MonoBehaviour
{


    public List<string> shuffleList(List<string> list)
    {
        System.Random rnd = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            string value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }
}
