using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModel;
using UniRx;

namespace Components
{
    public class MagnetDestroyerDisplay : MonoBehaviour
    {
        public CharacterTable characterTable;
        public GameObject magnetDestroyer;
        public float magnetTime;

        void Start()
        {
            characterTable.OnRoundFinished
                .Subscribe(MagnetDestroyerFichas)
                .AddTo(this);
            
            //characterTable.OnRound.OnNext(true);
        }

        public void MagnetDestroyerFichas(bool value)
        {
            StartCoroutine(ActivateMagnetDestroyer(magnetTime));
        }
        
        IEnumerator ActivateMagnetDestroyer(float seg)
        {
            characterTable.OnActiveButton.OnNext(false);
            magnetDestroyer.SetActive(true);
            yield return new WaitForSeconds(seg);
            magnetDestroyer.SetActive(false);
            characterTable.OnActiveButton.OnNext(true);        
        }
    }
}
