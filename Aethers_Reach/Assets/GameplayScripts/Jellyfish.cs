using UnityEngine;

public class Jellyfish : MonoBehaviour
{
    private bool hasHitPlayer = false;

    [Header("Collision SFX")]
    public AudioClip collisionSFX;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHitPlayer) return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                hasHitPlayer = true;

                // Play collision SFX
                if (collisionSFX != null && AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(collisionSFX);
                }

                // Apply knockback
                Vector2 direction = (player.transform.position - transform.position).normalized;
                player.ApplyKnockback(direction);

                // Play VFX
                Transform vfxTransform = other.transform.Find("CrystalLossBurstVFX");
                if (vfxTransform != null)
                {
                    ParticleSystem knockbackVFX = vfxTransform.GetComponent<ParticleSystem>();
                    if (knockbackVFX != null && knockbackVFX.gameObject.activeInHierarchy)
                    {
                        knockbackVFX.Play();
                    }
                }

                // Lose relics
                int lossAmount = 20; // choose penalty
                RelicCurrency.LoseCurrency(lossAmount);
            }
        }
    }
}
