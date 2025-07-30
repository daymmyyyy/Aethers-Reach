using UnityEngine;

public class InvincibilityPowerUp : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
            Destroy(other.gameObject);

            DisableTaggedColliders("Obstacle");
            DisableTaggedColliders("Ground");
    }

    void DisableTaggedColliders(string tag)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject obj in taggedObjects)
        {
            Collider2D col = obj.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
            }
        }
    }
}
