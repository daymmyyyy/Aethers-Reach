using UnityEngine;

public class Bounce : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float bounceForce = 12f; // upward velocity
    public AudioClip bounceSFX;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player hit the top of the jellyfish
        if (collision.collider.CompareTag("Player"))
        {
            ContactPoint2D contact = collision.contacts[0];
            // Only bounce if the player hits the top
            if (Vector2.Dot(contact.normal, Vector2.up) > 0.4f)
            {
                Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    // Apply upward velocity for bounce
                    Vector2 velocity = playerRb.velocity;
                    velocity.y = bounceForce;
                    playerRb.velocity = velocity;

                    if (bounceSFX != null && AudioManager.Instance != null)
                        AudioManager.Instance.PlaySFX(bounceSFX);
                }
            }
        }
    }
}
