
using UnityEngine;
using System.Collections;

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

    [Header("Movement")]
    public float moveSpeed = 10f;
    public bool instantRotation = true;
    public float rotationSpeed;

    [Header("Dodge Settings")]
    public float dodgeSpeed = 20f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 0.8f;

    [Header("Mouse Reticle")]
    public Transform mouseReticle;
    public float reticleMaxDistance = 6f;

    private Rigidbody2D rb;
    private Camera cam;

    private Vector2 inputMove;
    private float targetAngleDeg;

    public GameObject firingPoint;

    private bool isDodging;
    private bool canDodge = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        healthCurrent = healthMax;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        UpdateFiring();

        // Dodge input
        if (Input.GetKeyDown(KeyCode.Space) && canDodge)
            StartCoroutine(Dodge());

        if (isDodging)
            return;

        inputMove = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
        inputMove = Vector2.ClampMagnitude(inputMove, 1f);

        UpdateMouseReticle();
        RotateShipTowardReticle();
    }

    private void FixedUpdate()
    {
        if (isDodging)
            return;

        rb.linearVelocity = inputMove * moveSpeed;

        if (!instantRotation)
        {
            float newAngle = Mathf.MoveTowardsAngle(
                rb.rotation,
                targetAngleDeg,
                rotationSpeed * Time.fixedDeltaTime
            );
            rb.MoveRotation(newAngle);
        }
    }

    // =========================
    // MOUSE RETICLE
    // =========================
    private void UpdateMouseReticle()
    {
        if (cam == null || mouseReticle == null)
            return;

        Vector3 mouseScreen = Input.mousePosition;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Vector2 offset = mouseWorld - transform.position;
        offset = Vector2.ClampMagnitude(offset, reticleMaxDistance);

        mouseReticle.position = (Vector2)transform.position + offset;
    }

    private void RotateShipTowardReticle()
    {
        if (mouseReticle == null)
            return;

        Vector2 dir = mouseReticle.position - transform.position;

        if (dir.sqrMagnitude > 0.0001f)
        {
            if (instantRotation)
            {
                transform.up = dir.normalized;
            }
            else
            {
                targetAngleDeg =
                    Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            }
        }
    }

    // =========================
    // DODGE
    // =========================
    private IEnumerator Dodge()
    {
        canDodge = false;
        isDodging = true;

        rb.linearVelocity = transform.up * dodgeSpeed;

        yield return new WaitForSeconds(dodgeDuration);

        rb.linearVelocity = Vector2.zero;
        isDodging = false;

        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }

    // =========================
    // FIRING
    // =========================
    private void UpdateFiring()
    {
        bool isFiring = Input.GetButton("Fire1");
        fireTimer -= Time.deltaTime;

        if (isFiring && fireTimer <= 0f)
        {
            FireBullet();
            fireTimer = firingRate;
        }
    }

    public void FireBullet()
    {
        GameObject bullet = Instantiate(
            bulletRef,
            firingPoint.transform.position,
            transform.rotation
        );

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        rbBullet.AddForce(transform.up * bulletSpeed);
        Destroy(bullet, bulletLifetime);
    }

    // =========================
    // DAMAGE
    // =========================
    public void TakeDamage(float damage)
    {
        if (isDodging)
            return;

        healthCurrent -= damage;
        Debug.Log("Hit!");

        if (healthCurrent <= 0)
            Explode();
    }

    public void Explode()
    {
        Debug.Log("Dead!");
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
        if (asteroid != null)
            TakeDamage(asteroid.collisionDamage);
    }
}
