#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnuGames.MVVM;

namespace UnuGames
{
	public class CodeGenerationHelper
	{

		#region TAGS and REGIONS

		const string NAME_SPACES_TAG = "<#NAME_SPACES#>";
		const string NAME_TAG = "<#CNAME#>";
		const string TYPE_TAG = "<#CTYPE#>";
		const string PROPERTIES_TAG = "<#PROPERTIES#>";

		const string PROPERTIES_REGION = "#region Properties";
		const string END_REGION = "#endregion";

		#endregion

		const string TYPE_PATH = "TypeTemplate";
		const string VIEW_MODEL_HANDLER_PATH = "ViewModelHandlerTemplate";

		static string Getpath (string fileName)
		{
			string[] assets = AssetDatabase.FindAssets (fileName);
			if (assets != null && assets.Length > 0)
				return AssetDatabase.GUIDToAssetPath (assets [0]);
			return "";
		}

		static public string GenerateScript (string modelName, string baseType, params CustomPropertyInfo[] properties)
		{

			string code = "";

			TextAsset text = AssetDatabase.LoadAssetAtPath<TextAsset> (Getpath (TYPE_PATH));
			if (text != null) {
				code = text.text;
				code = Regex.Replace (code, NAME_TAG, modelName);
				code = Regex.Replace (code, TYPE_TAG, baseType);
				code = Regex.Replace (code, PROPERTIES_TAG, GeneratePropertiesBlock (properties));
				code = Regex.Replace (code, NAME_SPACES_TAG, GenerateNameSpaceBlock (properties));
			} else {
				UnuLogger.LogError ("There are something wrong, could not find code template!");
			}

			return code;
		}

		static public string GenerateViewModelHandler (string modelName, string viewModelType)
		{
			string code = "";
			TextAsset text = AssetDatabase.LoadAssetAtPath<TextAsset> (Getpath (VIEW_MODEL_HANDLER_PATH));
			if (text != null) {
				code = text.text;
				code = Regex.Replace (code, NAME_TAG, modelName);
				code = Regex.Replace (code, TYPE_TAG, viewModelType);
			} else {
				UnuLogger.LogError ("There are something wrong, could not find code template!");
			}

			return code;
		}

		static public string AddProperty (Type type, params CustomPropertyInfo[] properties)
		{
			string code = GetScriptByType (type);
			if (!string.IsNullOrEmpty (code)) {
				string propertiesCode = GetCodeRegion (code, PROPERTIES_REGION);
				string newPropertiesCode = GeneratePropertiesBlock (properties);
				if (propertiesCode != null) {
					newPropertiesCode = propertiesCode + newPropertiesCode;
				}

				code = AddCodeBlock (code, propertiesCode, newPropertiesCode, PROPERTIES_REGION);
			}

			string[] nameSpaces = Regex.Split (GenerateNameSpaceBlock (properties), NewLine ());
			foreach (string nameSpace in nameSpaces) {
				if (!code.Contains (nameSpace))
					code = nameSpace + NewLine () + code;
			}

			return code;
		}

		static public string AddCodeBlock (string code, string oldBlock, string newBlock, string region)
		{
			string newCode = "";
			if (string.IsNullOrEmpty (oldBlock)) {
				oldBlock = code.Substring (0, code.LastIndexOf ("}", StringComparison.OrdinalIgnoreCase));
				newCode = oldBlock + NewLine () + newBlock + NewLine () + "}";
			} else {
				string beforeBlock = code.Substring (0, code.IndexOf (oldBlock, StringComparison.OrdinalIgnoreCase));
				string afterBlock = code.Substring (code.IndexOf (oldBlock, StringComparison.OrdinalIgnoreCase) + oldBlock.Length, code.Length - oldBlock.Length - beforeBlock.Length);
				if (!string.IsNullOrEmpty (region))
					newBlock = region + NewLine () + newBlock + NewLine () + END_REGION;
				newCode = beforeBlock + NewLine () + newBlock + NewLine () + afterBlock;
			}

			return newCode;
		}

		static public string GetScriptByType (Type type)
		{
			string scriptPath = GetScriptPathByType (type);
			TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset> (scriptPath);
			if (textAsset != null) {
				return textAsset.text;
			}

			return string.Empty;
		}

