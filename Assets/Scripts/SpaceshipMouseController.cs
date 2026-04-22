using UnityEngine;
using System.Collections;

public class SpaceshipMouseController : MonoBehaviour
{
    [Header("Base Player")]
    public float healthMax = 3f;
    public float healthCurrent;

    [Header("Movement")]
    public float moveSpeed = 10f;
    public bool instantRotation = true;
    public float rotationSpeed = 720f;

    [Header("Dodge Settings")]
    public float dodgeSpeed = 20f;
    public float dodgeDuration = 0.2f;
    public float dodgeWindowDuration = 0.2f;
    public float dodgeCooldown = 0.8f;
    public float dodgeSloMoSpeed = 0.5f;

    [Header("Attack Settings")]
    public float attackSpeed = 24f;
    public float attackDuration = 0.2f;
    public float attackCooldown = 0.8f;

    [Header("Mouse Reticle")]
    public Transform mouseReticle;
    public float reticleMaxDistance = 6f;

    // Internal
    Rigidbody2D rb;
    Camera cam;
    Vector2 inputMove;
    float targetAngleDeg;

    bool isDodging;
    bool isAttacking;
    bool canDodge = true;
    bool canAttack;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        healthCurrent = healthMax;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        // =========================
        // INPUT
        // =========================
        UpdateMouseReticle();
        RotateShipTowardReticle();
        if (Input.GetMouseButtonDown(1) && canDodge && !isAttacking)
        {
            StartCoroutine(Dodge());
        }

        if (isDodging && canAttack && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Attack());
        }

        if (isDodging || isAttacking)
            return;

        inputMove = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        inputMove = Vector2.ClampMagnitude(inputMove, 1f);


    }

    void FixedUpdate()
    {
        if (isDodging || isAttacking)
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
    public void TakeDamage(float damage)
    {

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

    // =========================
    // DODGE (WASD-BASED)
    // =========================

    IEnumerator Dodge()
    {
        canDodge = false;
        isDodging = true;
        canAttack = true;

        // Enter slow motion
        SetTimeScale(dodgeSloMoSpeed);

        // Determine dodge direction
        Vector2 dodgeDir = inputMove;
        if (dodgeDir == Vector2.zero)
            dodgeDir = transform.up; // fallback

        rb.linearVelocity = dodgeDir.normalized * dodgeSpeed;

        yield return new WaitForSeconds(dodgeDuration);

        rb.linearVelocity = Vector2.zero;

        // Allow a short window to chain attack
        StartCoroutine(DodgeWindowTimeout());
    }

    IEnumerator DodgeWindowTimeout()
    {
        yield return new WaitForSecondsRealtime(dodgeWindowDuration);

        if (isDodging && !isAttacking)
        {
            ExitDodgeMode();
        }
    }

    void ExitDodgeMode()
    {
        isDodging = false;
        canAttack = false;

        SetTimeScale(1f);

        StartCoroutine(DodgeCooldown());
    }

    IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }

    // =========================
    // ATTACK (MOUSE DASH)
    // =========================

    IEnumerator Attack()
    {
        isAttacking = true;
        canAttack = false;

        ExitDodgeMode();

        Vector2 dir = (mouseReticle.position - transform.position).normalized;
        rb.linearVelocity = dir * attackSpeed;

        yield return new WaitForSeconds(attackDuration);

        rb.linearVelocity = Vector2.zero;
        isAttacking = false;

        yield return new WaitForSeconds(attackCooldown);
    }

    // =========================
    // MOUSE / ROTATION
    // =========================

    void UpdateMouseReticle()
    {
        if (!cam || !mouseReticle)
            return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 offset = mouseWorld - transform.position;
        offset = Vector2.ClampMagnitude(offset, reticleMaxDistance);

        mouseReticle.position = (Vector2)transform.position + offset;
    }

    void RotateShipTowardReticle()
    {
        if (!mouseReticle)
            return;

        Vector2 dir = mouseReticle.position - transform.position;
        if (dir.sqrMagnitude < 0.0001f)
            return;

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

    // =========================
    // TIME SCALE HELPER
    // =========================

    void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
        Time.fixedDeltaTime = 0.02f * scale;
    }
}