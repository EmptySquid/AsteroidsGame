using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Asteroid : MonoBehaviour
{
    public int spawnValue;

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


    private void Start()
    {
        healthCurrent = healthMax;
        _SM = FindAnyObjectByType<SoundManager>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        SpaceShip ship = collision.gameObject.GetComponent<SpaceShip>();
        if(ship != null)
        {
            ship.TakeDamage(collisionDamage);
        }
        if(collision.gameObject.tag == "Bullet1")
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
    }


    public void Explode()
    {
        if(spawnChunks == true)
        {
            int numChunks = Random.Range(chunksMin, chunksMax + 1);

            for (int i = 0; i < numChunks; i++)
            {
                CreateAsteroidChunk();
            }
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
