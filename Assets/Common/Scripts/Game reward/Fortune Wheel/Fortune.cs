using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fortune : MonoBehaviour
{
    private int _randValue;
    private float _timeInterval;
    private bool _isCoroutine;
    private float _angleFinished;
    private float _totalAngle;

    public int _section;
    public int[] _payment;

    public MoneySystemController moneySystem;

    private AudioSource _fortuneAudio;

    private void Start()
    {
        _fortuneAudio = GetComponent<AudioSource>();

        _isCoroutine = false;
        _totalAngle = 360 / _section;
    }

    public void StartingFortune()
    {
        if(_isCoroutine == false) 
        {
            StartCoroutine(Spin());
        }
    }

    IEnumerator Spin()
    {
        _isCoroutine = true;
        _randValue = Random.Range(100, 200);

        _timeInterval = 0.0001f * Time.deltaTime * 2;
        _fortuneAudio.Play();
        for (int i = 0; i < _randValue; i++)
        {
            transform.Rotate(0, 0, (_totalAngle / 2));

            if (i > Mathf.RoundToInt(_randValue * 0.2f))
                _timeInterval = 0.5f * Time.deltaTime;

            if (i > Mathf.RoundToInt(_randValue * 0.5f))
                _timeInterval = 1f * Time.deltaTime;

            if (i > Mathf.RoundToInt(_randValue * 0.7f))
                _timeInterval = 1.5f * Time.deltaTime;

            if (i > Mathf.RoundToInt(_randValue * 0.8f))
                _timeInterval = 2f * Time.deltaTime;

            yield return new WaitForSeconds(_timeInterval);
        }
        _fortuneAudio.Stop();

        if (Mathf.RoundToInt(transform.eulerAngles.z) % _totalAngle != 0)
        {
            transform.Rotate(0, 0, 22.5f);
        }

        _angleFinished = (float)Mathf.RoundToInt(transform.eulerAngles.z);

        costAngleError();

        if(_angleFinished == 360) 
        {
            Debug.Log("You win: $ " + _payment[_payment.Length - 1]);
            Ui.Instance.turnWinOrLost("Excelent!", _payment[_payment.Length - 1].ToString(), true, _payment[_payment.Length - 1]);
            moneySystem._cashNew = _payment[_payment.Length - 1];
            moneySystem.SavePlayerCash();
        } else 
        {
            for (int i = 0; i < _section; i++)
            {
                if (_angleFinished == i * _totalAngle)
                {
                    Debug.Log("You win: $ " + _payment[i]);
                    Ui.Instance.turnWinOrLost("Excelent!", _payment[i].ToString(), true, _payment[i]);
                    moneySystem._cashNew = _payment[i];
                    moneySystem.SavePlayerCash();
                }
            }
        }
        _isCoroutine = false;
    }
    private void costAngleError() 
    {
        if (_angleFinished > -10 && _angleFinished <= 8.5f)
        {
            _angleFinished = 0;
            return;
        }
        if (_angleFinished > 8.5f && _angleFinished <= 28)
        {
            _angleFinished = 20;
            return;
        }
        if (_angleFinished > 28 && _angleFinished <= 48)
        {
            _angleFinished = 40;
            return;
        }
        if (_angleFinished > 48 && _angleFinished <= 68)
        {
            _angleFinished = 60;
            return;
        }
        if (_angleFinished > 68 && _angleFinished <= 87)
        {
            _angleFinished = 80;
            return;
        }
        if (_angleFinished > 87 && _angleFinished <= 107)
        {
            _angleFinished = 100;
            return;
        }
        if (_angleFinished > 107 && _angleFinished <= 127)
        {
            _angleFinished = 120;
            return;
        }
        if (_angleFinished > 127 && _angleFinished <= 145)
        {
            _angleFinished = 140;
            return;
        }
        if (_angleFinished > 145 && _angleFinished <= 165)
        {
            _angleFinished = 160;
            return;
        }
        if (_angleFinished > 165 && _angleFinished <= 185)
        {
            _angleFinished = 180;
            return;
        }
        if (_angleFinished > 185 && _angleFinished <= 205)
        {
            _angleFinished = 200;
            return;
        }
        if (_angleFinished > 205 && _angleFinished <= 232)
        {
            _angleFinished = 220;
            return;
        }
        if (_angleFinished > 232 && _angleFinished <= 250)
        {
            _angleFinished = 240;
            return;
        }
        if (_angleFinished > 250 && _angleFinished <= 267)
        {
            _angleFinished = 260;
            return;
        }
        if (_angleFinished > 267 && _angleFinished <= 289)
        {
            _angleFinished = 280;
            return;
        }
        if (_angleFinished > 289 && _angleFinished <= 310)
        {
            _angleFinished = 300;
            return;
        }
        if (_angleFinished > 306 && _angleFinished <= 326)
        {
            _angleFinished = 320;
            return;
        }
        if (_angleFinished > 326 && _angleFinished <= 347)
        {
            _angleFinished = 340;
            return;
        }
        if (_angleFinished > 347 && _angleFinished <= 400)
        {
            _angleFinished = 360;
            return;
        }
    }
}
