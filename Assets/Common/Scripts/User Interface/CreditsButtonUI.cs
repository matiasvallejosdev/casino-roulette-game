using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsButtonUI : MonoBehaviour
{
    public void OnClick()
    {
        SoundContoller.Instance.PlayFxSound(3);
        MenuUi.Instance.OpenPanel(3);
    }
}
