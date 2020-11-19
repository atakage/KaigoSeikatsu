using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContactingManager : MonoBehaviour
{

    public StreetVariableManager streetVariableManager;
    public StreetContactManager streetContactManager;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ContactingScene START");
        Debug.Log(StreetVariableManager.actPoint);

        streetVariableManager = new StreetVariableManager();
        streetVariableManager.AssignInitialVariable();


        streetContactManager = new StreetContactManager();

            // 接触するものをランダムで決める
        string getOne = streetContactManager.ShuffleGroupCList();

        // 決めたものを作る
        streetContactManager.CreateGroupCList(getOne);

    }
}
