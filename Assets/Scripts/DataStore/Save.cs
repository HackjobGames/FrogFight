using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Save {
    static string path = Application.persistentDataPath + "/frog";
    public static string name;

    public static void SaveName(){
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, name);
        stream.Close();
    }
    public static void Load(){
        if(File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            name = formatter.Deserialize(stream) as string;
            Debug.Log(name);
            stream.Close();
        }
    }
}
