using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelExitUI : MonoBehaviour
{
    public UnityEngine.UI.Button _btnExit;
    public UnityEngine.UI.Button _btnDiscard;

    void Start()
    {
        _btnExit.onClick.AddListener(HandlerExitClick);
        _btnDiscard.onClick.AddListener(HandlerDiscardClick);
    }
    void HandlerExitClick() 
    {
        //GameManager.Instance.exitGame();
    }
    void HandlerDiscardClick() 
    {
        MenuUi.Instance.btnVolver.gameObject.SetActive(true);
        MenuUi.Instance.HandlerReturnClick();
    }
}
