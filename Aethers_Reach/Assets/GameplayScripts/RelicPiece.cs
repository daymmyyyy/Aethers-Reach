using UnityEngine;

public class RelicPiece : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RelicManager.Instance.CollectPiece();

            //find and play the child particle system on the player
            ParticleSystem collectVFX = other.transform.Find("ItemCollectVFX")?.GetComponent<ParticleSystem>();
            if (collectVFX != null)
            {
                collectVFX.Play();
            }

            Destroy(gameObject);
        }
    }
}
