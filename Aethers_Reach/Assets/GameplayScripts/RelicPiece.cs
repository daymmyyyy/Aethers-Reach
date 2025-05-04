using UnityEngine;

public class RelicPiece : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RelicManager.Instance.CollectPiece();
            Destroy(gameObject);
        }
    }
}
