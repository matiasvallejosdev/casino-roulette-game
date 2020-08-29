using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class FichasSave
{
    public Vector2 p;
    public string clave;
    public int[] valor;
    public bool pleno;
    public int index;

    public FichasSave(Vector2 p, string clave, int[] valor, bool pleno, int index)
    {
        this.p = p;
        this.clave = clave;
        this.valor = valor;
        this.pleno = pleno;
        this.index = index;
    }
}
