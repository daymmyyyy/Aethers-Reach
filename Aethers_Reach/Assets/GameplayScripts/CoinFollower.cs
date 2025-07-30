using UnityEngine;

public class CoinFollower : MonoBehaviour
{
    private Transform player;
    private float attractionRadius = 0f;
    private bool isFollowing = false;

    public void StartFollowing(Transform playerTransform, float radius)
    {
        player = playerTransform;
        attractionRadius = radius;
        isFollowing = true;
    }

    void Update()
    {
        if (!isFollowing || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attractionRadius)
        {
            float speed = 45f;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }
}
