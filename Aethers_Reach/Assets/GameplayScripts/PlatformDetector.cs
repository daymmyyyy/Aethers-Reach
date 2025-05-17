using UnityEngine;

public class PlatformDetector : MonoBehaviour
{
    public PlayerController player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            player.ForceGrounded();
        }
    }
}
