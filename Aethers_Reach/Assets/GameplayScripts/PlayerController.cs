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
    public float fadeSpeed = 3f;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded;
    private bool isHoldingUp;
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
    private float holdTimer = 0f;
    private float holdThreshold = 0.2f;

    public Vector3 lastPosition;

    private int groundContacts = 0;

    private ParticleSystem windTrailVFX;

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
        // Track button state
        if (Input.GetMouseButton(0))
        {
            holdTimer += Time.deltaTime;

            isHoldingUp = holdTimer >= holdThreshold;
        }
        else
        {
            holdTimer = 0f;
            isHoldingUp = false;
        }

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
            HandleBoostAndRecovery();
            if (knockbackTimer <= 0f) isKnockedBack = false;
            return;
        }

        HandleBoostAndRecovery();
        MovePlayer();
    }

    private void SetupAudio()
    {

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

            if (currentSpeed < maxSpeed) currentSpeed += speedIncreaseRate * Time.fixedDeltaTime;
            if (currentGlideSpeed < maxGlideSpeed) currentGlideSpeed += speedIncreaseRate * Time.fixedDeltaTime;
        }
        else
        {
            velocity.x = currentGlideSpeed;
            rb.gravityScale = isHoldingUp ? glideGravityScale : gravityScale;

            if (isHoldingUp)
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
        if ( isGrounded && isHoldingUp)
            animator.SetTrigger("run2glide");
        else if ( isHoldingUp && !isGrounded)
            animator.SetTrigger("descent2glide");
        else if (!isHoldingUp && !isGrounded)
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
    }

    private void HandleAudio()
    {
        if (AudioManager.Instance == null) return;

        AudioClip targetClip = isGrounded ? runningLoopClip : (!isGrounded ? glidingWindClip : null);
        float targetVolume = isGrounded ? runningVolume : (!isGrounded ? glidingVolume : 0f);

        if (targetClip == null)
        {
            // Fade out if no clip
            AudioManager.Instance.sfxSource.volume = Mathf.MoveTowards(AudioManager.Instance.sfxSource.volume, 0f, fadeSpeed * Time.deltaTime);
            if (AudioManager.Instance.sfxSource.volume <= 0.01f && AudioManager.Instance.sfxSource.isPlaying)
                AudioManager.Instance.sfxSource.Stop();
            return;
        }

        // Switch clip if different
        if (AudioManager.Instance.sfxSource.clip != targetClip)
        {
            AudioManager.Instance.sfxSource.clip = targetClip;
            AudioManager.Instance.sfxSource.loop = true;
            AudioManager.Instance.sfxSource.Play();
        }

        // Smoothly fade to target volume
        AudioManager.Instance.sfxSource.volume = Mathf.MoveTowards(AudioManager.Instance.sfxSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
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

            boostedSpeedStart = currentSpeed;   
            boostedGlideSpeedStart = currentGlideSpeed;

            currentSpeed *= 0.3f; 
            currentGlideSpeed *= 0.3f;

            recoveringSpeed = true;
            speedRecoveryTimer = 0f;

            Handheld.Vibrate();
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

                Debug.Log(Vector2.Dot(normal, Vector2.up));

                if (Vector2.Dot(normal, Vector2.up) < 0.4f)
                {

                    if (!isKnockedBack)
                    {
                        Vector2 direction = transform.position.normalized;
                        ApplyKnockback(direction);
                    }
                    break;
                }
            }

        }

        if (collision.collider.CompareTag("WindCollider"))
        {
            ActivateWindGustBoost(1f, 1f);
        }

        else if (collision.collider.CompareTag("BottomLimit"))
        {
            AudioManager.Instance.musicSource.Stop();
            AudioManager.Instance.sfxSource.Stop();
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

