using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float runSpeed = 5f;
    public float jumpForce = 8f;

    [Header("Glide Settings")]
    public float glideSpeed = 4f;
    public float windLiftForce = 6f;
    public float maxVerticalSpeed = 5f;
    public float gravityScale = 3f;
    public float glideGravityScale = 0.5f;

    [Header("Speed Increase Settings")]
    public float speedIncreaseRate = 0.1f;
    public float maxSpeed = 20f;
    public float maxGlideSpeed = 8f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Distance Display")]
    public Text distanceText;
    public float distanceMultiplier = 0.01f;

    [Header("Audio Clips")]
    public AudioClip runningLoopClip;
    public AudioClip glidingWindClip;

    [Header("Audio Volume Settings")]
    [Range(0f, 1f)] public float runningVolume = 0.6f;
    [Range(0f, 1f)] public float glidingVolume = 0.5f;
    public float fadeSpeed = 2f;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded;
    private bool isHoldingUp;
    private bool isGlideHolding;
    private bool isBoosted;
    private bool recoveringSpeed;
    private bool isKnockedBack;
    private bool wasGroundedLastFrame;
    private bool wasHoldingUpLastFrame;

    private float currentSpeed;
    private float currentGlideSpeed;
    private float originalSpeed;
    private float originalGlideSpeed;
    private float boostedSpeedStart;
    private float boostedGlideSpeedStart;
    private float speedRecoveryTimer;
    private float boostTimer;
    private float knockbackTimer;

    private float sessionDistance;
    public Vector3 lastPosition;

    private int groundContacts = 0;

    private ParticleSystem windTrailVFX;
    private AudioSource sfxSource, runningSource, glideSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = gravityScale;

        SetupAudio();

        currentSpeed = runSpeed;
        currentGlideSpeed = glideSpeed;
        sessionDistance = GameManager.Instance != null ? GameManager.Instance.sessionDistance : 0f;
        lastPosition = transform.position;

        windTrailVFX = transform.Find("WindTrailVFX")?.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        isHoldingUp = Input.GetMouseButton(0);

        if (isGrounded && Input.GetMouseButtonDown(0))
            Jump();

        UpdateAnimationAndVFX();
        UpdateDistanceCounter();
        HandleAudio();
    }

    void FixedUpdate()
    {
        CheckGrounded();

        if (isKnockedBack)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f) isKnockedBack = false;
            return;
        }

        HandleBoostAndRecovery();
        MovePlayer();
    }

    private void SetupAudio()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        runningSource = gameObject.AddComponent<AudioSource>();
        glideSource = gameObject.AddComponent<AudioSource>();

        runningSource.loop = true;
        runningSource.clip = runningLoopClip;
        runningSource.volume = 0f;

        glideSource.loop = true;
        glideSource.clip = glidingWindClip;
        glideSource.volume = 0f;
    }

    private void HandleBoostAndRecovery()
    {
        if (recoveringSpeed)
        {
            speedRecoveryTimer += Time.fixedDeltaTime;
            float t = speedRecoveryTimer / 1.5f;
            currentSpeed = Mathf.Lerp(boostedSpeedStart, originalSpeed, t);
            currentGlideSpeed = Mathf.Lerp(boostedGlideSpeedStart, originalGlideSpeed, t);
            if (t >= 1f) recoveringSpeed = false;
        }

        if (isBoosted)
        {
            boostTimer -= Time.fixedDeltaTime;
            if (boostTimer <= 0f)
            {
                isBoosted = false;
                recoveringSpeed = true;
                speedRecoveryTimer = 0f;
                boostedSpeedStart = currentSpeed;
                boostedGlideSpeedStart = currentGlideSpeed;
            }
        }
    }

    private void MovePlayer()
    {
        Vector2 velocity = rb.velocity;

        if (isGrounded)
        {
            velocity.x = currentSpeed;
            rb.gravityScale = gravityScale;
            isGlideHolding = false;

            if (currentSpeed < maxSpeed) currentSpeed += speedIncreaseRate * Time.fixedDeltaTime;
            if (currentGlideSpeed < maxGlideSpeed) currentGlideSpeed += speedIncreaseRate * Time.fixedDeltaTime;
        }
        else
        {
            velocity.x = currentGlideSpeed;
            rb.gravityScale = isHoldingUp ? glideGravityScale : gravityScale;

            if (isGlideHolding)
            {
                rb.gravityScale = 0f;
                velocity.y = 0f;
            }
            else if (isHoldingUp)
            {
                float speedFactor = Mathf.InverseLerp(runSpeed, maxSpeed, currentSpeed);
                float lift = windLiftForce * (1f + speedFactor);
                float maxY = maxVerticalSpeed * (1f + speedFactor);

                velocity.y += lift * Time.fixedDeltaTime;
                velocity.y = Mathf.Clamp(velocity.y, -Mathf.Infinity, maxY);
            }
        }

        rb.velocity = velocity;
    }

    private void UpdateDistanceCounter()
    {
        sessionDistance += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (GameManager.Instance != null)
            GameManager.Instance.sessionDistance = sessionDistance;

        distanceText.text = ((sessionDistance * distanceMultiplier)).ToString("F2") + " km";
    }

    private void UpdateAnimationAndVFX()
    {
        animator.SetBool("running", isGrounded && !isHoldingUp);
        animator.SetBool("gliding", !isGrounded && isHoldingUp);
        animator.SetBool("descending", !isGrounded && !isHoldingUp);

        // Triggers for transitions
        if (wasGroundedLastFrame && !isGrounded && isHoldingUp)
            animator.SetTrigger("run2glide");
        else if (!wasHoldingUpLastFrame && isHoldingUp && !isGrounded)
            animator.SetTrigger("descent2glide");
        else if (wasHoldingUpLastFrame && !isHoldingUp && !isGrounded)
            animator.SetTrigger("glide2descent");

        wasGroundedLastFrame = isGrounded;
        wasHoldingUpLastFrame = isHoldingUp;

        // VFX
        if (!isGrounded)
        {
            if (windTrailVFX != null && !windTrailVFX.isPlaying)
                windTrailVFX.Play();
        }
        else
        {
            if (windTrailVFX != null && windTrailVFX.isPlaying)
                windTrailVFX.Stop();
        }

    }

    private void Jump()
    {
        Vector2 velocity = rb.velocity;
        velocity.y = jumpForce;
        rb.velocity = velocity;

        isGlideHolding = true;
    }


    private void HandleAudio()
    {
        float fade = fadeSpeed * Time.deltaTime;

        if (isGrounded)
        {
            if (!runningSource.isPlaying) runningSource.Play();
            runningSource.volume = Mathf.MoveTowards(runningSource.volume, runningVolume, fade);
            glideSource.volume = Mathf.MoveTowards(glideSource.volume, 0f, fade);
            if (glideSource.volume <= 0.01f && glideSource.isPlaying) glideSource.Stop();
        }
        else
        {
            if (!glideSource.isPlaying) glideSource.Play();
            glideSource.volume = Mathf.MoveTowards(glideSource.volume, glidingVolume, fade);
            runningSource.volume = Mathf.MoveTowards(runningSource.volume, 0f, fade);
            if (runningSource.volume <= 0.01f && runningSource.isPlaying) runningSource.Stop();
        }
    }

    public void ApplyKnockback(Vector2 direction)
    {
        if (!isKnockedBack)
        {
            isKnockedBack = true;
            knockbackTimer = 1f;
            rb.velocity = Vector2.zero;
            rb.velocity = new Vector2(direction.x < 0 ? -15f : 15f, 0f);

            originalSpeed = currentSpeed;
            originalGlideSpeed = currentGlideSpeed;
            currentSpeed = 0f;
            currentGlideSpeed = 0f;
            recoveringSpeed = true;
            speedRecoveryTimer = 0f;
        }
    }

    public void ActivateWindGustBoost(float multiplier, float duration)
    {
        if (!isBoosted)
        {
            isBoosted = true;
            boostTimer = duration;
            originalSpeed = currentSpeed;
            originalGlideSpeed = currentGlideSpeed;
            currentSpeed *= multiplier;
            currentGlideSpeed *= multiplier;
        }
    }
    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void ForceGrounded()
    {
        isGrounded = true;
        isGlideHolding = false;
        rb.gravityScale = gravityScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            groundContacts++;
            isGrounded = true;
            UpdateAnimationAndVFX();

            foreach (ContactPoint2D contact in collision.contacts)
            {
                Vector2 normal = contact.normal.normalized;

                // Only die if the player hits the platform from the side or bottom
                if (Vector2.Dot(normal, Vector2.up) < 0.4f)
                {
                    Die();
                    break;
                }
            }

        }

        if (collision.collider.CompareTag("TopLimit") || collision.collider.CompareTag("BottomLimit"))
        {
            Die();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            groundContacts = Mathf.Max(groundContacts - 1, 0);
            if (groundContacts == 0) isGrounded = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void Die()
    {
        float kmTravelled = sessionDistance * distanceMultiplier;
        GameManager.Instance?.SaveProgressBeforeSceneChange(kmTravelled);
        SceneManager.LoadScene("GameOver");
        Destroy(gameObject);
    }
}

