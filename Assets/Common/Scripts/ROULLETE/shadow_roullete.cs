using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shadow_roullete : MonoBehaviour
{
    public void onAnimationOffFinished() 
    {
        this.gameObject.SetActive(false);
    }
}
