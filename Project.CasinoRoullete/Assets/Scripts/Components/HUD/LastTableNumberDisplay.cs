using System.Collections;
using System.Collections.Generic;
using Commands;
using UnityEngine;
using UnityEngine.UIElements;
using ViewModel;
using UniRx;
using System;

namespace Components
{
    public class LastTableNumberDisplay : MonoBehaviour
    {
        public CharacterTable characterTable;
        public GameRoullete gameRoullete;

        public GameObject numberContainer;
        public GameObject[] anchorNumbers;

        public Transform _container;
        public int _biggerPosition;
        public int _smallerPosition;
        public Vector3 _smallerScale;
        public Vector3 _biggerScale;

        private int _onScreen;

        void Start()
        {
            gameRoullete.OnNumber
                .Subscribe(LastNumberDisplay)
                .AddTo(this);

            Reset();
        }

        private void LastNumberDisplay(int value)
        {       
            if(_onScreen >= (_smallerPosition + _biggerPosition))
            {
                Reset();
            } 
            else 
            {
                RepositionLastNumbers();
            }

            AddNumber(value);   
            _onScreen++;
        }

        private void Reset()
        {
            _onScreen = 0;
            characterTable.currentNumbers.Clear();
            foreach(Transform go in _container.transform.GetComponentInChildren<Transform>())
            {
                Destroy(go.gameObject);
            }
        }

        private void RepositionLastNumbers()
        {
            if(_onScreen == 0)
                return;
            
            LastNumber[] numbersOnScreen = _container.GetComponentsInChildren<LastNumber>();
            foreach(LastNumber t in numbersOnScreen)
            {
                t.currentPosition++;
                t.gameObject.transform.position = FindNumberPosition(t.currentPosition);
                t.transform.localScale = t.currentPosition >= _biggerPosition ? _smallerScale : _biggerScale;
            }
        }

        public void AddNumber(int value)
        {
            GameObject num = Instantiate(numberContainer.transform.GetChild(value).gameObject);
            num.transform.localPosition = FindNumberPosition(0);
            num.GetComponent<LeanTweenScale>()._scaleXYZ = _biggerScale;
            LastNumber lastNumber= num.AddComponent<LastNumber>();
            lastNumber.currentPosition = 0;
            num.transform.SetParent(_container.transform);
            num.name = "last_" + _onScreen.ToString();
        }

        private Vector3 FindNumberPosition(int onScreen)
        {
            Vector3 pos = anchorNumbers[onScreen].GetComponent<SpriteRenderer>().bounds.center;
            return pos;
        }
    }
}