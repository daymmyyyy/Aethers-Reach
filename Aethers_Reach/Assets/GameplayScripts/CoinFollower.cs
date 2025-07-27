using UnityEngine;

public class CoinFollower : MonoBehaviour
{
    public float followSpeed = 5f;
    private Transform playerTarget;
    private bool isFollowing = false;

    public void StartFollowing(Transform player)
    {
        if (!isFollowing)
        {
            playerTarget = player;
            isFollowing = true;
        }
    }

    private void Update()
    {
        if (isFollowing && playerTarget != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                playerTarget.position,
                followSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isFollowing && other.CompareTag("Player"))
        {
            // Reached player — collect coin
            Destroy(gameObject);
        }
    }
}
