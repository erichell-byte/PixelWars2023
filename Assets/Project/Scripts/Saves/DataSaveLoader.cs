using System;
using System.IO;
using System.Runtime.InteropServices;
using Core;
using Extensions;
using UnityEngine;

namespace Saves
{
    public class DataSaveLoader
    {
        private static readonly string fileName = "savedata";
        public static SerializableDataSave SerializableData { get; private set; } = new SerializableDataSave();
        
        [DllImport("__Internal")]
        private static extern void SaveExtern(string name);

        public DataSaveLoader()
        {
            UnityEvents.ApplicationPause += Save;
            UnityEvents.ApplicationQuit += Save;
#if UNITY_WEBGL
            GameEvents.GameEndedByLose.Event += Save;
            GameEvents.GameEndedByWin.Event += Save;
#endif
        }
        
        public SerializableDataSave LoadData()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
        Application.ExternalCall("GameStarted");
#else
            if (File.Exists(GetCombinedPath()))
            {
                Load();
            }
            else
            {
                SerializableData = new SerializableDataSave();
                Save();
            }
#endif
            return SerializableData;
        }

        public void Load()
        {
            Stream stream = new FileStream(GetCombinedPath(), FileMode.Open, FileAccess.ReadWrite);
            BinaryReader reader = new BinaryReader(stream);
            
            SerializableData.Deserialize(reader);
            
            reader.Close();
            stream.Close();
        }

        public void Save()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            SerializableData.ConvertFromSerializedObjectToYandex();
            string jsonString = JsonUtility.ToJson(SerializableData.playerInfo);
            SaveExtern(jsonString);
#else
            Stream stream = new FileStream(GetCombinedPath(), FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter writer = new BinaryWriter(stream);

            SerializableData.Serialize(writer);
            
            writer.Close();
            stream.Close();
#endif
        }

        // for WEBGL yandex function
        public static void SetPlayerInfo(string value)
        {
            SerializableData.playerInfo = JsonUtility.FromJson<PlayerInfoForYandex>(value);
            SerializableData.ConvertFromYandexToSerializedObject();
        }

        private string GetCombinedPath()
        {
#if !UNITY_EDITOR
            string path = Path.Combine(Application.persistentDataPath, fileName);
#else
            string path = Path.Combine(Application.dataPath, fileName);
#endif
            return path;
        }
    }
}