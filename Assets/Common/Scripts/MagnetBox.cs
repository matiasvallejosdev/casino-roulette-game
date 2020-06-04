using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<fichas>())
        {
            Destroy(collision.gameObject);
            SoundContoller.Instance.fx_sound(2);
        }
    }
}
