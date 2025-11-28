using System;
using System.IO;
using Core.Ordinaries;
using UnityEngine;

namespace Core.Managers
{
    public class DataManager : Singleton<DataManager>
    {
        public string DataPath => _dataPath ??= Application.persistentDataPath + Path.AltDirectorySeparatorChar;
        private string _dataPath;

        public string UserID = "User";


        private string GetFilePath(string fileName)
        {
            return DataPath + UserID + "_" + fileName + ".json";
        }

        public bool HasFile(string fileName)
        {
            var path = GetFilePath(fileName);
            return File.Exists(path);
        }


        public string ToJson<T>(T target)
        {
            var container = new JsonContainer<T>(target);
            var json = JsonUtility.ToJson(container);
            return json;
        }

        public void Save<T>(T target, string fileName)
        {
            var path = GetFilePath(fileName);
            var json = ToJson(target);
            File.WriteAllText(path, json);
        }


        public T FromJson<T>(string json)
        {
            var container = JsonUtility.FromJson<JsonContainer<T>>(json);
            var target = container.value;
            return target;
        }

        public T Load<T>(string fileName)
        {
            var path = GetFilePath(fileName);
            var json = File.ReadAllText(path);
            var target = FromJson<T>(json);
            return target;
        }

        public bool TryLoad<T>(string fileName, out T target)
        {
            if (HasFile(fileName))
            {
                target = Load<T>(fileName);
                return true;
            }

            target = default;
            return false;
        }


        [Serializable]
        private class JsonContainer<T>
        {
            public JsonContainer(T value)
            {
                this.value = value;
            }

            public T value;
        }
    }
}