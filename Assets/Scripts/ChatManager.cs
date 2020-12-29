﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public List<string[]> textList;
    public Text panelText;
    public bool dialogueSW;
    public int clickCount;
    public int textCount;   // テキスト配列リストの配列の数
    // Start is called before the first frame update
    void Start()
    {
        panelText = GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>();
        clickCount = 0;

        string[] aa = {"T","E","S"," ","T","1" };
        string[] bb = { "T", "E", "S", " ", "T", "2", "2" };
        List<string[]> reqList = new List<string[]>();
        reqList.Add(aa);
        reqList.Add(bb);

        ShowDialogue(reqList);

    }

    private void Update()
    {
        if (dialogueSW)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // マウスをクリックするとclickCountが増加して次の配列テキストを読み込む
                ++clickCount;
                if (clickCount == textCount)
                {
                    StopAllCoroutines();
                    ExitDialogue();
                }
                else
                {
                    StopAllCoroutines();
                    StartCoroutine(StartDialogueCoroutine());
                }
            }
        }
    }

    public void ShowDialogue(List<string[]> textList)
    {
        // リストにある配列の数を読み込む
        textCount = textList.Count;
        this.textList = textList;
        StartCoroutine(StartDialogueCoroutine());
    }

    public void ExitDialogue()
    {
        dialogueSW = false;
        clickCount = 0;
        panelText.text = "";

    }

    IEnumerator StartDialogueCoroutine()
    {
        dialogueSW = true;
        panelText.text = "";

        for (int i=0; i< textList[clickCount].Length; i++)
        {
            panelText.text += textList[clickCount][i];
            Debug.Log(panelText.text);
            yield return new WaitForSeconds(0.05f);
        }
    }
}