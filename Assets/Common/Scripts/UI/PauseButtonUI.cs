using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButtonUI : MonoBehaviour
{
    public void handelPauseUI()
    {
        SoundContoller.Instance.fx_sound(4);
        game_manager.Instance.togglePause();
    }
}
