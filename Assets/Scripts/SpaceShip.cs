using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    public int score;
    public int multiplier;

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

    public ScreenFlash flash;

    private Rigidbody2D rb2D;

    public GameObject firingPoint;

    public CameraShake shake;

    public ScoreUI scoreUI;

    public float duration;
    public float intensity;

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
        flash.Flash();
        shake.Shake(duration, intensity);
        if (healthCurrent <= 0)
        {
            flash.Flash();
            Explode();

        }
        _SM.PlayRandomSound(_SM.impactSounds);
    }



    public void Explode()
    {
        Debug.Log("Game Over!");
        GameOver();
        _SM.PlayRandomSound(_SM.deathSounds);
        Destroy(gameObject);
    }

    public void GameOver()
    {
        bool celebrateHiscore = false;
        if(score > GetHighScore())
        {
            SetHighScore(score);
            celebrateHiscore = true;
        }
        scoreUI.Show(celebrateHiscore);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<Asteroid>() != null)
        {
            TakeDamage(collision.gameObject.GetComponent<Asteroid>().collisionDamage);
        }
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("Hiscore", 0);
    }
    public void SetHighScore(int score)
    {
        PlayerPrefs.SetInt("Hiscore", score);
    }

}
