using UnityEngine;

public class FollowPlayerX : MonoBehaviour
{
    public Transform player;
    private PlayerController playerController;


    void Start()
    {
        if (player != null)
            playerController = player.GetComponent<PlayerController>();
    }

    void LateUpdate()
    {
        if (player != null)
        {
            // Stop following while player is knocked back
            if (playerController != null && playerController.isKnockedBack)
                return;

            Vector3 targetPos = new Vector3(player.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
        }
    }
}