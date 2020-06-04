using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manejador_fichas : MonoBehaviour
{
    [SerializeField]private GameObject[] _fichas;
    [SerializeField]private string[] _nameFichas;
    private int _fichaSelected;
    private int _countNewFichas;  

    // Start is called before the first frame update
    void Start()
    {
        find_fichas();
    }
    private void find_fichas()
    {
        //_fichaSombra = GameObject.Find("ficha_sombra");
        for(int i = 0; i < _fichas.Length; i++)
        {
            _fichas[i].gameObject.transform.parent = gameObject.transform;
            _fichas[i].gameObject.SetActive(false);
        }  
    }
    public int valueFicha()
    {
        return _fichas[_fichaSelected].GetComponent<fichas>().getValueOfBet();
    } 
    public void nueva_ficha(Vector2 v, string ficha, bool fichasOnTop, Vector2 offsetFicha, string cl, int[] va, bool pl, GameObject btnPressed)
    {
        _countNewFichas++;
        string selected_ficha = ficha;
        // Buscar el numero de la ficha seleccionada
        GameObject newFicha = Instantiate(_fichas[_fichaSelected]);
        // Inicializa la posicion de la ficha y el valor de las posiciones en memoria
        fichas f = newFicha.GetComponent<fichas>();
        f.setPosicion(cl,va,pl,_countNewFichas, btnPressed);
        // Instancia una nueva
        GameObject shadow = newFicha.transform.GetChild(0).gameObject;
        newFicha.name = _countNewFichas.ToString();
        // Setea de hijo fichas nuevas
        if (pl)
        {
            newFicha.transform.SetParent(GameObject.Find("Fichas_Nuevas").transform.GetChild(0).transform);
        }
        else
        {
            newFicha.transform.SetParent(GameObject.Find("Fichas_Nuevas").transform.GetChild(1).transform);
        }
        // Position Center Pivot
        if (fichasOnTop)
        {
            // crea dos fichas 
            // 1 sombra
            // 1 normal
            shadow.SetActive(true);
            newFicha.SetActive(true);
            newFicha.transform.position = v + offsetFicha;
        } else
        {
            newFicha.SetActive(true);
            shadow.SetActive(false);
            newFicha.transform.position = v;
        }
    }
    public void num_ficha(int numero)
    {
        _fichaSelected = numero;
    }

    public void previous_ficha(Vector2 v, bool fichasOnTop, Vector2 offsetFicha, GameObject prev, string cl, int[] va, bool pl, GameObject btnPressed)
    {
        _countNewFichas++;
        // Buscar el numero de la ficha seleccionada
        GameObject prevFicha = Instantiate(prev);
        // Inicializa la posicion de la ficha y el valor de las posiciones en memoria
        fichas f = prevFicha.GetComponent<fichas>();
        // Instancia una nueva
        f.setPosicion(cl, va, pl, _countNewFichas, btnPressed);
        // Instancia una nueva
        GameObject shadow = prevFicha.transform.GetChild(0).gameObject;
        prevFicha.name = _countNewFichas.ToString();
        // Setea de hijo fichas nuevas
        if (f._pleno)
        {
            prevFicha.transform.SetParent(GameObject.Find("Fichas_Nuevas").transform.GetChild(0).transform);
        }
        else
        {
            prevFicha.transform.SetParent(GameObject.Find("Fichas_Nuevas").transform.GetChild(1).transform);
        }
        // Position Center Pivot
        if (fichasOnTop)
        {
            // crea dos fichas 
            // 1 sombra
            // 1 normal
            shadow.SetActive(true);
            prevFicha.SetActive(true);
            prevFicha.transform.position = v + offsetFicha;
        }
        else
        {
            prevFicha.SetActive(true);
            shadow.SetActive(false);
            prevFicha.transform.position = v;
        }
    }
}
