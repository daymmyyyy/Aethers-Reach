using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (RelicManager.Instance != null && RelicManager.Instance.ShouldHideRelics())
        {
            RelicManager.Instance.DestroyShrines(gameObject);
        }
    }

}
