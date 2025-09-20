using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

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

    [Header("Hop 2 Glide Settings")]
    public float hopForce = 5f;         
    public float hopDuration = 0.15f;   // how long the hop lasts

    [Header("Speed Increase Settings")]
    public float speedIncreaseRate = 0.1f;
    public float maxSpeed = 20f;
    public float maxGlideSpeed = 8f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Vector3 lastPosition;

    [Header("Distance Display")]
    public Text distanceText;
    public float distanceMultiplier = 0.001f;

    [Header("Audio Clips")]
    public AudioClip runningLoopClip;
    public AudioClip glidingWindClip;
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
    private bool isJumping;
    private bool controlsEnabled = false;
    private bool isHopping = false;

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
    private float holdThreshold = 0.15f;
    private float jumpTimer;
    private float hopTimer = 0f;

    private int groundContacts = 0;

    private TrailRenderer windTrailVFX;
    private ParticleSystem grassTrailVFX;
    private ParticleSystem sandTrailVFX;
    private ParticleSystem ruinsTrailVFX;


    void Start()
    {
        // Disable controls initially
        controlsEnabled = false;

        StartCoroutine(EnableControlsAfterDelay(1f));

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = gravityScale;

        currentSpeed = runSpeed;
        currentGlideSpeed = glideSpeed;
        sessionDistance = GameManager.Instance != null ? GameManager.Instance.sessionDistance : 0f;
        lastPosition = transform.position;

        windTrailVFX = transform.Find("WindTrailVFX")?.GetComponent<TrailRenderer>();
        grassTrailVFX = transform.Find("LeafGroundTrailVFX")?.GetComponent<ParticleSystem>();
        sandTrailVFX = transform.Find("SandGroundTrailVFX")?.GetComponent<ParticleSystem>();
        ruinsTrailVFX = transform.Find("RuinsGroundTrailVFX")?.GetComponent<ParticleSystem>();

        //set active ground trail based on biome
        string currentScene = SceneManager.GetActiveScene().name;

        grassTrailVFX?.Stop();
        sandTrailVFX?.Stop();
        ruinsTrailVFX?.Stop();
    }

    private IEnumerator EnableControlsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        controlsEnabled = true;
    }

    void Update()
    {
        if (!controlsEnabled) return; // ignore input until enabled

        // Track button state
        if (Input.GetMouseButton(0))
        {
            holdTimer += Time.deltaTime;

            // start hopping when player first presses
            if (holdTimer >= holdThreshold && isGrounded && !isHopping)
            {
                StartHop();
            }

            isHoldingUp = holdTimer >= holdThreshold;
        }
        else
        {
            holdTimer = 0f;
            isHoldingUp = false;
        }

        // Force isHoldingUp to false if grounded
        if (isGrounded)
            isHoldingUp = false;

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
            // Run on the ground
            velocity.x = currentSpeed;
            rb.gravityScale = gravityScale;

            if (currentSpeed < maxSpeed) currentSpeed += speedIncreaseRate * Time.fixedDeltaTime;
            if (currentGlideSpeed < maxGlideSpeed) currentGlideSpeed += speedIncreaseRate * Time.fixedDeltaTime;
        }
        else
        {
            // Air movement
            velocity.x = currentGlideSpeed;

            if (isHopping)
            {
                // Hop phase: apply upward impulse and reduce gravity temporarily
                hopTimer -= Time.fixedDeltaTime;
                if (hopTimer <= 0f)
                {
                    isHopping = false;
                    rb.gravityScale = glideGravityScale; // restore glide gravity after hop
                }
            }
            else if (isHoldingUp)
            {
                // Regular glide after hop
                rb.gravityScale = glideGravityScale;
                float speedFactor = Mathf.InverseLerp(runSpeed, maxSpeed, currentSpeed);
                float lift = windLiftForce * 2f * (1f + speedFactor); // double lift
                float maxY = maxVerticalSpeed * 1.5f * (1f + speedFactor); // increase maxY

                velocity.y += lift * Time.fixedDeltaTime;
                velocity.y = Mathf.Clamp(velocity.y, -Mathf.Infinity, maxY);
            }
            else
            {
                // Falling
                rb.gravityScale = gravityScale + 3f; // double gravity when descending
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

        distanceText.text = ((sessionDistance * distanceMultiplier)).ToString("F2") + "m";
    }

    private void UpdateAnimationAndVFX()
    {
        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
            isJumping = true;
        }
        else
        {
            isJumping = false;
        }

        if (isGrounded)
        {
            //if grounded but were gliding, FORCE snap to running
            if (!wasGroundedLastFrame || animator.GetCurrentAnimatorStateInfo(0).IsName("FlightIdleV2") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("FlightDescent"))
            {
                animator.ResetTrigger("run2glide");
                animator.ResetTrigger("descent2glide");
                animator.ResetTrigger("glide2descent");

                //force-snap to Run, no transition
                animator.Play("PlayerRunV2", 0, 0f);
                animator.Update(0f);
            }

            animator.SetBool("running", true);
            animator.SetBool("gliding", false);
            animator.SetBool("descending", false);

            //trails for each biome
            if (SceneManager.GetActiveScene().name.Contains("Biome1"))
            {
                if (!grassTrailVFX.isPlaying) grassTrailVFX?.Play();
                sandTrailVFX?.Stop();
                ruinsTrailVFX?.Stop();
            }
            else if (SceneManager.GetActiveScene().name.Contains("Biome2"))
            {
                if (!sandTrailVFX.isPlaying) sandTrailVFX?.Play();
                grassTrailVFX?.Stop();
                ruinsTrailVFX?.Stop();
            }
            else if (SceneManager.GetActiveScene().name.Contains("Biome3"))
            {
                if (!ruinsTrailVFX.isPlaying) ruinsTrailVFX?.Play();
                grassTrailVFX?.Stop();
                sandTrailVFX?.Stop();
            }
        }
        else
        {
            animator.SetBool("running", false);
            animator.SetBool("gliding", isHoldingUp);
            animator.SetBool("descending", !isHoldingUp);

            if (!wasGroundedLastFrame && isHoldingUp && wasHoldingUpLastFrame)
                animator.SetTrigger("descent2glide");
            else if (!wasHoldingUpLastFrame && !isHoldingUp)
                animator.SetTrigger("glide2descent");
            else if (wasGroundedLastFrame && isHoldingUp)
                animator.SetTrigger("run2glide");

            grassTrailVFX?.Stop();
            sandTrailVFX?.Stop();
            ruinsTrailVFX?.Stop();
            if (windTrailVFX != null) windTrailVFX.emitting = true;
        }

        wasGroundedLastFrame = isGrounded;
        wasHoldingUpLastFrame = isHoldingUp;
    }

    private void Jump()
    {
        Vector2 velocity = rb.velocity;
        velocity.y = jumpForce;
        rb.velocity = velocity;
        isJumping = true;
        jumpTimer = 0.2f; // Prevent gliding
    }

    private void HandleAudio()
    {
        if (AudioManager.Instance == null) return;

        // Choose clip based on grounded/gliding
        AudioClip targetClip = isGrounded ? runningLoopClip : glidingWindClip;

        if (targetClip == null)
        {
            // Fade out if no clip
            AudioManager.Instance.sfxSource.volume = Mathf.MoveTowards(
                AudioManager.Instance.sfxSource.volume, 0f, fadeSpeed * Time.deltaTime
            );
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

        // Fade volume toward the AudioManager SFX volume slider
        float targetVolume = AudioManager.Instance.GetSFXVolume(); // fully controlled by slider
        AudioManager.Instance.sfxSource.volume = Mathf.MoveTowards(
            AudioManager.Instance.sfxSource.volume, targetVolume, fadeSpeed * Time.deltaTime
        );
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

            //Handheld.Vibrate();
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

        if (collision.collider.CompareTag("BottomLimit"))
        {
            AudioManager.Instance.musicSource.Stop();
            AudioManager.Instance.sfxSource.Stop();
            Die();
        }
        else if (collision.collider.CompareTag("TopLimit"))
        {
            ApplyDownwardGust();
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

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver();
        }

        SceneManager.LoadScene("GameOver");
        Destroy(gameObject);
    }

    private void ApplyDownwardGust()
    {
        rb.velocity = new Vector2(rb.velocity.x, -15f);

        ActivateWindGustBoost(1.2f, 1f);

        isKnockedBack = true;
        knockbackTimer = 0.5f;
    }

    private void StartHop()
    {
        isHopping = true;
        hopTimer = hopDuration;

        rb.velocity = new Vector2(rb.velocity.x, hopForce);

        //temporarily reduce gravity so hop feels floaty
        rb.gravityScale = glideGravityScale * 0.5f;
    }

}

