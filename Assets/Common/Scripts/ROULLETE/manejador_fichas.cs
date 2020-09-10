using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class manejador_fichas : MonoBehaviour
{
    [SerializeField]private GameObject[] _fichas = null;
    [SerializeField]private string[] _nameFichas;
    private int _fichaSelected = 0;
    private int _countNewFichas = 0;

    public int valueOfFichaAll;

    // Start is called before the first frame update
    void Start()
    {
        find_fichas();
        RoundController.Instance.OnGameOpened();
    }
    private void find_fichas()
    {
        for(int i = 0; i < _fichas.Length; i++)
        {
            _fichas[i].gameObject.transform.SetParent(gameObject.transform);
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
        if(_fichas[_fichaSelected].name == "ficha_all") 
        {
            f.setValor(valueOfFichaAll);
        }
        // Instancia una nueva
        GameObject shadow = newFicha.transform.GetChild(0).gameObject;
        newFicha.name = _countNewFichas.ToString();
        // Setea de hijo fichas nuevas
        if(pl)
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
            f.setPosicion(v + offsetFicha, cl, va, pl, _countNewFichas, btnPressed, _fichaSelected);

        }
        else
        {
            newFicha.SetActive(true);
            shadow.SetActive(false);
            newFicha.transform.position = v;
            f.setPosicion(v, cl, va, pl, _countNewFichas, btnPressed, _fichaSelected);

        }
    }
    public void num_ficha(int numero)
    {
        _fichaSelected = numero;
    }

    public void RecoverFichas(Vector2 v, bool fichasOnTop, Vector2 offsetFicha, string cl, int[] va, bool pl, GameObject btnPressed, int fichaIndex)
    {
        _countNewFichas++;
        // Buscar el numero de la ficha seleccionada
        GameObject prevFicha = Instantiate(_fichas[fichaIndex]);
        // Inicializa la posicion de la ficha y el valor de las posiciones en memoria
        fichas f = prevFicha.GetComponent<fichas>();
        if (_fichas[_fichaSelected].name == "ficha_all")
        {
            f.setValor(valueOfFichaAll);
        }
        // Instancia una nueva
        GameObject shadow = prevFicha.transform.GetChild(0).gameObject;
        prevFicha.name = _countNewFichas.ToString();
        // Setea de hijo fichas nuevas
        if (pl)
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
            f.setPosicion(v + offsetFicha, cl, va, pl, _countNewFichas, btnPressed, fichaIndex);
        }
        else
        {
            prevFicha.SetActive(true);
            shadow.SetActive(false);
            prevFicha.transform.position = v;
            f.setPosicion(v, cl, va, pl, _countNewFichas, btnPressed, fichaIndex);
        }
    }
}
