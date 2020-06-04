using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fortune : MonoBehaviour
{
    private int _randValue;
    private float _timeInterval;
    private bool _isCoroutine;
    private int _angleFinished;
    private float _totalAngle;

    public int _section;
    public int[] _payment;

    private void Start()
    {
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

        if (Mathf.RoundToInt(transform.eulerAngles.z) % _totalAngle != 0)
        {
            transform.Rotate(0, 0, 0);
        }

        _angleFinished = Mathf.RoundToInt(transform.eulerAngles.z);
        print(_angleFinished);

        for (int i = 0; i < _section; i++) 
        {
            if(_angleFinished == i * _totalAngle) 
            {
                Debug.Log("You win: $ " + _payment[i]);
            }
        }
    }
}
