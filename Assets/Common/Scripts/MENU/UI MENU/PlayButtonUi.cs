using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonUi : MonoBehaviour
{
    public float _duration;
    public float _delay;
    public Vector3 _scaleXYZ;

    [Header("Invoke")]
    [SerializeField] private float repeatRate = 0;

    private void OnEnable()
    {
        InvokeRepeating("animButton", _duration * 2, repeatRate);
    }

    private void animButton() 
    {
        LeanTween.scale(gameObject, _scaleXYZ, _duration).setDelay(_delay).setOnComplete(onComplete);
    }

    private void onComplete()
    {
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), _duration);
    }

    public void onClick() 
    {
        game_manager.Instance.unloadLevel("1_Game_Menu");
        game_manager.Instance.loadLevel("2_Game_Roullete");
    }
}
