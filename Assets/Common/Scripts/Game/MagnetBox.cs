using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<FichaDisplay>())
        {
            Destroy(collision.gameObject);
            SoundContoller.Instance.PlayFxSound(2);
        }
    }
}
