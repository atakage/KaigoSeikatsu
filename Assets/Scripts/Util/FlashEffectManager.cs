using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashEffectManager : MonoBehaviour
{
    // ★ public: inspectorで変更可能, private: inspectorで変更不可能
    public Image flashImage;
    private float duration = 0.5f; 
    private float smoothness = 0.02f;
    public Color flashImageColor;

    public void StartFlashEffect(Color color)
    {
        this.flashImageColor = color;
        this.flashImage = GameObject.Find("Canvas").transform.Find("flashImage").GetComponent<Image>();
        this.flashImage.gameObject.SetActive(true);
        this.flashImage.color = this.flashImageColor;

        StartCoroutine("LerpColor");
    }

    IEnumerator LerpColor()
    {
        float progress = 0;
        float increment = smoothness / duration;
        Debug.Log("color smoothness: " + this.smoothness);
        Debug.Log("color duration: " + duration);
        Debug.Log("color increment: " + increment);
        while(progress < 1)
        {
            this.flashImage.color = Color.Lerp(this.flashImageColor, Color.clear, progress);
            progress += increment;
            yield return new WaitForSeconds(smoothness);
        }
        this.flashImage.gameObject.SetActive(false);
    }
}
