using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButtonUI : MonoBehaviour
{
    public void handelPauseUI()
    {
        SoundContoller.Instance.PlayFxSound(3);
        GameManager.Instance.togglePause();
    }
}
