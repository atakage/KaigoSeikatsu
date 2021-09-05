using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingSharingObjectManager : MonoBehaviour
{
    public GameObject canvasGameObj;
    private void Awake()
    {
        this.canvasGameObj = GameObject.Find("Canvas");
    }
}
