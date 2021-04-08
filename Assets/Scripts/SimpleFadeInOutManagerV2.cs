using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleFadeInOutManagerV2 : MonoBehaviour
{
    Image fadeImage;
    bool fadeSW;
    bool fadeStartSW;
    string valueName;

    public void PassParameter(bool fadeStartSW, string valueName)
    {
        // parameterを入れるとfade作業が始まる
        this.valueName = valueName;
        this.fadeStartSW = fadeStartSW;
    }

    void Awake()
    {
        fadeImage = GameObject.Find("Canvas").transform.Find("fadeImage").GetComponent<Image>();
        Color color = fadeImage.color;
        color.a = Time.deltaTime * 1.0f;
        fadeImage.color = color;
    }

    void Update()
    {
        if (fadeStartSW)
        {
            fadeImage.gameObject.SetActive(true);
            StartCoroutine("FadeIn");
            if (fadeSW)
            {
                fadeImage.gameObject.SetActive(false);
                GameObject.Find("Canvas").transform.Find("FadeCompleteValue").GetComponent<Text>().text = valueName;
                Destroy(this.gameObject);
            }
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
