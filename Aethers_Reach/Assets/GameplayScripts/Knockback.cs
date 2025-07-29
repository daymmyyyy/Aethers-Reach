using UnityEngine;

public class Knockback : MonoBehaviour
{
    private bool hasHitPlayer = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHitPlayer || !other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            hasHitPlayer = true;

            // Knockback direction
            Vector2 direction = (player.transform.position - transform.position).normalized;
            player.ApplyKnockback(direction);

            // Optional: Play knockback VFX
            Transform vfxTransform = other.transform.Find("CrashKnockbackVFX");
            if (vfxTransform != null)
            {
                ParticleSystem knockbackVFX = vfxTransform.GetComponent<ParticleSystem>();
                if (knockbackVFX != null && knockbackVFX.gameObject.activeInHierarchy)
                {
                    knockbackVFX.Play();
                }
            }
        }
    }
}
