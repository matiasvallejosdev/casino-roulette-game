using System;
using UniRx;
using UnityEngine;
using UnityEditor;
using ViewModel;
using System.Collections;
using Managers;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Infrastructure
{
    public class SaveRoundGateway : ISaveRound
    {
        private static protected readonly string FILE_NAME = "roulletedata";
        public Round roundData {get; set;}

        public IObservable<Unit> RoundSequentialSave(CharacterTable characterTable)
        {
            return Observable.FromCoroutine<Unit>(observer => SavePlayer(observer, characterTable));
        }

        public IObservable<Unit> RoundSequentialLoad(CharacterTable characterTable)
        {
            return Observable.FromCoroutine<Unit>(observer => LoadPlayer(observer));
        }

        IEnumerator SavePlayer(IObserver<Unit> observer, CharacterTable characterTable)
        {
            roundData = new Round()
            {
                idPlayer = characterTable.tableName,
                playerMoney = characterTable.characterMoney.characterMoney.Value
            };

            string path = GameManager.Instance.UrlDataPath + FILE_NAME;
            string json = JsonUtility.ToJson(roundData);

            File.WriteAllText(path, json);
            Debug.Log($"Saved data JSON with the table {roundData.idPlayer} with {json}");

            yield return new WaitUntil(() => File.Exists(path));
            
            observer.OnNext(Unit.Default); // push Unit or all buffer result.
            observer.OnCompleted();
        }

        IEnumerator LoadPlayer(IObserver<Unit> observer) 
        {
            string path = GameManager.Instance.UrlDataPath + FILE_NAME;
            string saveString = File.ReadAllText(path);

            yield return new WaitUntil(() => saveString != null);
            
            roundData = JsonUtility.FromJson<Round>(saveString);
            Debug.Log($"Loaded data JSON with the table {roundData.idPlayer} with {saveString}");

            observer.OnNext(Unit.Default); // push Unit or all buffer result.
            observer.OnCompleted();
        }
    }
}

