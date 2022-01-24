using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViewModel;
using UniRx;
using Commands;

namespace Components
{
    public class FortuneRewardDisplay : MonoBehaviour
    {
        public RewardFortune rewardFortune;
        public GameCmdFactory gameCmdFactory;
        
        private int _randValue;
        private float _timeInterval;
        private float _angleFinished;
        private float _totalAngle;

        private void Start()
        {
            rewardFortune.isPayment = false;
            rewardFortune.isPlay = false;

            _totalAngle = 360 / rewardFortune.sectionCount;

            rewardFortune.OnFortune
                .Subscribe(OnFortune)
                .AddTo(this);
        }
            
        public void OnFortune(bool isFortune)
        {
            if(isFortune)
                StartCoroutine(Spin());
        }
        
        IEnumerator Spin()
        {
            rewardFortune.isPlay = true;
            rewardFortune.isPayment = true;

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
                transform.Rotate(0, 0, 22.5f);
            }
            
            rewardFortune.isPlay = false;
        }
    }
}
