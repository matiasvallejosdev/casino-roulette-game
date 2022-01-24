using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModel;
using UniRx;
using System;

namespace Components
{
    public class RoulleteRotateDisplay : MonoBehaviour
    {    
        public GameRoullete gameRoullete;
        public GameObject ballRotator;
        public GameObject wheelRotator;

        void Start()
        {
            gameRoullete.currentSpeed = gameRoullete.defaultSpeed;
            gameRoullete.OnRotate
                .Subscribe(OnRotateRoullete)
                .AddTo(this);
        }

        private void OnRotateRoullete(bool isRotate)
        {
            gameRoullete.currentSpeed = gameRoullete.defaultSpeed;
        }

        void FixedUpdate()
        {
            wheelRotator.transform.Rotate(Vector3.forward * gameRoullete.currentSpeed * Time.deltaTime);
            ballRotator.transform.Rotate(Vector3.back * gameRoullete.currentSpeed * 3 * Time.deltaTime);  
        }
    }
}
