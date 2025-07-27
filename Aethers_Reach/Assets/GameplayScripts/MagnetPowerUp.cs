using System.Collections;
using UnityEngine;

public class MagnetPowerUp : MonoBehaviour
{
    public float magnetDuration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ActivateMagnet(other.transform));
            Destroy(gameObject); // Remove the pickup object
        }
    }

    private IEnumerator ActivateMagnet(Transform player)
    {
        float elapsed = 0f;

        while (elapsed < magnetDuration)
        {
            elapsed += Time.deltaTime;

            GameObject[] coins = GameObject.FindGameObjectsWithTag("Coins");
            foreach (GameObject coin in coins)
            {
                CoinFollower follower = coin.GetComponent<CoinFollower>();
                if (follower != null)
                {
                    follower.StartFollowing(player); // Will only start once
                }
            }

            yield return null;
        }
    }
}
