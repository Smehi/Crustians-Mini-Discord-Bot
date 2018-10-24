using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Weebot
{
    class DataStorage
    {
        private static Dictionary<string, string> pairs = new Dictionary<string, string>();

        private const string dataFile = "DataStorage.json";

        static DataStorage()
        {
            if (!ValidateStorageFile(dataFile))
            {
                return;
            }

            string json = File.ReadAllText(dataFile);
            pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public static void SaveData()
        {
            string json = JsonConvert.SerializeObject(pairs, Formatting.Indented);
            File.WriteAllText(dataFile, json);
        }

        private static bool ValidateStorageFile(string file)
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file, "");
                SaveData();
                return false;
            }
            return true;
        }

        public static void AddPairToStorage(string key, string value)
        {
            if (pairs.ContainsKey(key))
            {
                pairs[key] = value;
            }
            else
            {
                pairs.Add(key, value);
            }

            SaveData();
        }

        public static void RemovePairFromStorage(string key)
        {
            pairs.Remove(key);
            SaveData();
        }

        public static int GetPairsCount()
        {
            return pairs.Count;
        }

        public static bool PairHasKey(string key)
        {
            if (pairs.ContainsKey(key))
            {
                return true;
            }

            return false;
        }

        public static string GetKeyValue(string key)
        {
            if (pairs.ContainsKey(key))
            {
                return pairs[key];
            }

            return null;
        }

        public static void WipeFile()
        {
            if (File.Exists(dataFile))
            {
                File.Delete(dataFile);
            }
        }
    }
}
