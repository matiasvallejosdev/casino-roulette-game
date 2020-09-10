using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteButtonUI : MonoBehaviour
{
    public void delete()
    {
        SoundContoller.Instance.PlayFxSound(1);
        RoundController.Instance.ResetTable();
    }
}
