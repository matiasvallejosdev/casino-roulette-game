using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shadow_numeroAnterior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public Vector3 GetSpritePivot()
    {
        Vector3 v = this.gameObject.GetComponent<SpriteRenderer>().bounds.center;
        return v;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
