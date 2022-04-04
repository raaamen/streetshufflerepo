using System;
using System.Collections.Generic;

namespace SaveManagement
{
	[Serializable]
	public class Data
	{
		public SaveData[] savedObjectsArray = new SaveData[0];

		public Data(Dictionary<string, SaveData> savedDict)
		{
			savedObjectsArray = new SaveData[savedDict.Count];

			int i = 0;
			foreach (KeyValuePair<string, SaveData> item in savedDict)
			{
				savedObjectsArray[i] = item.Value;
				i++;
			}
		}
	}
}
