using UnityEngine;

public class BiomeCloudsOutTrigger : MonoBehaviour
{
    [Header("Cloud Transition")]
    public CloudTransitionController cloudTransition;

    private void Start()
    {
        // Play CloudsOut animation at start of the biome
        if (cloudTransition != null)
        {
            cloudTransition.PlayCloudsOut();
        }
    }
}
