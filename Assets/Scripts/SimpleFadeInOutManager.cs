﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SimpleFadeInOutManager : MonoBehaviour
{
    Image fadeImage;
    bool fadeSW;
    GameObject canvasGameObj;

    void Awake()
    {
        canvasGameObj = GameObject.Find("Canvas");
        fadeImage = canvasGameObj.transform.Find("fadeImage").GetComponent<Image>();
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        color.a = Time.deltaTime * 1.0f;
        fadeImage.color = color;
    }

    void Update()
    {
        StartCoroutine("FadeIn");
        if (fadeSW)
        {
                  // fade out後のあいだ作られるオブジェクトでこのオブジェクト(fadeOutEndMomentSW)を使ったあと必ずvalueを変更しなければならない
            if (canvasGameObj.transform.Find("fadeOutEndMomentSW") == null)
            {
                GameObject fadeOutEndMomentSW = new GameObject("fadeOutEndMomentSW");
                fadeOutEndMomentSW.SetActive(false);
                fadeOutEndMomentSW.AddComponent<Text>().text = "Y";
                fadeOutEndMomentSW.transform.SetParent(canvasGameObj.transform);
            }
            else
            {
                canvasGameObj.transform.Find("fadeOutEndMomentSW").GetComponent<Text>().text = "Y";
            }
            

            if (canvasGameObj.transform.Find("AlertGoing") == null)
            {
                GameObject AlertGoing = new GameObject("AlertGoing");
                AlertGoing.SetActive(false);
                AlertGoing.transform.SetParent(canvasGameObj.transform);

                GameObject FadeSwitchText = new GameObject("FadeSwitchText");
                FadeSwitchText.SetActive(false);
                FadeSwitchText.transform.SetParent(AlertGoing.transform);
                FadeSwitchText.AddComponent<Text>().text = "call";
            }

            fadeImage.gameObject.SetActive(false);
            canvasGameObj.transform.Find("AlertGoing").transform.Find("FadeSwitchText").GetComponent<Text>().text = "call";

            Destroy(this.gameObject);
        }
    }

    IEnumerator FadeIn()
    {
        Color color = fadeImage.color;

        for (int i = 0; i < 100; i++)
        {
            color.a += Time.deltaTime * 0.007f;
            fadeImage.color = color;

            if (fadeImage.color.a > 1)
            {
                fadeSW = true;
            }
        }
        //GameObject.Find("fadeImage").SetActive(false);
        yield return null;
    }
}
