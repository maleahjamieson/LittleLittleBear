using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveData : MonoBehaviour
{
    public static void SavePlayer(LLB llb, Inventory inv)  //invoke for saving player
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.stf";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(llb, inv);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static PlayerData LoadPlayer()  // invoke for loading player
    {
        string path = Application.persistentDataPath + "/save.stf";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = (PlayerData)formatter.Deserialize(stream);
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Shit somethings fucked in the saveData " + path);
            return null;
        }

    }
}
