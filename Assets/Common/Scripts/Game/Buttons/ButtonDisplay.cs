using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDisplay : MonoBehaviour
{
    public Button button;
    public SpriteRenderer _spriteRender {get; private set; } 
    public HandlerFichas handlerFichaScript;
    [SerializeField]
    private ButtonDisplay[] parentButtonScript;

    private bool _isButtonActive;
    private int _currentFichasOnTop;
    private Vector2 _offsetButton;
    private ButtonDisplay _buttonScript;
    
    private float startTime, endTime;
    public bool isLongPressed {get; private set; }
    bool _isPressed;

    public bool HasFichasOnTop
    {
        get
        {
            bool _fichasTopBoolean = false;
            if (_currentFichasOnTop != 0)
            {
                _fichasTopBoolean = true;
            }   
            return _fichasTopBoolean;
        }
        private set{ }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeButton();
    }
    private void InitializeButton()
    {
        _spriteRender = gameObject.GetComponent<SpriteRenderer>();
        _buttonScript = gameObject.GetComponent<ButtonDisplay>();

        //parentButtonScript = InitParentButtons();

        ActivateButton(true);
        ResetButton();
    }

    /*private ButtonDisplay[] InitParentButtons()
    {
        List<ButtonDisplay> s = new List<ButtonDisplay>();
        foreach(int v in button.value) 
        {
            s.Add(GameObject.Find("n_" + v.ToString()).GetComponent<ButtonDisplay>());
        }
        return s.ToArray();
    }*/
    
    public void AddFichasOnTop(int add)
    {
        _currentFichasOnTop += add;
    }
    public int GetCurrentFichasOnTop()
    {
        return _currentFichasOnTop;
    }

    public void ActivateButton(bool isOn) 
    {
        _isButtonActive = isOn;
    }

    /// <summary>
    /// Reset the number of fichas on the table
    /// </summary>
    public void ResetButton()
    {
        _currentFichasOnTop = 0;
        _offsetButton = new Vector2(0, 0);
      
        startTime = 0;
        endTime = 0;
        _isPressed = false;
    }
    
    public void FxFicha(bool parameter)
    {
        if(parameter){
            _spriteRender.color = new Color(255,255,255,0.58f);
        } else{
            _spriteRender.color = new Color(255,255,255,0);
        }
    }
    public void FxNumberWinner(bool parameter)
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
    public void FxButtonPressed(bool isPress) 
    {
        // Activate this button
        this.FxFicha(isPress);
        // Activate the parent buttons
        foreach(ButtonDisplay button in parentButtonScript) 
        {
            button.FxFicha(isPress);
        }
    }

    void Update()
    {
        //TapTouch();
        //Click();
    }

    /// <summary>
    /// When click button roullete execute the process to assignament ficha
    /// </summary>
    private void Click()
    {
        /*if (!isPressed)
        {
            // Time for pressed
            startTime = Time.time;
            TableController.Instance.OnLongPressButton.Invoke(true, buttonScript);
            isPressed = true;
        }*/

        // Find if is possible bet < totalWinner
        if (RoundController.Instance.CheckBetValue(handlerFichaScript.GetValueFicha(handlerFichaScript.CurrentIndexSelected)))
        {
            // Ficha Nueva
            handlerFichaScript.InstantiateNewFichaInGame(GetSpritePivot(_spriteRender.sprite), GetOffsetFicha(), HasFichasOnTop, this.gameObject);
            // Sound Control
            SoundContoller.Instance.PlayFxSound(1);
            // Top controller
            _currentFichasOnTop++;
        }
        else
        {
            Debug.Log("Bet is not possible because the value of ficha is very high");
        }
        //StartCoroutine(waitToDesactivate(0.6f));
        //CheckLongClickPress();
    }


    // When the long pressed
    private void CheckLongClickPress() 
    {
        if (Input.GetMouseButtonUp(0)) 
        {
            endTime = Time.time;
        }

        if (_isPressed) 
        {
            if(endTime - startTime > 0.5f) 
            {
                TableController.Instance.OnLongPressButton.Invoke(false, _buttonScript);

                _isPressed = false;
                startTime = 0;
                endTime = 0;
                return;
            }
        }
    }
    IEnumerator waitToDesactivate(float seg)
    {
        yield return new WaitForSeconds(seg);
        if (_isPressed) 
        {
            if (endTime - startTime < 0.5f && endTime - startTime > 0)
            {
                TableController.Instance.OnLongPressButton.Invoke(false, _buttonScript);
                _isPressed = false;
                startTime = 0;
                endTime = 0;
            }
        }
    }
    
    /// <summary>
    /// When touch tap button roullete execute the process to assignament ficha
    /// </summary>
    private void TapTouch() 
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
                    if (RoundController.Instance.CheckBetValue(handlerFichaScript.GetValueFicha(handlerFichaScript.CurrentIndexSelected)))
                    {
                        // Buscar si hay objetos
                        bool _fichasTopBoolean = false;
                        if (_currentFichasOnTop != 0)
                        {
                            _fichasTopBoolean = true;
                        }
                        // Ficha Nueva
                        handlerFichaScript.InstantiateNewFichaInGame(GetSpritePivot(_spriteRender.sprite), GetOffsetFicha(), _fichasTopBoolean, this.gameObject);
                        // Sound Control
                        SoundContoller.Instance.PlayFxSound(1);
                        // Top controller
                        _currentFichasOnTop++;
                    } else
                    {
                        Debug.Log("Bet is not possible because the value of ficha is very high");
                        // Animacion
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
        _offsetButton = _offsetButton + v;
        return _offsetButton;
    }
}
