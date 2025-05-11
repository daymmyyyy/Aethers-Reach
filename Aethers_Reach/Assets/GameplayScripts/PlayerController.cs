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
    public float glideHoldTime = 3f;

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

    [Header("Relic Speed Boost Settings")]
    public float boostDuration = 2f;
    private float boostTimer = 0f;
    private bool isBoosted = false;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isHoldingUp;
    private bool isGlideHolding;
    private float glideHoldTimer = 0f;
    private float currentSpeed;
    private float currentGlideSpeed;
    private Vector3 startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        startPosition = transform.position;
        currentSpeed = runSpeed;
        currentGlideSpeed = glideSpeed;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isHoldingUp = Input.GetKey(KeyCode.UpArrow);

        if (isGrounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }

        UpdateDistanceCounter();
    }

    void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;

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
                glideHoldTimer -= Time.fixedDeltaTime;

                if (glideHoldTimer <= 0f)
                {
                    isGlideHolding = false;
                }
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

        if (isKnockedBack)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
            return; // Skip normal movement while knocked back
        }

        rb.velocity = velocity;
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isGlideHolding = true;
        glideHoldTimer = glideHoldTime;
    }

    private void UpdateDistanceCounter()
    {
        float distanceTravelled = Vector3.Distance(startPosition, transform.position);
        float kilometers = distanceTravelled * distanceMultiplier;
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
                if (contact.normal.y < 0.5f)
                {
                    Die();
                    break;
                }
            }
        }
    }

    private void Die()
    {
        float distanceTravelled = Vector3.Distance(startPosition, transform.position);
        float kilometers = distanceTravelled * distanceMultiplier;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveScore(kilometers);
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

            Debug.Log("Relic Speed Boost Activated!");
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
        }
    }




}
