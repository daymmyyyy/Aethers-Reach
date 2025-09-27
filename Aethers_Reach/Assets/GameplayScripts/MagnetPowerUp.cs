using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MagnetPowerUp : MonoBehaviour
{
    public float magnetDuration = 5f;
    public float attractionRadius = 5f;

    private Slider timerSlider;

    void Awake()
    {
        GameObject sliderObj = GameObject.FindGameObjectWithTag("PowerUpSlider");
        if (sliderObj != null)
        {
            timerSlider = sliderObj.GetComponent<Slider>();
            timerSlider.gameObject.SetActive(false); // hidden at start
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ActivateMagnet(other.transform));
            Destroy(gameObject);
        }
    }

    private IEnumerator ActivateMagnet(Transform player)
    {
        if (timerSlider != null)
        {
            timerSlider.maxValue = magnetDuration;
            timerSlider.value = magnetDuration;
            timerSlider.gameObject.SetActive(true);
        }

        float elapsed = 0f;

        while (elapsed < magnetDuration)
        {
            elapsed += Time.deltaTime;

            if (timerSlider != null)
                timerSlider.value = magnetDuration - elapsed;

            GameObject[] coins = GameObject.FindGameObjectsWithTag("Coins");
            foreach (GameObject coin in coins)
            {
                CoinFollower follower = coin.GetComponent<CoinFollower>();
                if (follower != null)
                {
                    follower.StartFollowing(player, attractionRadius);
                }
            }

            yield return null;
        }

        if (timerSlider != null)
            timerSlider.gameObject.SetActive(false);
    }
}
