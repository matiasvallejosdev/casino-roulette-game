using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViewModel
{
    public class ChipSave
    {
        public float[] positionXY;
        public string clave;
        public int[] valor;
        public bool pleno;
        public int index;
        public int costo;
        public string btn;

        public ChipSave(float[] positionXY, string clave, int[] valor, bool pleno, int index, int costo, string btn)
        {
            this.positionXY = positionXY;
            this.clave = clave;
            this.valor = valor;
            this.pleno = pleno;
            this.index = index;
            this.costo = costo;
            this.btn = btn;
        }
    }
}
