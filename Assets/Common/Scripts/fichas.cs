using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fichas : MonoBehaviour
{
    public int _valor;
    public string _key;
    public Dictionary<string,int[]> _posicion = new Dictionary<string,int[]>();
    public bool _pleno;
    public GameObject button;

    public void setPosicion(string p, int[] v, bool pleno, int num, GameObject btnPressed)
    {
        button = btnPressed;
        _key = p;
        _pleno = pleno;
        _posicion.Add(p,v);
        //Debug.Log("Pleno: " + _pleno.ToString() + " // Valor: " + _valor.ToString() + " // Posicion: " + p + " // Numero: " + num);
    }
    public int[] getValueOfDictionary(string key)
    {
        int[] aux = _posicion[key];
        return aux;
    }
    public string getPosicion()
    {
        return _posicion.Values.ToString();
    }
    public int getValueOfBet()
    {
        return _valor;
    }
    public string getKeyOfDictionary()
    {
        string a = _key;
        return a;
    }
    public int countTheNumberOfTheDictionary(string key)
    {
        int aux = 0;
        foreach(var element in _posicion[key])
        {
            aux = aux + 1;
        }
        return aux;
    }
}
