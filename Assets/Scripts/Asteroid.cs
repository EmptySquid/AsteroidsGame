using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Asteroid : MonoBehaviour
{

    public float collisionDamage = 1f;
    public float healthMax = 3f;
    public float healthCurrent;
    public GameObject explodeParticle;

    private void Start()
    {
        healthCurrent = healthMax;
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
        Instantiate(explodeParticle, transform.position, transform.rotation);
        Destroy(gameObject);
    }

}
