using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shadow_numeroAnterior : MonoBehaviour
{
    public Vector3 GetSpritePivot()
    {
        Vector3 v = this.gameObject.GetComponent<SpriteRenderer>().bounds.center;
        return v;
    }
}
