using UnityEngine;

public class RelicPiece : MonoBehaviour
{
    public AudioClip collectionClip;
    [Range(0f, 1f)] public float collectionVolume = 0.5f;

    private bool collected = false;

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

        if (collectionClip != null)
        {
            AudioSource playerAudio = other.GetComponent<AudioSource>();
            if (playerAudio == null)
            {
                playerAudio = other.gameObject.AddComponent<AudioSource>();
            }

            playerAudio.PlayOneShot(collectionClip, collectionVolume);
        }

        Destroy(gameObject);
    }
}
