using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsButtonUI : MonoBehaviour
{
    public void OnClick()
    {
        SoundContoller.Instance.PlayFxSound(3);
        MenuUi.Instance.OpenPanel(1);
    }
}
