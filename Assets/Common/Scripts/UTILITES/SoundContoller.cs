using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundContoller : Singlenton<SoundContoller>
{
    [Header("Fx Sound")]
    public AudioClip[] fx;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void fx_sound(int pos)
    {
        _audioSource.clip = fx[pos];
        _audioSource.loop = false;
        _audioSource.Play();
    }
}
