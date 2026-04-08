using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] asteroidsRef;
    public float checkInterval = 3f;
    public int spawnAmount;
    public float pushForce = 100f;
    public int spawnThreshold = 10;
    public float inaccuracy = 2f;

    private float checkTimer = 0f;

    public int numberOfStartAsteroids;
    
    public int TotalAsteroidValue()
    {
        Asteroid[] asteroids = FindObjectsByType<Asteroid>(FindObjectsSortMode.None);
        int value = 0;
        for (int n = 0; n < asteroids.Length; n++)
        {
            value += asteroids[n].spawnValue;
        }
        return value;
    }

    //private void Start()
    //{
    //    for (int i = 0)
    //}

    private void Update()
    {
        checkTimer += Time.deltaTime;
        if(checkTimer > checkInterval)
        {
            checkTimer = 0f;

            if(TotalAsteroidValue() < spawnThreshold)
            {
                for (int i = 0; i < spawnAmount; i++)
                {
                    SpawnNewAsteroid();
                }
            }
            else
            {
                Debug.Log("Too many asteroids!");
            }
        }
    }

    public void SpawnNewAsteroid()
    {
        int asteroidIndex = Random.Range(0, asteroidsRef.Length);
        GameObject asteroidRef = asteroidsRef[asteroidIndex];

        Vector3 spawnPoint = OffscreenSpawnPoint();

        GameObject asteroid = Instantiate(asteroidRef, spawnPoint, transform.rotation);

        Vector2 force = PushDirection(spawnPoint) * pushForce;
        Rigidbody2D rb = asteroid.GetComponent<Rigidbody2D>();
        StartCoroutine(WaitForApplyForce(rb, force));
    }
    IEnumerator WaitForApplyForce(Rigidbody2D rb, Vector2 force)
    {
        yield return new WaitForSeconds(1);
        rb.AddForce(force);
    }

    public Vector2 PushDirection(Vector2 from)
    {
        Vector2 miss = Random.insideUnitCircle * inaccuracy;
        Vector2 destination = (Vector2)transform.position + miss;
        Vector2 direction = (destination - from).normalized;

        return direction;
    }

    public Vector3 OffscreenSpawnPoint()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return transform.position;

        // Choose one of the four screen edges
        int edge = Random.Range(0, 4);
        Vector2 viewportPos;

        switch (edge)
        {
            case 0: // Left
                viewportPos = new Vector2(-0.1f, Random.value);
                break;
            case 1: // Right
                viewportPos = new Vector2(1.1f, Random.value);
                break;
            case 2: // Bottom
                viewportPos = new Vector2(Random.value, -0.1f);
                break;
            default: // Top
                viewportPos = new Vector2(Random.value, 1.1f);
                break;
        }

        Vector3 worldPos = cam.ViewportToWorldPoint(viewportPos);
        worldPos.z = transform.position.z;

        return worldPos;
    }

}
