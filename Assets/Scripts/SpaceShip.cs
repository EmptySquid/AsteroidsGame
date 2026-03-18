using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;

public class SpaceShip : MonoBehaviour
{
    public float healthMax = 3f;
    public float healthCurrent;

    public float enginePower = 10f;
    public float turnPower = 10f;

    public GameObject bulletRef;
    public float bulletSpeed;
    public float bulletLifetime = 3f;
    public float firingRate = 0.33f;
    public float fireTimer = 0f;
    
    public SoundManager _SM;

    private Rigidbody2D rb2D;

    public GameObject firingPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2D = GetComponent <Rigidbody2D>();
        _SM = FindAnyObjectByType<SoundManager>();
        healthCurrent = healthMax;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        ApplyThrust(vertical);
        ApplyTorque(horizontal);
        UpdateFiring();


    }

    private void UpdateFiring()
    {
        bool isFiring = Input.GetButton("Fire1");
        fireTimer = fireTimer - Time.deltaTime;
        if (isFiring && fireTimer <= 0f)
        {
            FireBullet();
            fireTimer = firingRate;
        }
    }

    private void ApplyThrust(float amount)
    {
        Vector2 thrust = transform.up * enginePower * Time.deltaTime * amount;
        rb2D.AddForce(thrust);
    }
    private void ApplyTorque(float amount)
    {
        float torque = amount * turnPower * Time.deltaTime;
        rb2D.AddTorque(-torque);
    }

    public void FireBullet()
    {
        GameObject bullet = Instantiate(bulletRef, firingPoint.transform.position, transform.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        _SM.PlayRandomSound(_SM.bulletSounds);
        Vector2 force = transform.up * bulletSpeed;
        
        rb.AddForce(force);
        Destroy(bullet, bulletLifetime);
        
    }

    public void TakeDamage(float damage)
    {
        healthCurrent = healthCurrent - damage;
        if(healthCurrent <= 0)
        {
            Explode();

        }
        _SM.PlayRandomSound(_SM.impactSounds);
    }

    public void Explode()
    {
        Debug.Log("Game Over!");
        _SM.PlayRandomSound(_SM.deathSounds);
        Destroy(gameObject);
    }



}
