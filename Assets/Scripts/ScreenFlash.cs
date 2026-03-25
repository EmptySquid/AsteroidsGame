using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{

    public float flashDuration = 0.33f;
    private Image flashImage;
    private Color imageColor;

    void Start()
    {
        flashImage = GetComponent<Image>();
        imageColor = flashImage.color;

    }

    private IEnumerator FlashRoutine()
    {
        float timer = 0f;
        float t = 0f;
        float alphaFrom = 1f; // fully opaque
        float alphaTO = 0f; // fully transparent

        while (t < 1f) // repeats while condition is true
        {
            timer += Time.deltaTime;
            t = Mathf.Clamp01(timer / flashDuration);
            float alpha = Mathf.Lerp(alphaFrom, alphaTO, t);
            Color col = imageColor;
            col.a = alpha;
            flashImage.color = col;
            yield return new WaitForEndOfFrame();
        }


        yield return null;
    }
    public void Flash()
    {
        StartCoroutine(FlashRoutine());
    }

}
