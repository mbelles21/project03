using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if(File.Exists(fullPath)) {
            try {
                // load serialized data from file
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullPath, FileMode.Open)) {
                    using(StreamReader reader = new StreamReader(stream)) {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // deserialize json back into obj
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch(Exception e) {
                Debug.LogError("Error occurred trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        
        return loadedData;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try {
            // create directory path
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // serialize game data obj into Json
            string dataToStore = JsonUtility.ToJson(data, true);

            // write data to file
            using(FileStream stream = new FileStream(fullPath, FileMode.Create)) {
                using(StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(dataToStore);
                }
            }
        }
        catch(Exception e) {
            Debug.LogError("Error occurred trying to save data to file: " + fullPath + "\n" + e);
        }
    }
}
