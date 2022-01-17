using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HelpManager : MonoBehaviour
{
    HelpSharingVarManager helpSharingVarManager;
    // Start is called before the first frame update
    void Start()
    {
        helpSharingVarManager = GameObject.Find("HelpSharingVarManager").GetComponent("HelpSharingVarManager") as HelpSharingVarManager;

        helpSharingVarManager.closeButtonGameObj.GetComponent<Button>().onClick.AddListener(ClickCloseButton);
    }

    public void ClickCloseButton()
    {
        SceneManager.UnloadSceneAsync("HelpScene");
    }
}
