using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestA : MonoBehaviour
{
    public SceneTransitionManager sceneTransitionManager;
    public PlayerSaveDataManager playerSaveDataManager;
    public BuildManager buildManager;

    public Text panelText;
    public string[] taleArray;
    public int index;
    // 現在進行中のスクリプト
    public string presentTale;


    Button choiceAButton;
    Button choiceBButton;

    public void Start()
    {

        Debug.Log("TestA Start");

        StreetVariableManager.clickSwitch = true;
        

        TextAsset textAsset = Resources.Load("contact/groupC/testA", typeof(TextAsset)) as TextAsset;
        string[] dataArray = textAsset.text.Split('/');
        DialogTextManager.instance.SetScenarios(new string[] { dataArray[0] });


        //人物と最初に接触ここで初期設定、好感度や記憶の破片なで


        // playerとのprogress(好感度)を読み出す
        playerSaveDataManager = new PlayerSaveDataManager();
        buildManager = GameObject.Find("BuildManager").GetComponent("BuildManager") as BuildManager;

        PlayerData playerData = new PlayerData();
        ItemListData[] itemListData = new ItemListData[2];

        playerData.progress = 1;

        // playerとのprogress(好感度)に合うスクリプトを再生
        if (playerData.progress == 0)
        {
            playerData.progress = 1;
            playerSaveDataManager.SavePlayerData(playerData);
            Debug.Log("TestAとの関係:" + playerData.progress);
            Tale1();
        }
        else if (playerData.progress == 1)
        {

            itemListData[0] = new ItemListData();
            itemListData[0].itemName = "item3";
            itemListData[0].quantity = 222;

            itemListData[1] = new ItemListData();
            itemListData[1].itemName = "item5";
            itemListData[1].quantity = 666;

            playerSaveDataManager.SaveItemListData(itemListData);
            //playerSaveDataManager.SavePlayerData(playerData);
            Tale2();
        }else if (playerData.progress == 20)
        {

        }else if (playerData.progress == 40)
        {

        }else if (playerData.progress == 60)
        {

        }
        // 80%
        else
        {

        }
        

        
    }





    
    public void Tale1()
    {
        Debug.Log("Tale1 START");

        TextAsset textAsset = Resources.Load("contact/groupC/TestA/testA_tale1", typeof(TextAsset)) as TextAsset;
        taleArray = textAsset.text.Split('/');
        //話を始める最後に選択肢
        presentTale = "Tale1";
    }

    public void Tale2()
    {
        Debug.Log("Tale2 START");

        TextAsset textAsset = Resources.Load("contact/groupC/TestA/testA_tale2", typeof(TextAsset)) as TextAsset;
        taleArray = textAsset.text.Split('/');
        presentTale = "Tale2";

        // ボタンにonClick設定
        choiceAButton = GameObject.Find("Canvas").transform.Find("ChoiceAButton").GetComponent<Button>();
        choiceAButton.onClick.AddListener(ClickChoiceAButton);

    }


    // ボタンクリック
    public void ClickChoiceAButton()
    {
        Debug.Log("ClickChoiceAButton");

        if (presentTale.Equals("Tale2"))
        {
            // テキスト読み込む
            TextAsset textAsset =  Resources.Load("contact/groupC/TestA/taleA_tale2_choiceA", typeof(TextAsset)) as TextAsset;
            taleArray = textAsset.text.Split('/');

            // ボタンを隠す
            GameObject.Find("Canvas").transform.Find("ChoiceAButton").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("ChoiceBButton").gameObject.SetActive(false);

            DialogTextManager.instance.SetScenarios(new string[] { "A!!" });
            // テキストを読むためにindexを初期化&クリックスイッチtrue
            index = 0;
            StreetVariableManager.clickSwitch = true;
        }
        else if(presentTale.Equals("Tale1")) { 

            // テキスト読み込む
            TextAsset textAsset = Resources.Load("contact/groupC/TestA/testA_tale1_end", typeof(TextAsset)) as TextAsset;
            taleArray = textAsset.text.Split('/');

            // ボタンを隠す
            GameObject.Find("Canvas").transform.Find("ChoiceAButton").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("ChoiceBButton").gameObject.SetActive(false);

            DialogTextManager.instance.SetScenarios(new string[] { "A!!" });

            // テキストを読むためにindexを初期化&クリックスイッチtrue
            index = 0;
            StreetVariableManager.clickSwitch = true;
        }
    }

    public void ClickChoiceBButton()
    {
        Debug.Log("ClickChoiceBButton");
    }


    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && StreetVariableManager.clickSwitch)
        {
            Debug.Log("INDEX:" + index);
            Debug.Log("taleArrayINDEX:" + taleArray.Length);

            DialogTextManager.instance.SetScenarios(new string[] { taleArray[index] });

            panelText = GameObject.Find("panelText").gameObject.GetComponent<Text>();

            // indexの増加位置を考えて最後のテキストのすぐ前で切る
            if (panelText.text.Equals("taleend"))
            {
                Debug.Log("END!!");
                //選択肢ボタンを作る、クリックスイッチをfalseにする
                StreetVariableManager.clickSwitch = false;

                GameObject.Find("Canvas").transform.Find("ChoiceAButton").gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.Find("ChoiceBButton").gameObject.SetActive(true);

                
                choiceAButton = GameObject.Find("Canvas").transform.Find("ChoiceAButton").GetComponent<Button>();
                choiceBButton = GameObject.Find("Canvas").transform.Find("ChoiceBButton").GetComponent<Button>();

                // ボタンにonClick設定
                choiceAButton.onClick.AddListener(ClickChoiceAButton);
                choiceBButton.onClick.AddListener(ClickChoiceBButton);

            }
            // indexの増加位置を考えて最後のテキストのすぐ前で切る
            else if (panelText.text.Equals("tale2 content") && presentTale.Equals("Tale2"))
            {
                Debug.Log("Tale2 END!!");
                //選択肢ボタンを作る、クリックスイッチをfalseにする
                StreetVariableManager.clickSwitch = false;

                GameObject.Find("Canvas").transform.Find("ChoiceAButton").gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.Find("ChoiceBButton").gameObject.SetActive(true);


            }


            // 選択が終わるとシーン転換
            if (panelText.text.Equals("bye") && presentTale.Equals("Tale1"))
            {
                sceneTransitionManager = new SceneTransitionManager();
                StreetVariableManager.clickSwitch = false;
                Debug.Log("sCene");
                sceneTransitionManager.LoadTo("StreetScene");
            }else if (panelText.text.Equals("bye") && presentTale.Equals("Tale2"))
            {
                sceneTransitionManager = new SceneTransitionManager();
                StreetVariableManager.clickSwitch = false;
                sceneTransitionManager.LoadTo("StreetScene");
            }


            if(taleArray.Length-1 > index) ++index;
        }
    }
}
