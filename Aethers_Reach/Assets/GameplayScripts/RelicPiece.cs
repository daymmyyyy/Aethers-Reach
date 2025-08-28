using UnityEngine;

public class RelicPiece : MonoBehaviour
{
    public AudioClip collectionClip;
    [Range(0f, 1f)] public float collectionVolume = 0.5f;

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

        ParticleSystem collectVFX = other.transform.Find("ItemCollectVFX")?.GetComponent<ParticleSystem>();
        if (collectVFX != null)
        {
            collectVFX.Play();
        }

        if (collectionClip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.sfxSource.volume = collectionVolume;
            AudioManager.Instance.PlaySFX(collectionClip);

            AudioManager.Instance.sfxSource.volume = 0.3f;
        }

        Destroy(gameObject);
    }
}
