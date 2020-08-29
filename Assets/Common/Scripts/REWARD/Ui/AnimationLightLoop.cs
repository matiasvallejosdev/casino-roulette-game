using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationLightLoop : MonoBehaviour
{
    [SerializeField] private float _durationLoop = 0;
    [SerializeField] private float _delayLoop = 0;

    private void Start()
    {
        InvokeRepeating("AnimationLoop", _durationLoop, _delayLoop);
    }

    private void AnimationLoop() 
    {
        if (gameObject.activeSelf) 
        {
            gameObject.SetActive(false);
        } else 
        {
            gameObject.SetActive(true);
        }
    }
}
