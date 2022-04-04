using System;
using SaveManagement;
using UnityEngine;

[System.Serializable]
public class SaveDataGeneric<T>
{
	public T Value
	{
		get
		{
			try
			{
				T v = (T)SaveManager.GetData(name);
				return v;
			}
			catch (Exception e)
			{
				string typeName = SaveManager.GetData(name).GetType().ToString();
				throw new Exception("Error converting saved value to the correct saved data " + name + " is saved as a type " + typeName +
					". This can be caused by data be saved under the same name or a variable changing what variable is saved there.\n" + e.StackTrace);
			}
		}
		set
		{
			SaveManager.SetData(name, value);
		}

	} //Value of the class
	public string name; //Name of the data

	/// <summary>
	/// <para>Basic Declaration, sets the name to Unnamed variable.</para>
	/// </summary>
	public SaveDataGeneric(string _name)
	{
		name = _name;
	}

	public SaveDataGeneric(string _name, T _value)
	{
		name = _name;
		if (SaveManager.GetData(name) == null)
			Value = _value;
	}
}
