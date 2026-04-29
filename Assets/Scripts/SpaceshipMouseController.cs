using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting;

public class SpaceshipMouseController : MonoBehaviour
{

    public int score;
    public int multiplier = 1;
    public GameManager _GM;
    public GameObject destroyParticle;
    public SoundManager _SM;
    public SpawnManager _SPAWN;
    public SpriteRenderer _spriteRenderer;

    public Sprite standingSprite;
    public Sprite chargeSprite;
    public Sprite punchSprite;

    public ScoreUI scoreUI;

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

    public bool isDodging;
    public bool isAttacking;
    public bool canDodge = true;
    public bool canAttack;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        _SM = FindAnyObjectByType<SoundManager>();
        healthCurrent = healthMax;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        _spriteRenderer = GetComponent<SpriteRenderer>();
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

        //   if (isDodging)
        //   {
        //       _spriteRenderer.sprite = chargeSprite;
        //   } else if (isAttacking)
        //   {
        //       _spriteRenderer.sprite = punchSprite;
        //   } else
        //   {
        //       _spriteRenderer.sprite = standingSprite;
        //   }
        _spriteRenderer.sprite = standingSprite;
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
        Instantiate(destroyParticle, transform.position, transform.rotation);
        _SM.PlayRandomSound(_SM.deathSounds);
        GameOver();

        _GM.KillPlayer(gameObject);
    }
    public void GameOver()
    {
        bool celebrateHiscore = false;
        if (score > GetHighScore())
        {
            SetHighScore(score);
            celebrateHiscore = true;
        }
        scoreUI.Show(celebrateHiscore);
    }
    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("Hiscore", 0);
    }
    public void SetHighScore(int score)
    {
        PlayerPrefs.SetInt("Hiscore", score);
    }


    public void AddToScore(int amount)
    {
        int prevMultiplier = multiplier;

        int totalScore = amount * multiplier;
        score += totalScore;

        multiplier++;

        int prevTens = prevMultiplier / 10;
        int currTens = multiplier / 10;

        _SPAWN.spawnAmount += currTens - prevTens;
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
    //    if (asteroid != null)
    //    {
    //        if (isAttacking)
    //        {
    //            asteroid.TakeDamage(6);
    //        }
    //        else
    //        {
    //            TakeDamage(asteroid.collisionDamage);
    //        }

    //    }

    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
        if (asteroid != null)
        {
            if (isAttacking)
            {
                asteroid.TakeDamage(6);
            }
            else
            {
                TakeDamage(asteroid.collisionDamage);
            }

        }
    }

    // =========================
    // DODGE (WASD-BASED)
    // =========================

    IEnumerator Dodge()
    {
        canDodge = false;
        isDodging = true;
        canAttack = true;

        _spriteRenderer.sprite = chargeSprite;

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

        _spriteRenderer.sprite = punchSprite;

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