namespace SaveManagement
{
	[System.Serializable]
	public class SaveData
	{
		public string Key = "";
		public object Value = null;

		public SaveData(string _key, object _value)
		{
			Key = _key;
			Value = _value;
		}
	}
}
