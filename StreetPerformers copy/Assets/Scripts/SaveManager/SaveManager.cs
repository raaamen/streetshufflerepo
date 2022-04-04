using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SaveManagement
{
	public class SaveManager : MonoBehaviour
	{

        private Dictionary<string, object> parameters = new Dictionary<string, object>();

        private static float sessionStartTime;
        private static float sessionUnpauseTime;
        private static float sessionPauseTime;

		private static Dictionary<string, SaveData> savedItems = new Dictionary<string, SaveData>();

		private static string nameOfSavingFile { get { return Application.persistentDataPath + Path.DirectorySeparatorChar + "SaveData.dat"; } } //Name of the text file that will be retrived
		private static bool canWrite; //bool that determines if the savemanager can write to the file 
		private static bool hasBeenInstantiated = false;
		private static bool hasFileBeenRead = false;

		//If it's a Development build wipes the save, reads from a file, and cancels any active norification
		private void Awake()
		{
			if (hasBeenInstantiated)
			{
                Destroy(gameObject);
            }

            DontDestroyOnLoad(this.gameObject);
			hasBeenInstantiated = true;

			ReadFromFile();

			if (GetData("SaveDataRefresh") == null)
			{
				WipeFile();
				SetData("SaveDataRefresh", 1f);
			}
			ReadFromFile();

			canWrite = true;

            sessionStartTime = Time.realtimeSinceStartup;
            sessionUnpauseTime = Time.realtimeSinceStartup;
            sessionPauseTime = Time.realtimeSinceStartup;
        }

		//Writes to the file when the application pauses
		private void OnApplicationPause()
		{
			//WriteToFile();
		}
		public static void SaveToFile() {
			WriteToFile();
		}
		private void OnDestroy()
		{
			//WriteToFile();
		}

        private void OnApplicationQuit()
        {
            //Potentially necessary to put players back into a battle they quit from.
            //parameters.Add("Battles fought", );
            parameters.Clear();
            parameters.Add("Session length (total)", Time.realtimeSinceStartup - sessionStartTime);
            //Add player ID, run ID, session ID
            //sessionEndEvent.Invoke(parameters);
        }

        #region Variable Retrieval

        public static object GetData(string key)
		{
			ReadFromFile();

			if (savedItems.ContainsKey(key))
				return savedItems[key].Value;
			else
				return null;
		}

		public static void SetData(string key, object value)
		{
			ReadFromFile();

			if (savedItems.ContainsKey(key))
				savedItems[key].Value = value;
			else
				savedItems.Add(key, new SaveData(key, value));
		}

		public static void RemoveData(string key)
		{
			ReadFromFile();

			if (savedItems.ContainsKey(key))
				savedItems.Remove(key);

			return;
		}

		#endregion

		#region File Manipulation

		private static void DataToDictionaries(Data data)
		{
			foreach (SaveData item in data.savedObjectsArray)
			{
				savedItems.Add(item.Key, item);
			}
		}

		public static void DisableWriting()
		{
			canWrite = false;
		}

		public static void EnableWriting() {
			canWrite = true;
		}

		private static void ReadFromFile()
		{
			if (hasFileBeenRead)
				return;

			BinaryFormatter formatter = new BinaryFormatter();
			FileStream fileStream = File.Open(nameOfSavingFile, FileMode.OpenOrCreate);

			if (fileStream.Length > 0)
			{
				Data savedData = (Data)formatter.Deserialize(fileStream);

				DataToDictionaries(savedData);
			}

			fileStream.Close();
			hasFileBeenRead = true;
		}

		private static void WriteToFile()
		{
			if (canWrite)
			{
				Debug.Log("Writing to file.");
				BinaryFormatter formatter = new BinaryFormatter();
				FileStream fileStream = File.Open(nameOfSavingFile, FileMode.OpenOrCreate);

				formatter.Serialize(fileStream, new Data(savedItems));

				fileStream.Close();

			}
			else
			{
				Debug.Log("Writing disabled");
			}
		}

		public static void WipeFile()
		{
			Debug.Log("Wiping file.");

			savedItems.Clear();

			File.Delete(nameOfSavingFile);
			hasFileBeenRead = false;
		}

		#endregion
	}
}
