using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class HandlerFichas : MonoBehaviour
{
    [Header("Fichas")]
    [SerializeField] 
    private GameObject[] fichas = null;
    [SerializeField]
    private int _valueFichaAll;
    private int _currentIndexSelected = 0;
    private int _counterFichas = 0;
    private GameObject _fichasContainer;

    public int FichaAll
    {
        get{  return _valueFichaAll;  }
        set{  _valueFichaAll = value; }
    }
    public int CurrentIndexSelected
    {
        get{  return _currentIndexSelected;  }
        set{  _currentIndexSelected = value; }
    }

    void Start()
    {
        RoundController.Instance.OnGameOpened();
        _fichasContainer = this.gameObject.transform.GetChild(0).gameObject;
    }
    public int GetValueFicha(int index)
    {
        return fichas[index].GetComponent<FichaDisplay>().GetValueFicha();
    }
    public void ChangeSelectedIndexFichas(int num)
    {
        _currentIndexSelected = num;
    }
    public void InstantiateNewFichaInGame(Vector2 position, Vector2 offsetPosition, bool fichasOnTop, GameObject btnPressed)
    {
        _counterFichas++;
        // Buscar el numero de la ficha seleccionada
        GameObject newFicha = Instantiate(fichas[_currentIndexSelected]);
        newFicha.SetActive(true);

        // Inicializa los componentes
        FichaDisplay fichaScript = newFicha.GetComponent<FichaDisplay>();
        fichaScript.IsActiveInGame = true;

        newFicha.name = fichaScript.ficha.keyFicha.ToString() + "_" + _counterFichas.ToString();

        ButtonDisplay buttonPressedScript = btnPressed.GetComponent<ButtonDisplay>();

        if(fichaScript.ficha.keyFicha == KeyFicha.FichaAll)
        {
            fichaScript.SetValueFichaAll(_valueFichaAll);
        }

        if(buttonPressedScript.button.isPleno)
        {
            newFicha.transform.SetParent(_fichasContainer.transform.GetChild(0));
        } 
        else 
        {
            newFicha.transform.SetParent(_fichasContainer.transform.GetChild(1));
        }

        if (fichasOnTop)
        {
            // Set position
            newFicha.transform.position = position + offsetPosition;
            fichaScript.SetParameters(position + offsetPosition, buttonPressedScript);
        }
        else
        {
            newFicha.transform.position = position;
            fichaScript.SetParameters(position, buttonPressedScript);
        }
    }

    public void RecoverFichas(Vector2 v, bool fichasOnTop, Vector2 offsetFicha, string cl, int[] va, bool pl, GameObject btnPressed, int fichaIndex)
    {
        /*_counterFichas++;
        // Buscar el numero de la ficha seleccionada
        GameObject prevFicha = Instantiate(fichas[fichaIndex]);
        prevFicha.name = _counterFichas.ToString();

        // Inicializa los componentes
        FichaDisplay f = prevFicha.GetComponent<FichaDisplay>();
        prevFicha.name = _counterFichas.ToString();

        ButtonDisplay buttonPressedScript = btnPressed.GetComponent<ButtonDisplay>();

        // Verifica si es all
        if (fichas[_currentIndexSelected].name == "ficha_all")
        {
            //f.setValor(valueFichaAll);
        }

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
            // Create ficha and shadow
            /*prevFicha.SetActive(true);
            prevFicha.transform.position = v + offsetFicha;
            f.setPosicion(v + offsetFicha, cl, va, pl, countNewFicha, btnPressed, fichaIndex);

            sombra.transform.position = prevFicha.transform.position - new Vector3(0.02f, 0.027f, 0);
            sombra.transform.SetParent(prevFicha.transform);
        }
        else
        {
            prevFicha.SetActive(true);
            prevFicha.transform.position = v;
            //f.setPosicion(v, cl, va, pl, countNewFicha, btnPressed, fichaIndex);
        }*/
    }
}
