﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FadeInOutManager : MonoBehaviour
{
    Image fadeImage;
    bool fadeSW;

    void Awake()
    {
        fadeImage = GameObject.Find("Canvas").transform.Find("fadeImage").GetComponent<Image>();
        Color color = fadeImage.color;
        color.a = Time.deltaTime * 1.0f;
        fadeImage.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine("FadeIn");
        if (fadeSW)
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator FadeIn()
    {
        Color color = fadeImage.color;

        for (int i=0; i<100; i++)
        {
            Debug.Log("fadeImage.color.a: " + fadeImage.color.a);
            color.a += Time.deltaTime * 0.007f;
            fadeImage.color = color;

            if(fadeImage.color.a == 1)
            {
                fadeSW = true;
            }
        }
        //GameObject.Find("fadeImage").SetActive(false);
        yield return null;
    }
}