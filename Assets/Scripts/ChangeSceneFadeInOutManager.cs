using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSceneFadeInOutManager : MonoBehaviour
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
           
            if (GameObject.Find("ChangeSceneFadeInOutManagerSW") == null)
            {
                GameObject ChangeSceneFadeInOutManagerSW = new GameObject("ChangeSceneFadeInOutManagerSW");
                ChangeSceneFadeInOutManagerSW.SetActive(false);
                ChangeSceneFadeInOutManagerSW.AddComponent<Text>().text = "Y";
                ChangeSceneFadeInOutManagerSW.transform.SetParent(GameObject.Find("FadeInOutRefObject").transform);
            }
            else
            {
                GameObject.Find("FadeInOutRefObject").transform.Find("ChangeSceneFadeInOutManagerSW").GetComponent<Text>().text = "Y";
            }

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
