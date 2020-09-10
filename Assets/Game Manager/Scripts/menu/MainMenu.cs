using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Track the Animation Component
    // Track the AnimationClips for fade in/out
    // Function that can receive animations events
    // Function to play fade in/out

    [SerializeField] public Animation _mainMenuAnimator;
    [SerializeField] public AnimationClip _fadeOutClip;
    [SerializeField] public AnimationClip _fadeInClip;

    public Events.EventFadeComplete OnMainMenuFadeComplete;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    public void OnFadeOutComplete()
    {
        OnMainMenuFadeComplete.Invoke(true);
        Debug.LogWarning("Fadeout Complete");

        actualizarSaldos();
    }

    public void OnFadeInComplete()
    {
        OnMainMenuFadeComplete.Invoke(false);
        Debug.LogWarning("Fadein Complete");
        UIManager.Instance.SetDummyCameraActive(true);
    }


    void HandleGameStateChanged(GameManager.GameState curentState, GameManager.GameState previous)
    {
        if(previous == GameManager.GameState.PREGAME && curentState == GameManager.GameState.RUNING)
        {
            FadeOut();
        }
    }

    public void FadeIn()
    {
        _mainMenuAnimator.Stop();
        _mainMenuAnimator.clip = _fadeInClip;
        _mainMenuAnimator.Play();
    }
    public void FadeOut()
    {
        UIManager.Instance.SetDummyCameraActive(false);
        _mainMenuAnimator.Stop();
        _mainMenuAnimator.clip = _fadeOutClip;
        _mainMenuAnimator.Play();
    }

    private void actualizarSaldos()
    {
        saldo();
        apuesta();
    }

    private void saldo()
    {

    }

    private void apuesta()
    {

    }
}
