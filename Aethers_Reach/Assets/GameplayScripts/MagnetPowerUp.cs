using UnityEngine;

public class MagnetPowerUp : MonoBehaviour
{
    public float magnetDuration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ActivateMagnet(magnetDuration); // Player handles timer, UI, and coin attraction
            }

            Destroy(gameObject); // remove pickup
        }
    }
}
