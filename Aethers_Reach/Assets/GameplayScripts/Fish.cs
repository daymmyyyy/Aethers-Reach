using UnityEngine;

public class Fish : MonoBehaviour
{
    public float moveSpeed = 5f;
    private bool shouldMove = false;
    private Camera mainCamera;
    private bool hasHitPlayer = false;
    public float breakSpeedThreshold = 50f;

    [Header("Collision SFX")]
    public AudioClip collisionSFX;

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
                    // Player breaks through fish
                    Collider2D col = GetComponent<Collider2D>();
                    if (col != null) col.enabled = false; // remove collider

                    SpriteRenderer sr = GetComponent<SpriteRenderer>();
                    if (sr != null) sr.enabled = false; // hide sprite

                    // Play VFX
                    Transform vfxTransform = transform.Find("CreatureCloudBurst");
                    if (vfxTransform != null)
                    {
                        ParticleSystem crashVFX = vfxTransform.GetComponent<ParticleSystem>();
                        if (crashVFX != null)
                        {
                            crashVFX.Play();
                            StartCoroutine(DestroyAfterVFX(crashVFX));
                        }
                        else
                        {
                            Destroy(gameObject);
                        }
                    }
                    else
                    {
                        Destroy(gameObject);
                    }

                    Debug.Log("Fish destroyed due to high player speed!");
                    return;
                }

                hasHitPlayer = true;

                // Play collision SFX
                if (collisionSFX != null && AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(collisionSFX);
                }

                // Apply knockback
                Vector2 direction = (player.transform.position - transform.position).normalized;
                player.ApplyKnockback(direction);

                // Play VFX for relic loss
                Transform lossVFXTransform = other.transform.Find("CrystalLossBurstVFX");
                if (lossVFXTransform != null)
                {
                    ParticleSystem knockbackVFX = lossVFXTransform.GetComponent<ParticleSystem>();
                    if (knockbackVFX != null && knockbackVFX.gameObject.activeInHierarchy)
                    {
                        knockbackVFX.Play();
                    }
                }

                // Lose relics
                int lossAmount = 20; // penalty amount
                RelicCurrency.LoseCurrency(lossAmount);
            }
        }
    }

    private System.Collections.IEnumerator DestroyAfterVFX(ParticleSystem vfx)
    {
        float waitTime = vfx.main.duration + vfx.main.startLifetime.constantMax;
        yield return new WaitForSeconds(waitTime);

        Destroy(gameObject);
    }
}
