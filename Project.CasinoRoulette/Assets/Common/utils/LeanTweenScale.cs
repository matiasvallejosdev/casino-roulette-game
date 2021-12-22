using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTweenScale : MonoBehaviour
{

    public float _duration;
    public float _delay;
    public Vector3 _scaleXYZ;

    private void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, _scaleXYZ, _duration)
                 .setDelay(_delay)
                 .setOnComplete(onComplete);
    }

    private void onComplete()
    {
        // Do on complete
    }
}
