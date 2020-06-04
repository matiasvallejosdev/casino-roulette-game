using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorC : MonoBehaviour
{
   backNumber_controller sc;
   private void Start() 
   {
       sc = GameObject.Find("BackNumberHUD").GetComponent<backNumber_controller>();
   }

   [MenuItem("Get new number HUD")]
    private void nuevoNumeroInHUD()
    {
        int e = Random.Range(1,37);
        sc.nuevoNumeroHUD(e);
    }
}
