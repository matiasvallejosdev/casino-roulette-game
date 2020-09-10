using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PanelSettignsUI : MonoBehaviour
{
    public struct colors
    {
        public Color32 red;
        public Color32 green;
    }
    public UnityEngine.UI.Button _btnMusic;
    public UnityEngine.UI.Button _btnSound;
    public UnityEngine.UI.Button _btntableSettings;
    public UnityEngine.UI.Button _btnprivacy;
    public UnityEngine.UI.Button _btnterms;

    colors structColors;

    void Start()
    {
        _btnMusic.onClick.AddListener(HandlerMusicClick);
        _btnSound.onClick.AddListener(HandlerSoundClick);
        _btntableSettings.onClick.AddListener(HandlerTableSettingsClick);
        _btnprivacy.onClick.AddListener(HandlerPrivacyClick);
        _btnterms.onClick.AddListener(HandlerTermsClick);

        structColors.red = new Color32(255, 27, 0, 255);
        structColors.green = new Color32(35, 255, 0, 255);
    }

    public void HandlerMusicClick() 
    {
        if (!SoundContoller.Instance.GetMusicStatus()) 
        {
            _btnMusic.transform.GetChild(0).GetComponent<Image>().color = structColors.red;
            SoundContoller.Instance.SetMusicMute(true);
        } 
        else 
        {
            _btnMusic.transform.GetChild(0).GetComponent<Image>().color = structColors.green;
            SoundContoller.Instance.SetMusicMute(false);
        }
    }
    public void HandlerSoundClick()
    {
        if (!SoundContoller.Instance.GetSoundStatus())
        {
            _btnSound.transform.GetChild(0).GetComponent<Image>().color = structColors.red;
            SoundContoller.Instance.SetSoundMute(true);
        }
        else
        {
            _btnSound.transform.GetChild(0).GetComponent<Image>().color = structColors.green;
            SoundContoller.Instance.SetSoundMute(false);
        }
    }
    public void HandlerTableSettingsClick()
    {

    }
    public void HandlerPrivacyClick()
    {

    }
    public void HandlerTermsClick()
    {

    }
}
