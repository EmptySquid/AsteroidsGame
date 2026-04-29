using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Asteroid : MonoBehaviour
{
    public int spawnValue;
    public int scoreValue;

    public float collisionDamage = 1f;
    public float healthMax = 3f;
    public float healthCurrent;
    public GameObject explodeParticle;

    public GameObject[] chunks;
    public int chunksMin = 0;
    public int chunksMax = 4;
    public float explodeDist = 0.05f;
    public float explosionForce = 10f;

    public bool spawnChunks;

    public SoundManager _SM;
    public GameObject indicator;
    public Canvas canvas;


    private void Start()
    {
        canvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
        GameObject i = Instantiate(indicator, transform.position, transform.rotation);
        i.transform.SetParent(canvas.transform);
        i.GetComponent<OffScreenIndicator>().target = gameObject.transform;
        healthCurrent = healthMax;
        _SM = FindAnyObjectByType<SoundManager>();
        Destroy(gameObject, 6);
    }


    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Bullet1")
    //    {
    //        Destroy(collision.gameObject);
    //        TakeDamage(1f);
    //    }
        
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bullet1")
        {
            Destroy(collision.gameObject);
            TakeDamage(1f);
        }
    }
    public void TakeDamage(float damage)
    {
        healthCurrent = healthCurrent - damage;
        if (healthCurrent <= 0)
        {
            
            Explode();
        }
        else
        {
            _SM.PlayRandomSound(_SM.impactSounds);
        }
    }



    public void Explode()
    {
        Debug.Log("Destroying");
        SpaceshipMouseController ship = FindFirstObjectByType<SpaceshipMouseController>();
        if(ship != null)
        {
            ship.AddToScore(scoreValue);
        }

        Instantiate(explodeParticle, transform.position, transform.rotation);
        _SM.PlayRandomSound(_SM.explosionSounds);
        Destroy(gameObject);
    }

    private void CreateAsteroidChunk()
    {
        int rndIndex = Random.Range(0, chunks.Length);
        GameObject chunkRef = chunks[rndIndex];

        Vector2 spawnPos = transform.position;
            spawnPos.x += Random.Range(-explodeDist, explodeDist);
            spawnPos.y += Random.Range(-explodeDist, explodeDist);

        GameObject chunk = Instantiate(chunkRef, spawnPos, transform.rotation);

        Vector2 dir = (spawnPos - (Vector2)transform.position).normalized;

        Rigidbody2D rb = chunk.GetComponent<Rigidbody2D>();
        rb.AddForce(dir * explosionForce);


    }

   
}
