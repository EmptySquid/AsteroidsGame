using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    //public Transform camTransform;
    //public float Duration = 0.5f;
    //public float Magnitude = 0.5f;
    //public float shakeAmount = 0.7f;
    //public float decreaseFactor = 1.0f;

    //Vector3 originalPos;

    //void Start()
    //{
    //    if (camTransform == null)
    //    {
    //        camTransform = Camera.main.transform;
    //    }
    //    originalPos = camTransform.localPosition;
    //}



    //private IEnumerator ShakeRoutine()
    //{
    //    float timer = 0f;
    //    float t = 0f;
    //
    //    while (t < 1f) // repeats while condition is true
    //    {
    //        timer += Time.deltaTime;
    //        t = Mathf.Clamp01(timer / shakeDuration);
    //
    //        yield return new WaitForEndOfFrame();
    //    }
    //
    //    yield return null;
    //
    //}

    private IEnumerator ShakeRoutine()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.3f;
        float duration = 0.5f;
        float magnitude = 0.5f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }


    public void Shake()
    {
        StartCoroutine(ShakeRoutine());
    }

}
