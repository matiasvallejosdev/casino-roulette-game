using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class backNumber_controller : MonoBehaviour
{
    private GameObject _numberContainer;
    private GameObject[] _numeros;
    private GameObject _contentNumeros;
    [SerializeField] private GameObject _shadowContainer;
    private GameObject[] _shadowRef;
    private int _onScreen;
    private int count;
    [SerializeField] private GameObject[] _numerosOnScreen;

    private GameObject _contentMuyBack;
    private GameObject _shadowMuyBackContainer;
    public GameObject[] _shadowMuyBackRef;
    [SerializeField] private GameObject[] _numerosOnScreenMuyBack;
    private int _onScreenMuyBack;
    public Button BTN_TEMP;
    // Start is called before the first frame update
    void Awake()
    {
        _onScreen = 0;
        _onScreenMuyBack = 0;

        count = 0;
        // Busca las referencias
        _numerosOnScreen = new GameObject[5];
        _numerosOnScreenMuyBack = new GameObject[11];
        
        _contentMuyBack = GameObject.Find("ContentNumberMuyBack");
        _contentNumeros = GameObject.Find("ContentNumber");

        _numberContainer = GameObject.Find("NumberContainer");
        _numeros = new GameObject[_numberContainer.transform.childCount];
        for(int i = 0; i < _numberContainer.transform.childCount; i++)
        {
            _numeros[i] = _numberContainer.transform.GetChild(i).gameObject;
        }

        _shadowContainer = GameObject.Find("Shadow");
        _shadowRef = new GameObject[_shadowContainer.transform.childCount];
        for(int i = 0; i < _shadowContainer.transform.childCount; i++)
        {
            _shadowRef[i] = _shadowContainer.transform.GetChild(i).gameObject;
        }
        /*
        _shadowMuyBackContainer = GameObject.Find("ShadowMuyBack");
        _shadowMuyBackRef = new GameObject[_shadowMuyBackContainer.transform.childCount];
        for(int i = 0; i < _shadowMuyBackContainer.transform.childCount; i++)
        {
            _shadowMuyBackRef[i] = _shadowMuyBackContainer.transform.GetChild(i).gameObject;
        }
        */
    }

    public void nuevoNumeroHUD(int num)
    {
        if(_onScreen < 5)
        {
            _onScreen++;
            count++;
        } else
        {
            count++;
        }
        // Crea un nuevo
        GameObject aux = Instantiate(_numeros[num]);
        aux.transform.localPosition = findPosition(_onScreen);
        aux.transform.SetParent(_contentNumeros.transform);
        aux.transform.localScale = new Vector3(22.74f,22.74f,22.74f);
        aux.name = "n_anterior_" + count.ToString();
        _numerosOnScreen[_onScreen-1] = aux;
    }
    private Vector3 findPosition(int onScreen)
    {
        if(onScreen != 0 && _contentNumeros.transform.childCount < 5)
        {
            Vector3 pos = _shadowRef[onScreen - 1].GetComponent<shadow_numeroAnterior>().GetSpritePivot();
            //Debug.Log("Posicionando numero anterior!" + pos.ToString());
            return pos;
        } else if(onScreen == 5)
        {
            muyBackNumber(_numerosOnScreen[0].gameObject);
            for(int i = 0; i < _numerosOnScreen.Length-1; i++)
            {
                GameObject siguiente = _numerosOnScreen[i+1];
                _numerosOnScreen[i] = siguiente; 
            }
            for(int e = 0; e < _numerosOnScreen.Length-1; e++)
            {
                _numerosOnScreen[e].gameObject.transform.position = _shadowRef[e].GetComponent<shadow_numeroAnterior>().GetSpritePivot();
            }
            Vector3 pos = _shadowRef[onScreen - 1].GetComponent<shadow_numeroAnterior>().GetSpritePivot();
            //Debug.Log("Posicionando numero! .. " + pos.ToString());
            return pos;
        } 
        return new Vector3(0,0,0);
    }
    private void muyBackNumber(GameObject go_new)
    {
        if(_onScreenMuyBack < 11)
        {
            _onScreenMuyBack++;
        } 
        go_new.gameObject.transform.position = findPositionMuyBack(_onScreenMuyBack);
        go_new.gameObject.transform.SetParent(_contentMuyBack.transform);
        go_new.transform.localScale = new Vector3(8.628f,8.628f,8.628f);
        _numerosOnScreenMuyBack[_onScreenMuyBack-1] = go_new;
    }
    private Vector3 findPositionMuyBack(int onScreen)
    {
        if(onScreen != 0 && _contentMuyBack.transform.childCount < 11)
        {
            Vector3 pos = _shadowMuyBackRef[onScreen - 1].GetComponent<shadow_numeroAnterior>().GetSpritePivot();
            //Debug.Log("Posicionando numero MUY BACK!" + pos.ToString());
            return pos;
        } else if(onScreen == 11)
        {
            Destroy(_numerosOnScreenMuyBack[0].gameObject);
            for(int i = 0; i < _numerosOnScreenMuyBack.Length-1; i++)
            {
                GameObject siguiente = _numerosOnScreenMuyBack[i+1];
                _numerosOnScreenMuyBack[i] = siguiente; 
            }
            for(int e = 0; e < _numerosOnScreenMuyBack.Length-1; e++)
            {
                _numerosOnScreenMuyBack[e].gameObject.transform.position = _shadowMuyBackRef[e].GetComponent<shadow_numeroAnterior>().GetSpritePivot();
            }
            Vector3 pos = _shadowMuyBackRef[onScreen - 1].GetComponent<shadow_numeroAnterior>().GetSpritePivot();
            //Debug.Log("Posicionando numero MUYBACKnew! .. " + pos.ToString());
            return pos;
        } 
        return new Vector3(0,0,0);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}