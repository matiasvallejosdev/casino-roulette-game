using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    public int _seg = 0;
    public Text _win;
    public Text _number;
    public AudioSource _audioSource;
   
    public void winOrLost(string win, string number, bool isWin)
    {
        this.gameObject.SetActive(true);
        StartCoroutine(winner(_seg, win, number, isWin));
    }

    IEnumerator winner(int seg, string win, string number, bool isWin)
    {
        _win.text = win.ToString();

        if (isWin)
        {
            _number.text = number.ToString();
            _audioSource.Play();
        }
        else
        {
            _number.text = number.ToString();
        }
        yield return new WaitForSeconds(seg);
        this.gameObject.SetActive(false);
    }
}
