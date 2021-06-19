using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FadeInOutManager : MonoBehaviour
{
    public FacilityManager facilityManager;
    Image fadeImage;
    bool fadeSW;

    private void Start()
    {
        facilityManager = new FacilityManager();
    }

    void Awake()
    {
        fadeImage = GameObject.Find("Canvas").transform.Find("fadeImage").GetComponent<Image>();
        fadeImage.gameObject.SetActive(true);
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
            // fade out後のあいだ作られるオブジェクトでこのオブジェクト(fadeOutEndMomentSW)を使ったあと必ず削除しなければならない
            GameObject fadeOutEndMomentSW = new GameObject("fadeOutEndMomentSW");
            fadeOutEndMomentSW.SetActive(false);
            fadeOutEndMomentSW.AddComponent<Text>().text = "Y";
            fadeOutEndMomentSW.transform.SetParent(GameObject.Find("Canvas").transform); 

            GameObject.Find("Canvas").transform.Find("fadeOutEventCheck").GetComponent<Text>().text = "Y";
            Destroy(this.gameObject);
            fadeImage.gameObject.SetActive(false);
            facilityManager.FacilityUISetActive(true);
        }
    }

    IEnumerator FadeIn()
    {
        Color color = fadeImage.color;

        for (int i=0; i<100; i++)
        {
            color.a += Time.deltaTime * 0.007f;
            fadeImage.color = color;

            if(fadeImage.color.a > 1)
            {
                fadeSW = true;
            }
        }
        //GameObject.Find("fadeImage").SetActive(false);
        yield return null;
    }
}
