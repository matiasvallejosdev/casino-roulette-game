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
        private int _delay = 3;

        void Start()
        {
            gameRoullete.OnNumber
                .Subscribe(FxNewNumber)
                .AddTo(this);
        }

        private void FxNewNumber(int num)
        {
            StartCoroutine(FxNumber(_secondToDisplay, num));
        }

        IEnumerator FxNumber(int seg, int num)
        {
            yield return new WaitForSeconds(_delay);
            goContainer.SetActive(true);
            GameObject goNum = Instantiate(numberContainer.transform.GetChild(num).gameObject);
            if(num == 0)
            {
                goNum.transform.GetChild(0).transform.localPosition = new Vector3(-0.08f, 0.19f, 0);
            }
            goNum.transform.localPosition = anchorPos.transform.position;
            goNum.GetComponent<LeanTweenScale>()._scaleXYZ = new Vector3(0.87f,0.87f,0.87f);
            goNum.transform.SetParent(anchorPos.transform);
            goNum.SetActive(true);
            yield return new WaitForSeconds(seg);
            Destroy(goNum);
            goContainer.SetActive(false);
        }
    }
}
