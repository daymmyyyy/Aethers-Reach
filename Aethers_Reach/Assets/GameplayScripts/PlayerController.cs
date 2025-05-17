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

    void Start()
    {
       sessionDistance = GameManager.Instance != null ? GameManager.Instance.sessionDistance : 0f;

        lastPosition = transform.position; 

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;


        currentSpeed = runSpeed;
        currentGlideSpeed = glideSpeed;

    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isHoldingUp = Input.GetMouseButton(0);

        if (isGrounded && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetMouseButtonDown(0)))
        {
            Jump();
        }

        UpdateDistanceCounter();
    }

    void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;

        if (isKnockedBack)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
            return;
        }

        // Handle recovery after knockback or boost
        if (recoveringSpeed)
        {
            speedRecoveryTimer += Time.fixedDeltaTime;
            float t = speedRecoveryTimer / speedRecoveryDuration;

            currentSpeed = Mathf.Lerp(boostedSpeedStart, originalSpeed, t);
            currentGlideSpeed = Mathf.Lerp(boostedGlideSpeedStart, originalGlideSpeed, t);

            if (t >= 1f)
            {
                recoveringSpeed = false;
            }
        }

        if (isBoosted)
        {
            boostTimer -= Time.fixedDeltaTime;
            if (boostTimer <= 0f)
            {
                isBoosted = false;

                speedRecoveryTimer = 0f;
                recoveringSpeed = true;

                boostedSpeedStart = currentSpeed;
                boostedGlideSpeedStart = currentGlideSpeed;
            }
        }


        // Movement and gravity logic
        if (isGrounded)
        {
            velocity.x = currentSpeed;
            rb.gravityScale = gravityScale;
            isGlideHolding = false;

                if (currentSpeed < maxSpeed)
                    currentSpeed += speedIncreaseRate * Time.fixedDeltaTime;

                if (currentGlideSpeed < maxGlideSpeed)
                    currentGlideSpeed += speedIncreaseRate * Time.fixedDeltaTime;
        }
        else
        {
            velocity.x = currentGlideSpeed;

            if (isGlideHolding)
            {
                rb.gravityScale = 0f;
                velocity.y = 0f;
            }
            else
            {
                if (isHoldingUp)
                {
                    rb.gravityScale = glideGravityScale;
                    velocity.y += windLiftForce * Time.fixedDeltaTime;
                    velocity.y = Mathf.Clamp(velocity.y, -Mathf.Infinity, maxVerticalSpeed);
                }
                else
                {
                    rb.gravityScale = gravityScale;
                }
            }
        }

        rb.velocity = velocity;

        if (isGrounded)
        {
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.3f, groundLayer);
            if (hit.collider != null)
            {
                // Snap to slope
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Min(rb.velocity.y, -0.1f));
            }
        }

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("TopLimit") || collision.collider.CompareTag("BottomLimit"))
        {
            Die();
        }

        if (collision.collider.CompareTag("Ground"))
        {
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


}
