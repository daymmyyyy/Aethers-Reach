using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Ground Movement Settings")]
    public float runSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Glide & Wind Settings")]
    public float glideSpeed = 4f;      
    public float windLiftForce = 8f;
    public float maxVerticalSpeed = 5f;
    public float gravityScale = 3f;
    public float glideGravityScale = 0.8f;
    private float takeoffMomentum = 0f;
    public float momentumMultiplier = 1f;


    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Distance Tracking")]
    public Text distanceText;
    public float distanceMultiplier = 0.01f;
    private Vector3 startPosition;


    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isHoldingUp;
    private bool hasStartedGliding = false;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        startPosition = transform.position;
    }

    void Update()
    {
        // Check if player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isHoldingUp = Input.GetKey(KeyCode.UpArrow);

        // Jump if grounded
        if (isGrounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }

        // First-time gliding activation
        if (!hasStartedGliding && !isGrounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            hasStartedGliding = true;
            takeoffMomentum = rb.velocity.magnitude; // momentum at the start of gliding

        }

        UpdateDistanceCounter();
    }

    void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;

        if (isGrounded)
        {
            // Move forward when grounded
            velocity.x = runSpeed;
        }
        else
        {
            if (hasStartedGliding && isHoldingUp)
            {
                float adjustedLift = windLiftForce * Mathf.Clamp01(takeoffMomentum * momentumMultiplier);

                // apply wind lift and forward glide
                velocity.x = glideSpeed;
                velocity.y += adjustedLift * Time.fixedDeltaTime;
                velocity.y = Mathf.Min(windLiftForce, maxVerticalSpeed);
                rb.gravityScale = glideGravityScale;
            }
            else
            {
                // Fall normally, keep horizontal velocity
                velocity.x = rb.velocity.x;
                rb.gravityScale = gravityScale;
            }
        }

        rb.velocity = velocity;
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        takeoffMomentum = rb.velocity.magnitude;
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

        // Loop through all contact points
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;

            // If hitting bottom or side
            if (normal.y < 0.5f)
            {
                Die();
                break;
            }
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // add death animation
        SceneManager.LoadScene("MainMenu");
        Destroy(gameObject);
    }


}
