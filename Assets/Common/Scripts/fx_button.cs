using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fx_button : MonoBehaviour 
{
    #region variables
    private GameObject _go;
    public SpriteRenderer _spriteRender;

    private GameObject _goManejadorFichas;
    private manejador_fichas _scManejadorFichas;

    public int _fichasOnTop;
    private Vector2 _offsetFicha;

    // Variables posicion
    [Header("Variables Posicion")]
    public string clave;
    public int[] valor;
    public bool pleno;
    # endregion
    // Start is called before the first frame update
    void Start()
    {
        intializeVariables();
    }

    private void intializeVariables()
    {
        _goManejadorFichas = GameObject.Find("Fichas_Container");
        _scManejadorFichas = _goManejadorFichas.GetComponent<manejador_fichas>();

        _go = this.gameObject;
        _spriteRender = _go.GetComponent<SpriteRenderer>();

        resetOnTop();
    }

    public void resetOnTop()
    {
        _fichasOnTop = 0;
        _offsetFicha = new Vector2(0, 0);
    }

    public void fx_fichasOn(bool parameter)
    {
        if(parameter){
            _spriteRender.color = new Color(255,255,255,0.58f);
        } else{
            _spriteRender.color = new Color(255,255,255,0);
        }
    }

    public void fx_numberWinner(bool parameter)
    {
        if (parameter)
        {
            _spriteRender.color = new Color(233, 191, 9, 0.58f);
        }
        else
        {
            _spriteRender.color = new Color(233, 191, 9, 0);
        }
    }

    private void fx_higlights()
    {
        StartCoroutine(shadow());
    }

    IEnumerator shadow()
    {
        _spriteRender.color = new Color(255,255,255,0.15f);
        yield return new WaitForSeconds(0.06f);
        _spriteRender.color = new Color(255,255,255,0.30f);
        yield return new WaitForSeconds(0.06f);
        _spriteRender.color = new Color(255,255,255,0.45f);
        yield return new WaitForSeconds(0.06f);
        _spriteRender.color = new Color(255,255,255,0.30f);
        yield return new WaitForSeconds(0.06f);
        _spriteRender.color = new Color(255,255,255,0.15f);
        yield return new WaitForSeconds(0.05f);
        _spriteRender.color = new Color(255,255,255,0);
    }

    // Update is called once per frame
    void Update()
    {
        tap();
    }

    private void tap() 
    {
       if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null) 
            {
                if(hit.collider.gameObject.name == this.gameObject.name)
                {
                    // Find if is possible bet < totalWinner
                    if (RoundController.Instance.verficatedValueOfFicha(_scManejadorFichas.valueFicha()))
                    {
                        Debug.Log("Bet possible");
                        // Animacion
                        fx_higlights();
                        // Buscar si hay objetos
                        bool _fichasTopBoolean = false;
                        if (_fichasOnTop != 0)
                        {
                            _fichasTopBoolean = true;
                        }
                        // Ficha Nueva
                        _scManejadorFichas.nueva_ficha(GetSpritePivot(_spriteRender.sprite), this.gameObject.name, _fichasTopBoolean, GetOffsetFicha(), clave, valor, pleno, this.gameObject);
                        // Sound Control
                        SoundContoller.Instance.fx_sound(1);
                        // Top controller
                        _fichasOnTop++;
                    } else
                    {
                        Debug.Log("Bet is not possible because the value of ficha is very high");
                        // Animacion
                        fx_higlights();
                        // Sound error
                    }
                }
            }
        }
    }
    public Vector2 GetSpritePivot(Sprite sprite)
    {
        Vector2 v = _spriteRender.bounds.center;
        return v;
    }


    public Vector2 GetOffsetFicha()
    {
        Vector2 v = new Vector2(0.01f,0.038f);
        _offsetFicha = _offsetFicha + v;
        return _offsetFicha;
    }
}
