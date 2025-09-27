using UnityEngine;

public class InvincibilityPowerUp : MonoBehaviour
{
    [Tooltip("How long the player stays invincible when collected")]
    public float invincibilityDuration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            // Tell the player to handle invincibility
            pc.ActivateInvincibility(invincibilityDuration);
        }

        Destroy(gameObject); // remove the pickup object
    }
}
