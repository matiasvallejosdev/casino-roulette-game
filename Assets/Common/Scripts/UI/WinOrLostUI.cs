using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WinOrLostUI : MonoBehaviour
{
    public int _seg = 0;
    [SerializeField] private Text _win;
    [SerializeField] private Text _number;

    public void winOrLost(string win, string number, bool isWin)
    {
        this.gameObject.SetActive(true);

        StartCoroutine(winner(_seg, win, number, isWin));
    }

    IEnumerator winner(int seg, string win, string number, bool isWin)
    {
        _win.text = win.ToString();

        Color green = new Color(173f, 255f, 131f, 1f); 
        Color red = new Color(255f, 131f, 132f, 1f);

        if (isWin)
        {
            _number.text = number.ToString();
            _number.color = green;
        }
        else
        {
            _number.text =  number.ToString();
            _number.color = red;

        }
        yield return new WaitForSeconds(_seg);
        SoundContoller.Instance.fx_sound(6);
        this.gameObject.SetActive(false);
    }
}
