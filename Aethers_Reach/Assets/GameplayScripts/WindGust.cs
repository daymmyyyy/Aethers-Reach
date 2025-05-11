using UnityEngine;

public class WindGust : MonoBehaviour
{
    public float boostMultiplier = 1.5f;
    public float boostDuration = 2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ActivateWindGustBoost(boostMultiplier, boostDuration);
            }
        }
    }
}
