using System.Collections;
using UnityEngine;
using ViewModel;
using UniRx;
using System;
using TMPro;

namespace Components
{
    public class NewNumberRoulleteDisplay : MonoBehaviour
    {
        public GameRoullete gameRoullete;
        public GameObject numberContainer;
        public GameObject goContainer;
        public GameObject anchorPos;
        public SpriteRenderer anchorSprite;
        public int _secondToDisplay;

        private bool _preventStart = true;
        private int _delay = 4;

        void Start()
        {
            gameRoullete.currentNumber
                .Subscribe(FxNewNumber)
                .AddTo(this);
        }

        private void FxNewNumber(int num)
        {
            if(_preventStart)
            {
                _preventStart = false;
                return;
            }

            StartCoroutine(FxNumber(_secondToDisplay, num));
        }

        IEnumerator FxNumber(int seg, int num)
        {
            yield return new WaitForSeconds(_delay);
            goContainer.SetActive(true);
            GameObject goNum = Instantiate(numberContainer.transform.GetChild(num).gameObject);
            goNum.SetActive(true);
            goNum.transform.position = anchorPos.transform.position;
            goNum.transform.SetParent(anchorPos.transform);
            goNum.GetComponent<LeanTweenScale>()._scaleXYZ = new Vector3(0.9f,0.9f,0.9f);
            yield return new WaitForSeconds(seg);
            Destroy(goNum);
            goContainer.SetActive(false);
        }
    }
}
