using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarnMoneyUI : MonoBehaviour
{
    public float to = 0;
    public float speed = 0;

    private void OnEnable()
    {
        LeanTween.moveLocalY(gameObject, to, speed).setLoopPingPong();
    }

    // -68 * 0.6f
}
