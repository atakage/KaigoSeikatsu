using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SimpleFadeInOutManager : MonoBehaviour
{
    Image fadeImage;
    bool fadeSW;

    void Awake()
    {
        fadeImage = GameObject.Find("Canvas").transform.Find("fadeImage").GetComponent<Image>();
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
            Destroy(this.gameObject);
            fadeImage.gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("AlertGoing").transform.Find("FadeSwitchText").GetComponent<Text>().text = "call";
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
