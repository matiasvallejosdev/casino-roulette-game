using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTweenScaleDestroy : MonoBehaviour
{
    [SerializeField] private float _duration = 0; 
    [SerializeField] private float _delay = 0;

    private void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, new Vector3(0.5829054f, 0.5829054f, 0.5829054f), _duration).setDelay(_delay).setOnComplete(Desaparecer);
    }

    private void Desaparecer()
    {
        LeanTween.scale(gameObject, new Vector3(0, 0, 0), _duration).setDelay(_delay).setOnComplete(onComplete);    
    }

    private void onComplete()
    {
        Destroy(gameObject);
    }
}
