using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraShake : MonoBehaviour
{

    //public int Iterations;
    //public float ShakeAmount;
    //public float ShakeDelay;
    //
    //
    //private IEnumerator ShakeRoutine()
    //{
    //    Debug.Log("Camera Shake");
    //    Vector3 originalPos = transform.position;
    //    for (int n = 0; n < Iterations; n++)
    //    {
    //        Vector3 pos = Random.insideUnitCircle * ShakeAmount;
    //        transform.position = transform.position + pos;
    //        yield return new WaitForSeconds(ShakeDelay);
    //    }
    //    transform.position = originalPos;
    //    yield return null;
    //}

    


    Vector3 originalPos;
    Vector3 originalScale;


    private void Awake()
    {
        originalPos = transform.position;
        originalScale = transform.lossyScale;
    }

    IEnumerator CameraShakeRoutine(float duration, float intensity)
    {
        //Setup
        float timer = 0;
        Vector2 shakeDir;
        float changeX = 0;
        float changeY = 0;
        float stepDist = .5f;

        while (timer < duration)
        {
            //Pick random direction
            shakeDir.x = Random.Range(-intensity, intensity);
            shakeDir.y = Random.Range(-intensity, intensity);

            //Update Position
            changeX = Mathf.Lerp(transform.position.x, transform.position.x + shakeDir.x, stepDist);
            changeY = Mathf.Lerp(transform.position.y, transform.position.y + shakeDir.y, stepDist);

            //Debug.Log("X: " + transform.position.x + " Y: " + transform.position.y + " Z: " + transform.position.z);
            transform.position = new Vector3(changeX, changeY, transform.position.z);

            //Update Timer
            timer += Time.deltaTime;

            //Wait until end of the frame
            yield return new WaitForEndOfFrame();
        }
        //reset position
        transform.position = originalPos;
    }

    public void Shake(float duration, float intensity)
    {
        StartCoroutine(CameraShakeRoutine(duration, intensity));
    }

}
