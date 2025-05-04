using UnityEngine;

public class FollowPlayerX : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        if (player != null)
        {
            transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);
        }
    }
}
