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
    private float distanceOffset = 0f;

    [Header("Relic Speed Boost Settings")]
    public float boostDuration = 2f;
    private float boostTimer = 0f;
    private bool isBoosted = false;

    [Header("Audio Source")]
    public AudioClip runningLoopClip;
    public AudioClip glidingWindClip;

    private AudioSource sfxSource;
    private AudioSource runningSource;
    private AudioSource glideSource;

    [Header("Audio Volume Settings")]
    [Range(0f, 1f)] public float runningVolume = 0.6f;
    [Range(0f, 1f)] public float glidingVolume = 0.5f;
    public float fadeSpeed = 2f; // audio fades in/out


    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isHoldingUp;
    private bool isGlideHolding;
    private float currentSpeed;
    private float currentGlideSpeed;

    private float originalSpeed;
    private float originalGlideSpeed;
    private float speedRecoveryDuration = 1.5f;
    private float speedRecoveryTimer = 0f;
    private bool recoveringSpeed = false;
    private float boostedSpeedStart;
    private float boostedGlideSpeedStart;
    private float sessionDistance = 0f;
    public Vector3 lastPosition;
    private ParticleSystem windTrailVFX;

    private Animator animator;

    private bool controlsEnabled = false;

    void Start()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        runningSource = gameObject.AddComponent<AudioSource>();
        glideSource = gameObject.AddComponent<AudioSource>();
        runningSource.volume = 0f;
        glideSource.volume = 0f;

        animator = GetComponent<Animator>();

        //looping audio sources
        runningSource.loop = true;
        runningSource.clip = runningLoopClip;

        glideSource.loop = true;
        glideSource.clip = glidingWindClip;

        sessionDistance = GameManager.Instance != null ? GameManager.Instance.sessionDistance : 0f;

        lastPosition = transform.position; 

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;


        currentSpeed = runSpeed;
        currentGlideSpeed = glideSpeed;

        windTrailVFX = transform.Find("WindTrailVFX")?.GetComponent<ParticleSystem>();



    }

    void Update()
    {
        if (!controlsEnabled) return;

        isHoldingUp = Input.GetMouseButton(0);

        if (isGrounded && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetMouseButtonDown(0)))
        {
            Jump();
        }

     
       

        // Play WindTrailVFX only when holding input and in air
        if (isHoldingUp || !isGrounded)
        {
            animator.SetBool("running", false);
            animator.SetBool("gliding", true);

            if (windTrailVFX != null && !windTrailVFX.isPlaying)
                windTrailVFX.Play();

        }
        else
        {
            if (windTrailVFX != null && windTrailVFX.isPlaying)
                windTrailVFX.Stop();
        }

        UpdateDistanceCounter();
        HandleAudio();
    }




    void FixedUpdate()
    {
        if (!controlsEnabled) return;
        Vector2 velocity = rb.velocity;

        // Handle knockback
        if (isKnockedBack)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
                isKnockedBack = false;

            return;
        }

        // Handle speed recovery after knockback or boost
        if (recoveringSpeed)
        {
            speedRecoveryTimer += Time.fixedDeltaTime;
            float t = speedRecoveryTimer / speedRecoveryDuration;

            currentSpeed = Mathf.Lerp(boostedSpeedStart, originalSpeed, t);
            currentGlideSpeed = Mathf.Lerp(boostedGlideSpeedStart, originalGlideSpeed, t);

            if (t >= 1f)
                recoveringSpeed = false;
        }

        // Handle boost duration
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

        // Apply running or gliding movement
        if (isGrounded)
        {
            velocity.x = currentSpeed;
            rb.gravityScale = gravityScale;
            isGlideHolding = false;

            if (currentSpeed < maxSpeed)
            {
                currentSpeed += speedIncreaseRate * Time.fixedDeltaTime;
                Debug.Log("Current Speed: " + currentSpeed);
            }

            if (currentGlideSpeed < maxGlideSpeed)
                currentGlideSpeed += speedIncreaseRate * Time.fixedDeltaTime;
        }
        else
        {
            velocity.x = currentGlideSpeed;

            // Handle gliding behavior
            if (isGlideHolding)
            {
                // Initial glide state after jump — float briefly
                rb.gravityScale = 0f;
                velocity.y = 0f;
            }
            else if (isHoldingUp)
            {
                // Apply wind lift while holding
                rb.gravityScale = glideGravityScale;

                float speedFactor = GetSpeedFactor();
                float dynamicWindLift = windLiftForce * (1f + speedFactor);
                float dynamicMaxVerticalSpeed = maxVerticalSpeed * (1f + speedFactor);

                velocity.y += dynamicWindLift * Time.fixedDeltaTime;
                velocity.y = Mathf.Clamp(velocity.y, -Mathf.Infinity, dynamicMaxVerticalSpeed);
            }
            else
            {
                // Not gliding — fall with full gravity
                rb.gravityScale = gravityScale;
            }
        }

        rb.velocity = velocity;

        // Extra ground check to prevent unwanted sticking or bouncing
        if (isGrounded)
        {
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.3f, groundLayer);
            if (hit.collider != null)
            {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Min(rb.velocity.y, -0.1f));
            }
        }
    }


    private float GetSpeedFactor()
    {
        return Mathf.InverseLerp(runSpeed, maxSpeed, currentSpeed);
    }


    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isGlideHolding = true;
    }

    private void UpdateDistanceCounter()
    {
        float step = Vector3.Distance(transform.position, lastPosition);
        sessionDistance += step;
        lastPosition = transform.position;

        if (GameManager.Instance != null)
            GameManager.Instance.sessionDistance = sessionDistance;

        float kilometers = (sessionDistance * distanceMultiplier) + distanceOffset;
        distanceText.text = kilometers.ToString("F2") + " km";

    }

    private int groundContacts = 0;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            groundContacts++;
            animator.SetBool("running", true);
            animator.SetBool("gliding", false);
            isGrounded = true;

            foreach (ContactPoint2D contact in collision.contacts)
            {
                float angle = Vector2.Angle(contact.normal, Vector2.up);

                // Kill the player only if hitting from sides or bottom
                if (angle > 80f)
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
            if (groundContacts == 0)
                isGrounded = false;
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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveProgressBeforeSceneChange(kmTravelled);
        }

        Debug.Log("Player has died");

        SceneManager.LoadScene("GameOver");
        Destroy(gameObject);
    }

    public void TriggerSpeedBoost()
    {
        if (!isBoosted)
        {
            isBoosted = true;
            boostTimer = boostDuration;

            currentSpeed *= 1.5f;
            currentGlideSpeed *= 1.5f;

            Debug.Log("relic speed boost activated!");
        }
    }

    private bool isKnockedBack = false;
    private float knockbackDuration = 1f;
    private float knockbackTimer = 0f;

    public void ApplyKnockback(Vector2 direction)
    {
        if (!isKnockedBack)
        {
            isKnockedBack = true;
            knockbackTimer = knockbackDuration;

            rb.velocity = Vector2.zero;

            // knockback
            float knockbackSpeed = 15f;
            rb.velocity = new Vector2(direction.x < 0 ? -knockbackSpeed : knockbackSpeed, 0f);

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

            // Store the current speed before boosting
            originalSpeed = currentSpeed;
            originalGlideSpeed = currentGlideSpeed;

            currentSpeed *= multiplier;
            currentGlideSpeed *= multiplier;

            Debug.Log("Wind Gust Boost Activated!");
        }
    }

    public void ForceGrounded()
    {
        isGrounded = true;
        isGlideHolding = false;
        rb.gravityScale = gravityScale;
    }

    private void HandleAudio()
    {
        // Fade speed
        float fade = fadeSpeed * Time.deltaTime;

        if (isGrounded)
        {
            if (!runningSource.isPlaying)
            {
                runningSource.Play();
            }

            // Fade in running
            runningSource.volume = Mathf.MoveTowards(runningSource.volume, runningVolume, fade);

            // Fade out glide
            glideSource.volume = Mathf.MoveTowards(glideSource.volume, 0f, fade);

            // Stop gliding if fully faded out
            if (glideSource.volume <= 0.01f && glideSource.isPlaying)
            {
                glideSource.Stop();
            }
        }
        else
        {
            if (!glideSource.isPlaying)
            {
                glideSource.Play();
            }

            // Fade in gliding
            glideSource.volume = Mathf.MoveTowards(glideSource.volume, glidingVolume, fade);

            // Fade out running
            runningSource.volume = Mathf.MoveTowards(runningSource.volume, 0f, fade);

            // Stop running if fully faded out
            if (runningSource.volume <= 0.01f && runningSource.isPlaying)
            {
                runningSource.Stop();
            }
        }
    }

    public void EnableControls()
    {
        controlsEnabled = true;
    }

    public void DisableControls()
    {
        controlsEnabled = false;
    }

}
