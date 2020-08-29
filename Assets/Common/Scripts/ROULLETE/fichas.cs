using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fichas : MonoBehaviour
{
    public Vector2 pos;
    public int _valor;
    public string _key;
    public Dictionary<string,int[]> _posicion = new Dictionary<string,int[]>();
    public bool _pleno;
    public GameObject button;
    public string _nameOfThisFicha;
    public int _fichaSelected;

    /// <summary>
    /// Set the ficha parameters in the game. The vectors & values in the game.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="v"></param>
    /// <param name="pleno"></param>
    /// <param name="num"></param>
    /// <param name="btnPressed"></param>
    /// <param name="fichaIndex"></param>
    public void setPosicion(Vector2 pos, string p, int[] v, bool pleno, int num, GameObject btnPressed, int fichaIndex)
    {
        this.pos = pos;
        button = btnPressed;
        _key = p;
        _pleno = pleno;
        _posicion.Add(p,v);
        _fichaSelected = fichaIndex;
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
    public void setValor(int value) 
    {
        _valor = value;
    }
    public int GetIndex() 
    {
        return _fichaSelected;
    }
}
