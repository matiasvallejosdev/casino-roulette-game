using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HandlerBall : MonoBehaviour
{
    /*
    public GameObject RotatorBallSphere;
    public GameObject instanceSphere;
    public GameObject[] handlerNumberWheel;

    private Vector3 _ballPosition;

    /// <summary>
    /// Instantiate the new ball and set position in the number 
    /// </summary>
    /// <param name="numero"></param>
    public void SetBallInWheel(int numero)
    {
        // Activa el ball y lo posiciona en el numero indicado!
        for(int i = 0; i < handlerNumberWheel.Length; i++)
        {
            string name = "handler_" + (numero.ToString());
            if(handlerNumberWheel[i].name == name)
            {
                // Effect instance ball in roullete
                _ballPosition = new Vector3(handlerNumberWheel[i].transform.position.x,handlerNumberWheel[i].transform.position.y,handlerNumberWheel[i].transform.position.z);
                RotatorBallSphere = Instantiate(instanceSphere);
                RotatorBallSphere.transform.position = _ballPosition;
                RotatorBallSphere.SetActive(true);
                RotatorBallSphere.transform.parent = GameObject.Find("Center").transform;
                //Debug.Log("Ball posicionada en el numero " + numero);
                // Look Number Winner
                FxNewNumber a = GetComponent<FxNewNumber>();
                a.SetEffectTableToNewNumber(numero);
            }
        }
    }*/
}
