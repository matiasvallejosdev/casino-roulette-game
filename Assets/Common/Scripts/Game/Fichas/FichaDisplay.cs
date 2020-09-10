using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FichaDisplay : MonoBehaviour
{
    public Ficha ficha;
    public ButtonDisplay currentButton;

    private Vector2 _currentPosition { get; set; }
    private SpriteRenderer _spriteRenderer;
    public int valueFichaAll{get; private set;}

    private bool _isActiveInGame = false;
    public bool IsActiveInGame
    {
        get{ return _isActiveInGame; }
        set{ _isActiveInGame = value; }
    }
    void Start() 
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = ficha.imageFicha;
    }

    public void SetParameters(Vector2 position, ButtonDisplay buttonPressed)
    {
        this._currentPosition = position;
        currentButton = buttonPressed;
    }
    public Vector2 GetPosition()
    {
        return _currentPosition;
    }
    public int GetValueFicha()
    {
        return ficha.valueFicha;
    }
    public string GetKeyFicha()
    {
        return ficha.keyFicha.ToString();
    }
    public void SetValueFichaAll(int value) 
    {
        valueFichaAll = value;
    }
    public int GetIndexFicha() 
    {
        return ficha.indexArrayFicha;
    }
}
