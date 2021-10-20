using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Text;

[Serializable]
public class Save {
    static string path = Application.persistentDataPath + "/frog";

    public string name;
    public float[] color;

    public static Save save;

    public Save(string name, float[] color) {
      this.name = name;
      this.color = color;
    }

    public static void SaveGame(){
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, save);
        stream.Close();
    }

    public static void Load(){
        if(File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            try {
              save = formatter.Deserialize(stream) as Save;
            } catch(Exception e) {
              save = new Save("", new float[]{0, 0, 0});
            }
            stream.Close();
        }
        if (save == null) {
          save = new Save("", new float[]{0, 0, 0});
        }
    }
}
