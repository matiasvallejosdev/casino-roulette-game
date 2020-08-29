using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class manejador_ball : MonoBehaviour
{
    public GameObject _newSphere;

    private GameObject _instanceSphere;
    private GameObject[] manejadores;
    private Vector3 pos_ball;

    // Start is called before the first frame update
    void Start()
    {
        manejadores = GameObject.FindGameObjectsWithTag("Numeros_Ruleta");
        _instanceSphere = GameObject.Find("InstanceSphere");
        _instanceSphere.SetActive(false);
    }
    /// <summary>
    /// Instantiate the new ball and set position in the number 
    /// </summary>
    /// <param name="numero"></param>
    public void colocar_ball(int numero)
    {
        // Activa el ball y lo posiciona en el numero indicado!
        for(int i = 0; i < manejadores.Length; i++)
        {
            string name = "handler_" + (numero.ToString());
            if(manejadores[i].name == name)
            {
                // Effect instance ball in roullete
                pos_ball = new Vector3(manejadores[i].transform.position.x,manejadores[i].transform.position.y,manejadores[i].transform.position.z);
                _newSphere = Instantiate(_instanceSphere);
                _newSphere.transform.position = pos_ball;
                _newSphere.SetActive(true);
                _newSphere.transform.parent = GameObject.Find("Center").transform;
                //Debug.Log("Ball posicionada en el numero " + numero);
                // Look Number Winner
                fx_nuevoNumero a = GetComponent<fx_nuevoNumero>();
                a.effectNewNumber(numero);
            }
        }
    }

}
