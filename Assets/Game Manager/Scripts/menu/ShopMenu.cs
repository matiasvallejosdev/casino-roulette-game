using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    [SerializeField] private Button _exitBtn;
    [SerializeField] private Button _resumeBtn;

    private void Start()
    {
        _resumeBtn.onClick.AddListener(HandleResumeClick);
        _exitBtn.onClick.AddListener(HandleExitClick);
    }
    void HandleResumeClick()
    {
        game_manager.Instance.toggleShop();
    }
    void HandleExitClick()
    {
        game_manager.Instance.exitGame();
    }
}
