using UnityEngine;

public class RelicPiece : MonoBehaviour
{
    public AudioClip collectionClip;

    private bool collected = false;

    private void Start()
    {
        if (RelicManager.Instance != null && RelicManager.Instance.ShouldHideRelics())
        {
            RelicManager.Instance.DestroyRelicPiece(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player")) return;
        collected = true;

        RelicManager.Instance.CollectPiece();

        // Play VFX
        ParticleSystem collectVFX = other.transform.Find("RelicCollectVFX")?.GetComponent<ParticleSystem>();
        if (collectVFX != null)
        {
            collectVFX.Play();
        }

        if (collectionClip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(collectionClip);
        }

        Destroy(gameObject);
    }
}
