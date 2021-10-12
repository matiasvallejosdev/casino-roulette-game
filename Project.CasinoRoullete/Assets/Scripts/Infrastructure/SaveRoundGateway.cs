using System;
using UniRx;
using UnityEngine;
using UnityEditor;
using ViewModel;
using System.Collections;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Infrastructure
{
    public class GlobalGateway : ISaveRound
    {
        private protected string URL_PATH = Application.persistentDataPath + "/roullete.data";
        public Round roundData {get; set;}

        public IObservable<Unit> RoundSequentialSave()
        {
            return Observable.FromCoroutine<Unit>(observer => SavePlayer(observer))
                                            .Do(_ => Debug.Log("Save round data in " + URL_PATH));
        }

        public IObservable<Unit> RoundSequentialLoad()
        {
            return Observable.FromCoroutine<Unit>(observer => LoadPlayer(observer))
                                            .Do(_ => Debug.Log("Load round data in " + URL_PATH));
        }

        IEnumerator SavePlayer(IObserver<Unit> observer)//int[] player, FichasSave[] fichas, bool editRound)
        {
            string path = URL_PATH;
            yield return new WaitForSeconds(0.01f);

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            //PlayerData data = new PlayerData(r.Next(0, 1000), fichas, player[0], editRound);

            Debug.Log("Saving data in: " + roundData.ToString());

            formatter.Serialize(stream, roundData);
            stream.Close();

            observer.OnNext(Unit.Default); // push Unit or all buffer result.
            observer.OnCompleted();
        }

        IEnumerator LoadPlayer(IObserver<Unit> observer) 
        {
            string path = URL_PATH;
            yield return new WaitForSeconds(0.01f);

            FileStream stream = new FileStream(path, FileMode.Open);

            if (File.Exists(path) && stream.Length > 0) 
            {
                BinaryFormatter formatter = new BinaryFormatter();

                roundData = formatter.Deserialize(stream) as Round;
                stream.Close();
            }
            else 
            {
                Debug.LogError("Save file not found in " + path);
            }
            
            observer.OnNext(Unit.Default); // push Unit or all buffer result.
            observer.OnCompleted();
        }
    }
}

