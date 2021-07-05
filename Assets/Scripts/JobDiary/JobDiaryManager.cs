using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class JobDiaryManager : MonoBehaviour
{
    public SceneTransitionManager sceneTransitionManager;
    public PlayerSaveDataManager playerSaveDataManager;
    public GameObject canvasGameObj;
    public GameObject jobDiaryAlphaScreenCanvas;
    public GameObject jobDiaryFlipIconCanvas;
    public GameObject jobDiaryTipScriptCanvas;
    public GameObject jobDiaryTipCloseCanvas;
    public RectTransform flipIconRectTransform;
    public Vector3 flipIconRectPosiion;
    public PlayerData playerData;
    private void Start()
    {
        sceneTransitionManager = new SceneTransitionManager();
        playerSaveDataManager = new PlayerSaveDataManager();

        canvasGameObj = GameObject.Find("Canvas");
        jobDiaryAlphaScreenCanvas = GameObject.Find("jobDiaryAlphaScreenCanvas");
        jobDiaryFlipIconCanvas = GameObject.Find("jobDiaryFlipIconCanvas");
        jobDiaryTipScriptCanvas = GameObject.Find("jobDiaryTipScriptCanvas");
        jobDiaryTipCloseCanvas = GameObject.Find("jobDiaryTipCloseCanvas");

        flipIconRectTransform = (RectTransform)jobDiaryFlipIconCanvas.transform.Find("flipIcon");
        flipIconRectPosiion = flipIconRectTransform.anchoredPosition;


        // 戻るボタンの目的地を設定
        if (GameObject.Find("SceneChangeManager") != null)
        {
            string goBackScene = GameObject.Find("SceneChangeManager").transform.Find("SceneChangeCanvas").transform.Find("destinationFrom-toItemCheckScene").GetComponent<Text>().text;
            canvasGameObj.transform.Find("returnButton").GetComponent<Button>().onClick.AddListener(delegate { ClickReturnButton(goBackScene); });
        }

        // 最初のjobDiaryTip
        playerData = playerSaveDataManager.LoadPlayerData();
        if(playerData.tip.checkedJobDiaryTip == false) DisplayJobDiaryTip();
    }

    public void DisplayJobDiaryTip()
    {
        jobDiaryAlphaScreenCanvas.transform.Find("alpha30ScreenImage").gameObject.SetActive(true);
        jobDiaryAlphaScreenCanvas.transform.Find("alphaBlockScreenImage").gameObject.SetActive(true);
        jobDiaryFlipIconCanvas.transform.Find("flipIcon").gameObject.SetActive(true);
        jobDiaryTipScriptCanvas.transform.Find("Text").gameObject.SetActive(true);
        jobDiaryTipCloseCanvas.transform.Find("Button").gameObject.SetActive(true);

        jobDiaryTipCloseCanvas.transform.Find("Button").GetComponent<Button>().onClick.AddListener(EndTip);

        StartCoroutine("StartTipDisplay");

        playerData = playerSaveDataManager.LoadPlayerData();
        playerData.tip.checkedJobDiaryTip = true;
        playerSaveDataManager.SavePlayerData(playerData);
    }

    IEnumerator StartTipDisplay()
    {
        bool startFormRight = true;
        bool startFormLeft = false;
        int tipCount = 0;
        Vector3 movingPosition = flipIconRectPosiion;
        Vector3 destinationRightToLeftVector3 = flipIconRectPosiion * -1;
        Vector3 destinationLeftToRightVector3 = destinationRightToLeftVector3 * -1;

        while (true)
        {
            
            if (startFormRight)
            {
                if (flipIconRectTransform.anchoredPosition.x > destinationRightToLeftVector3.x)
                {
                    movingPosition.x -= 15f;
                    flipIconRectTransform.anchoredPosition = movingPosition;
                }
                else
                {
                    flipIconRectTransform.anchoredPosition = destinationRightToLeftVector3;
                    startFormRight = false;
                    startFormLeft = true;
                    tipCount += 1;
                    // Coroutine(thread)自体を休憩
                    yield return new WaitForSeconds(0.4f);
                }
            }
            else if (startFormLeft)
            {

                if (flipIconRectTransform.anchoredPosition.x < destinationLeftToRightVector3.x)
                {
                    movingPosition.x += 15f;
                    flipIconRectTransform.anchoredPosition = movingPosition;
                }
                else
                {
                    flipIconRectTransform.anchoredPosition = destinationLeftToRightVector3;
                    startFormLeft = false;
                    startFormRight = true;
                    tipCount += 1;
                    yield return new WaitForSeconds(0.4f);
                }
                
            }
            if (tipCount > 5) EndTip();
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void EndTip()
    {
        jobDiaryAlphaScreenCanvas.transform.Find("alpha30ScreenImage").gameObject.SetActive(false);
        jobDiaryAlphaScreenCanvas.transform.Find("alphaBlockScreenImage").gameObject.SetActive(false);
        jobDiaryFlipIconCanvas.transform.Find("flipIcon").gameObject.SetActive(false);
        jobDiaryTipScriptCanvas.transform.Find("Text").gameObject.SetActive(false);
        jobDiaryTipCloseCanvas.transform.Find("Button").gameObject.SetActive(false);

        StopCoroutine("StartTipDisplay");
    }

    public void ClickReturnButton(string goBackScene)
    {
        // 破壊しない場合増加する
        Destroy(GameObject.Find("SceneChangeManager"));
        sceneTransitionManager.LoadTo(goBackScene);
    }

    public List<JobDiaryModel> AddEventToJobDiary(JobDiaryModel[] jobDiaryModelArray, string eventCode, string eventScript, string choosingText)
    {
        List<JobDiaryModel> jobDiaryModelList = new List<JobDiaryModel>();

        JobDiaryModel jobDiaryModel = new JobDiaryModel();
        jobDiaryModel.eventCode = eventCode;
        jobDiaryModel.eventScript = eventScript;
        jobDiaryModel.choosingString = choosingText;

        // jobDiaryがnullなら既存イベントをリストに追加せずに続ける
        if (jobDiaryModelArray == null || jobDiaryModelArray.Length < 1)
        {
            jobDiaryModelList.Add(jobDiaryModel);
        }
        // jobDiaryにイベントがあるなら既存リストに追加
        else
        {
            jobDiaryModelList = jobDiaryModelArray.OfType<JobDiaryModel>().ToList();
            jobDiaryModelList.Add(jobDiaryModel);
        }
        return jobDiaryModelList;
    }
}
