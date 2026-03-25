using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class SpaceshipMouseController : MonoBehaviour
{
    [Header("Base Player")]
    public float healthMax = 3f;
    public float healthCurrent;

    public GameObject bulletRef;
    public float bulletSpeed;
    public float bulletLifetime = 3f;
    public float firingRate = 0.33f;
    public float fireTimer = 0f;

    private Rigidbody2D rb2D;
    [Header("MouseController")]
    public float moveSpeed = 10f;

    public bool instantRotation = true;

    public float rotationSpeed;

    public ScreenFlash flash;
    public CameraShake shake;

    private Rigidbody2D rb;
    private Camera cam;

    private Vector2 inputMove;
    private float targetAngleDeg;

    public GameObject firingPoint;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
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

    public void FireBullet()
    {
        GameObject bullet = Instantiate(bulletRef, firingPoint.transform.position, transform.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        Vector2 force = transform.up * bulletSpeed;

        rb.AddForce(force);
        Destroy(bullet, bulletLifetime);
    }

    public void TakeDamage(float damage)
    {
        healthCurrent = healthCurrent - damage;
        flash.Flash();
        shake.Shake();
        Debug.Log("Hit!");
        if (healthCurrent <= 0)
        {
            Explode();
        }
    }

    public void Explode()
    {
        Debug.Log("Dead!");
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.GetComponent<Asteroid>() != null)
        {
            TakeDamage(collision.gameObject.GetComponent<Asteroid>().collisionDamage);
        }
    }
    void Update()
    {
        UpdateFiring();

        inputMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        inputMove = Vector2.ClampMagnitude(inputMove, 1f);

        if(cam != null)
        {
            Vector3 mouseWorld3 = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseWorld = new Vector2(mouseWorld3.x, mouseWorld3.y);

            Vector2 dir = mouseWorld - (Vector2)transform.position;

            if (dir.sqrMagnitude > 0.0001f) 
            {
                if (instantRotation)
                {
                    transform.up = dir.normalized;
                }
                else
                {
                    targetAngleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
                }
            }
        }

    }
    private void FixedUpdate()
    {
        rb.linearVelocity = inputMove * moveSpeed;

        if(instantRotation)
        {
            float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngleDeg, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newAngle);
        }
    }
}
