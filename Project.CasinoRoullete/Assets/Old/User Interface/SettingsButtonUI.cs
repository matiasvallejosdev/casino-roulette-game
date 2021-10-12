using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButtonUI : MonoBehaviour
{
    public void OnClick() 
    {
        SoundContoller.Instance.PlayFxSound(3);
        MenuUi.Instance.OpenPanel(2);
    }
}
