using UnityEngine;

public class PlayVFX : MonoBehaviour
{
    [Header("VFX to Play")]
    public ParticleSystem[] vfxList;

    private bool hasPlayed = false;

    public void Play()
    {
        if (hasPlayed) return;

        foreach (ParticleSystem vfx in vfxList)
        {
            if (vfx != null)
            {
                vfx.Play();
            }
        }

        hasPlayed = true;
    }
}
