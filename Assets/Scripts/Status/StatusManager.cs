using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour
{
    StatusInitVar statusInitVar;
    // Start is called before the first frame update
    void Start()
    {
        statusInitVar = GameObject.Find("StatusInitVar").GetComponent("StatusInitVar") as StatusInitVar;

        statusInitVar.closeButtonGameObj.GetComponent<Button>().onClick.AddListener(() => ClickCloseButton());
    }


    public void ClickCloseButton()
    {
        SceneManager.UnloadSceneAsync("StatusScene");
    }
}
