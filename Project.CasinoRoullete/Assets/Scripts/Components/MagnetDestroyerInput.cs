using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{    
    public class MagnetDestroyerInput : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Chip"))
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
