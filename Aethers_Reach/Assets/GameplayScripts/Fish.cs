using UnityEngine;

public class Fish : MonoBehaviour
{
    public float moveSpeed = 5f;
    private bool shouldMove = false;
    private Camera mainCamera;
    private bool hasHitPlayer = false;
    public float breakSpeedThreshold = 60f;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);

        if (!shouldMove && screenPoint.x >= 0f && screenPoint.x <= 1f)
        {
            shouldMove = true;
        }

        if (shouldMove)
        {
            if (screenPoint.x < -0.1f)
            {
                shouldMove = false;
            }
            else
            {
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            }
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHitPlayer) return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                float playerSpeed = player.GetComponent<Rigidbody2D>().velocity.x;

                if (playerSpeed >= breakSpeedThreshold)
                {
                    Destroy(gameObject);
                    Debug.Log("Fish destroyed due to high player speed!");
                    return;
                }

                hasHitPlayer = true;

                Vector2 direction = (player.transform.position - transform.position).normalized;
                player.ApplyKnockback(direction);

                // Lose relics
                if (RelicManager.Instance != null)
                {
                    RelicManager.Instance.LoseRelics(2);
                    RelicManager.Instance.DropRelics(2, transform);
                }
            }
        }
    }



}
