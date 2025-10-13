using UnityEngine;
using System.Collections;

public class CloudTransitionController : MonoBehaviour
{
    public Animator cloudAnimator;        // assign in inspector
    public GameObject cloudObject;        // parent object of clouds
    [SerializeField] private string cloudsInTrigger = "CloudsIn";
    [SerializeField] private string cloudsOutTrigger = "CloudsOut";
    public bool cloudAnimationFinished = false;


    private void Awake()
    {
        if (cloudObject == null)
            cloudObject = gameObject;

        if (cloudAnimator == null)
            cloudAnimator = GetComponent<Animator>();
    }

    public void PlayCloudsIn()
    {
        cloudObject.SetActive(true);
        cloudAnimator.ResetTrigger(cloudsOutTrigger);
        cloudAnimator.SetTrigger(cloudsInTrigger);
    }

    public void PlayCloudsOut()
    {
        cloudObject.SetActive(true);
        cloudAnimator.ResetTrigger(cloudsInTrigger);
        cloudAnimator.SetTrigger(cloudsOutTrigger);
    }

    public void DisableCloudObject()
    {
        cloudObject.SetActive(false);
    }

    public void OnCloudsInFinished()
    {
        cloudAnimationFinished = true;
    }

    public IEnumerator PlayCloudsInAndWait()
    {
        cloudAnimationFinished = false;
        PlayCloudsIn();
        yield return new WaitUntil(() => cloudAnimationFinished);
    }

}