		static public string GetScriptPathByType (Type type)
		{
			string typeName = type.Name;
			string[] assets = AssetDatabase.FindAssets (typeName);
			string scriptPath = "";

			bool isViewModel = false;
			if (type == typeof(UIManBase)) {
				isViewModel = true;
			}

			foreach (string asset in assets) {
				scriptPath = AssetDatabase.GUIDToAssetPath (asset);
				if (scriptPath.EndsWith (".cs", StringComparison.OrdinalIgnoreCase)) {
					if (isViewModel) {
						UIManBase viewModelAsset = AssetDatabase.LoadAssetAtPath<UIManBase> (scriptPath);
						if (viewModelAsset != null) {
							break;
						}
					} else {
						TextAsset script = AssetDatabase.LoadAssetAtPath<TextAsset> (scriptPath);
						if (script != null) {
							break;
						}
					}
				}
			}

			TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset> (scriptPath);
			if (textAsset != null) {
				return scriptPath;
			}

			return string.Empty;
		}

		static public string GenerateNameSpaceBlock (params CustomPropertyInfo[] properties)
		{

			if (properties == null)
				return string.Empty;

			List<string> nameSpaces = new List<string> ();
			foreach (CustomPropertyInfo cpi in properties) {
				string nameSpace = cpi.GetNamespace ();
				if (!string.IsNullOrEmpty (nameSpace) && !nameSpaces.Contains (nameSpace)) {
					if (nameSpace != "System" && !cpi.PropertyType.IsAllias ())
						nameSpaces.Add (nameSpace);
				}
			}

			string code = "";

			foreach (string nameSpace in nameSpaces) {
				code += string.Format ("using {0};", nameSpace) + NewLine ();
			}

			return code;
		}

		static public string GeneratePropertiesBlock (params CustomPropertyInfo[] properties)
		{
			if (properties == null)
				return string.Empty;

			string propertiesCode = "";
			if (properties != null) {
				foreach (CustomPropertyInfo cpi in properties) {
					propertiesCode += cpi.ToString () + NewLine ();
				}
			}

			return propertiesCode;
		}

		static public string GetCodeRegion (string code, string region)
		{
			string propertiesCode = "";
			int propertiesRegionIndex = code.IndexOf (region, StringComparison.OrdinalIgnoreCase);
			if (propertiesRegionIndex != -1) {
				code = code.Substring (propertiesRegionIndex + region.Length, code.Length - propertiesRegionIndex - region.Length);
				int endRegionIndex = code.IndexOf (END_REGION, StringComparison.OrdinalIgnoreCase);
				if (endRegionIndex != -1) {
					code = code.Substring (0, endRegionIndex);
					propertiesCode = code;
				}
			}

			return propertiesCode;
		}

		static public string NormalizeFieldName (string name)
		{
			if (name.Length > 1)
				return name [0].ToString ().ToLower () + name.Substring (1, name.Length - 1);
			return name.ToLower ();
		}

		static public string NormalizePropertyName (string name)
		{
			if (name.Length > 1)
				return name [0].ToString ().ToUpper () + name.Substring (1, name.Length - 1);
			return name.ToUpper ();
		}

		static string NewLine ()
		{
			return Environment.NewLine;
		}

		static public string GeneratPathWithSubfix (string path, string subfix)
		{
			if (!string.IsNullOrEmpty (subfix)) {
				path = path.Substring (0, path.Length - 3) + subfix;
			}

			return path;
		}

		static public string DeleteScript (string path)
		{
			string code = "";

			if (!File.Exists (path))
				return null;

			try {
				code = File.ReadAllText (path);
				AssetDatabase.DeleteAsset (path);
			} catch (IOException ex) {
				UnuLogger.LogError (ex);
			}

			return code;
		}

