using UnityEngine;

public class CameraShake : MonoBehaviour
{

    private IEnumerator FlashRoutine()
    {
        float timer = 0f;
        float t = 0f;

        while (t < 1f) // repeats while condition is true
        {
            timer += Time.deltaTime;
            t = Mathf.Clamp01(timer / flashDuration);
            
            yield return new WaitForEndOfFrame();
        }


        yield return null;
    }




    void Start()
    {
        
    }

    public void Flash()
    {
        StartCoroutine(());
    }

}
