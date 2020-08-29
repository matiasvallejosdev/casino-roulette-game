using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class SaveSystem
{
    public static void SavePlayer(int[] player, FichasSave[] lastRound, bool editRound)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/roullete.data";

        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerRound round = new PlayerRound(lastRound, editRound);
        System.Random r = new System.Random();

        PlayerData data = new PlayerData(r.Next(0, 1000000),player, round);

        Debug.Log("Guardando: " + data.ToString());

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer() 
    {
        string path = Application.persistentDataPath + "/roullete.data";
        FileStream stream = new FileStream(path, FileMode.Open);

        if (File.Exists(path) && stream.Length > 0) 
        {
            BinaryFormatter formatter = new BinaryFormatter();

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else 
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