		/// <summary>
		/// Saves the script.
		/// </summary>
		/// <returns><c>true</c>, if script was saved, <c>false</c> otherwise.</returns>
		/// <param name="path">Path.</param>
		/// <param name="subFix">Sub fix.</param>
		/// <param name="code">Code.</param>
		/// <param name="overwrite">If set to <c>true</c> overwrite.</param>
		/// <param name="currentBaseType">Current base type.</param>
		/// <param name="newBaseType">New base type.</param>
		static public bool SaveScript (string path, string code, bool overwrite, string currentBaseType = "", string newBaseType = "")
		{
			try {
				if (!overwrite && File.Exists (path)) {
					return false;
				}
	
				string currentCode = "";
				if (File.Exists (path))
					currentCode = File.ReadAllText (path);
				if (!code.Equals (currentCode)) {
					File.WriteAllText (path, code, System.Text.Encoding.UTF8);
					return true;
				} else {
					return false;
				}
			} catch (IOException ex) {
				UnuLogger.LogError (ex);
				return false;
			}
		}
	}
}

namespace UnuGames
{
	/// <summary>
	/// Custom property info.
	/// </summary>
	public class CustomPropertyInfo
	{
		public bool IsSelected { get; set; }

		public string Name { get; set; }

		public string LastName { get; set; }

		public Type PropertyType { get; set; }

		public Type LastPropertyType { get; set; }

		public object DefaltValue { get; set; }

		public object LastValue { get; set; }

		public bool HasChange {
			get {
				return (DefaltValue != null && !DefaltValue.Equals (LastValue)) || !PropertyType.Equals (LastPropertyType) || !Name.Equals (LastName);
			}
		}

	
		public CustomPropertyInfo ()
		{
		}

		public CustomPropertyInfo (string name, Type type, object defaltValue = null)
		{
			Name = name;
			PropertyType = type;
		
			if (defaltValue != null && defaltValue.GetType () == type) {
				DefaltValue = defaltValue;
			} else {
				if (type.IsValueType)
					DefaltValue = Activator.CreateInstance (type);
				else
					DefaltValue = null;
			}
		
			LastValue = DefaltValue;
			LastPropertyType = PropertyType;
			LastName = Name;
		}

		public T GetLastValueAs<T> ()
		{
			try {
				return (T)LastValue;
			} catch {
				UnuLogger.Log ("Type of property has been changed, the default value will set to default value of new type!");
				return default(T);
			}
		}

		public void SetLastValueAs<T> (T value)
		{
			LastValue = value;
		}

		public void CommitChange ()
		{
			if (PropertyType != LastPropertyType) {
				PropertyType = LastPropertyType;
				if (PropertyType.IsAllias () && PropertyType.IsValueType)
					DefaltValue = ReflectUtils.GetDefaultValue (PropertyType);
				else
					DefaltValue = null;
			}
		
			DefaltValue = LastValue;
			Name = LastName;
		}

		public override string ToString ()
		{
			string strDefaultValue = "";
			if (PropertyType == typeof(string))
				strDefaultValue = DefaltValue == null ? "\"null\"" : "\"" + DefaltValue + "\"";
			else if (PropertyType.BaseType == typeof(ObservableModel))
				strDefaultValue = "new " + PropertyType.Name + "()";
			else
				strDefaultValue = DefaltValue.ToString ();
			if (PropertyType == typeof(bool)) {
				strDefaultValue = strDefaultValue.ToLower ();
			}		
			string fieldName = CodeGenerationHelper.NormalizeFieldName (Name);
			string propertyName = CodeGenerationHelper.NormalizePropertyName (Name);
			string field = string.Format ("\t{0} _{1} = {2};", PropertyType.GetAllias (), fieldName, strDefaultValue);
			string attribute = "\t[UIManProperty]";
			string property = string.Format ("\tpublic {0} {1} {{", PropertyType.GetAllias (), propertyName);
			string getter = string.Format ("\t\tget {{ return _{0}; }}", fieldName);
			string setter = string.Format ("\t\tset {{ _{0} = value; OnPropertyChanged(); }}", fieldName);
			string code = NewLine () + field + NewLine () + attribute + NewLine () + property + NewLine () + getter + NewLine () + setter + NewLine () + "\t}";
			return code;
		}

		string NewLine ()
		{
			return Environment.NewLine;
		}

		public string GetNamespace ()
		{
			return PropertyType.Namespace;
		}
	}
}
#endif