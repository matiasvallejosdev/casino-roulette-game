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
    
    /// <summary>
    /// Reset the number of fichas on the table
    /// </summary>
    public void resetOnTop()
    {
        _fichasOnTop = 0;
        _offsetFicha = new Vector2(0, 0);
    }
    /// <summary>
    /// Set the focus effect white in the current button
    /// </summary>
    /// <param name="parameter"></param>
    public void fx_fichasOn(bool parameter)
    {
        if(parameter){
            _spriteRender.color = new Color(255,255,255,0.58f);
        } else{
            _spriteRender.color = new Color(255,255,255,0);
        }
    }
    /// <summary>
    /// Set the focus effect white when the current button contain the numbmer winner.
    /// </summary>
    /// <param name="parameter"></param>
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
    /// <summary>
    /// Fx highlights when the button is pressed
    /// </summary>
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
        tapTouch();
        tapClick();
    }

    /// <summary>
    /// When click button roullete execute the process to assignament ficha
    /// </summary>
    private void tapClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPos2D = new Vector2(clickPos.x, clickPos.y);

            RaycastHit2D hit = Physics2D.Raycast(touchPos2D, Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.name == this.gameObject.name)
                {
                    // Find if is possible bet < totalWinner
                    if (RoundController.Instance.verficatedValueOfFicha(_scManejadorFichas.valueFicha()))
                    {
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
                    }
                    else
                    {
                        Debug.Log("Bet is not possible because the value of ficha is very high");
                        // Animacion
                        fx_higlights();
                    }
                }
            }
        }
    }

    /// <summary>
    /// When touch tap button roullete execute the process to assignament ficha
    /// </summary>
    private void tapTouch() 
    {
       if (Input.touchCount > 0) 
       {
            Touch touch = Input.GetTouch(0);

            Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            Vector2 touchPos2D = new Vector2(touchPos.x, touchPos.y);
            
            RaycastHit2D hit = Physics2D.Raycast(touchPos2D, Vector2.zero);
            if (hit.collider != null) 
            {
                if(hit.collider.gameObject.name == this.gameObject.name)
                {
                    // Find if is possible bet < totalWinner
                    if (RoundController.Instance.verficatedValueOfFicha(_scManejadorFichas.valueFicha()))
                    {
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
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Get the center or pivot of the sprite
    /// </summary>
    /// <param name="sprite"></param>
    /// <returns></returns>
    public Vector2 GetSpritePivot(Sprite sprite)
    {
        Vector2 v = _spriteRender.bounds.center;
        return v;
    }
    /// <summary>
    /// Get the offset for the current ficha.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetOffsetFicha()
    {
        Vector2 v = new Vector2(0.01f,0.038f);
        _offsetFicha = _offsetFicha + v;
        return _offsetFicha;
    }
}
