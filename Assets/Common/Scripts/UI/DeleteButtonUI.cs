using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteButtonUI : MonoBehaviour
{
    public void delete()
    {
        SoundContoller.Instance.fx_sound(4);
        RoundController.Instance.DeleteFichasInTable();
    }
}
