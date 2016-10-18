using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace UnuGames
{
	public class UIGenerator : EditorWindow
	{
		const string PREFAB_EXT = ".prefab";
			
		static string savedDirectory;
		static UIGenerator container;
		UISearchField searchField;
		ListView listTypes;
		EditablePopup baseTypePopup;
		static string[] types;
		static float typeAreaWidth = 250f;
		static float propertiesAreaWidth = 540f;
		static Vector2 propertiesScrollPos;

		static Dictionary<Type, EditablePropertyDrawer[]> propertiesDrawerCache = new Dictionary<Type, EditablePropertyDrawer[]> ();
		static Type selectedType = null;
		static CustomPropertyInfo[] selectedProperties = null;

		static string currentScriptPath = null;
		static string handlerScriptPath = null;
		static string[] arrSupportType = new string[3] {
			"ObservableModel",
			"UIManScreen",
			"UIManDialog"
		};

		static bool reload = true;
		static UIManConfig config;

		static public string GetSupportTypeName (int index) {
			return arrSupportType [index];
		}

		static public bool IsViewModelExisted (string name) {
			return ArrayUtility.Contains (types, name);
		}

		[MenuItem("UIMan/UI Generator", false, 1)]
		static void Init ()
		{
			ReflectUtils.RefreshAssembly (false);
			types = ReflectUtils.GetAllUIManType ();
			container = EditorWindow.GetWindow<UIGenerator> (true, "UIMan - UI Generator");
			container.minSize = new Vector2 (800, 600);
			container.maxSize = container.minSize;
			GetConfig ();
		}

		[MenuItem("UIMan/Configuration", false, 2)]
		static void Config ()
		{
			GetConfig ();
			Selection.activeObject = config;
		}

		static void GetConfig () {
			config = Resources.Load<UIManConfig> ("UIManConfig");
			if (config == null)
				ConfigFile.Create<UIManConfig> ();
		}

		void OnGUI ()
		{
			if (EditorApplication.isCompiling) {
				ShowNotification (new GUIContent("Unity is compiling..."));
				GUI.enabled = false;
			} else {
				GUI.enabled = true;
			}

			if (container == null) {
				Init ();
			}

			GUILayout.BeginHorizontal ();

			GUILayout.BeginVertical ("Box", GUILayout.Width (typeAreaWidth), GUILayout.ExpandHeight(true));
			TypeWindow ();
			GUILayout.EndVertical ();

			GUILayout.BeginVertical ("Box", GUILayout.Width (propertiesAreaWidth), GUILayout.ExpandHeight(true));
			PropertiesWindow ();
			GUILayout.EndVertical ();

			GUILayout.EndHorizontal ();
		}

		void TypeWindow (int id = 0)
		{
			GUILayout.BeginVertical ();

			GUILayout.Space (2);

			GUILayout.EndVertical ();

			if (searchField == null)
				searchField = new UISearchField (OnSearchType, OnClickCreateType);
			
			searchField.Draw ();

			if (listTypes == null)
				listTypes = new ListView ();
			listTypes.SetData (types, false, OnSelecType, searchField.KeyWord, this);
			listTypes.Draw ();

			if(reload && config != null) {
				if(!string.IsNullOrEmpty(config.selectedType))
					listTypes.Select (config.selectedType);

				MakePrefab ();

				reload = false;
			}
		}
	
		void LayoutWindow (int id = 1)
		{
			GUILayout.Label ("NO LAYOUT HAS BEEN SELECTED!");
		}
	
		void PropertiesWindow (int id = 2)
		{
			GUILayout.BeginVertical ();

			if (listTypes != null && !string.IsNullOrEmpty (listTypes.SelectedItem)) {
				if (selectedType != null) {

					// Title
					GUILayout.Space (2);
					LableHelper.TitleLabel (selectedType.Name);
					LineHelper.Draw (Color.gray);

					// Common
					GUILayout.Space (2);
					GUILayout.BeginHorizontal();
					if (GUILayout.Button ("Edit UI", GUILayout.Height (30))) {

						GameObject prefabInstance;
						UnityEngine.Object obj = FindObjectOfType (selectedType);
						if (obj != null) {
							prefabInstance = ((MonoBehaviour)obj).gameObject;
						} else {

							bool isDialog = selectedType.BaseType == typeof(UIManDialog);
							string prefabFolder = GetUIPrefabPath (selectedType, isDialog);
							string prefabFile = selectedType.Name + PREFAB_EXT;
							string prefabPath = Path.Combine (prefabFolder, prefabFile);
							GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject> (prefabPath);
							if (prefab == null) {
								prefab = FindAssetObject<GameObject> (selectedType.Name, PREFAB_EXT);
							}

							prefabInstance = PrefabUtility.InstantiatePrefab (prefab) as GameObject;
							if (isDialog)
								prefabInstance.transform.SetParent (UIMan.Instance.dialogRoot, false);
							else
								prefabInstance.transform.SetParent (UIMan.Instance.screenRoot, false);
						}
						Selection.activeGameObject = prefabInstance;
					}
					GUILayout.EndHorizontal ();
					LineHelper.Draw (Color.gray);

					// Base type
					GUILayout.Space (10);
					LableHelper.HeaderLabel ("Type");
					LineHelper.Draw (Color.gray);
					baseTypePopup.Draw ();

					if (baseTypePopup.SelectedItem != "ObservableModel") {
						if (!System.IO.File.Exists (handlerScriptPath)) {
							if (GUILayout.Button ("Generate Handler")) {
								string backupCode = CodeGenerationHelper.DeleteScript (handlerScriptPath);
								GenerateViewModelHandler (backupCode, selectedType.BaseType.Name);
							}
						}
					}

					// Properties
					GUILayout.Space (10);
					LableHelper.HeaderLabel ("Properties");
					LineHelper.Draw (Color.gray);

					propertiesScrollPos = EditorGUILayout.BeginScrollView(propertiesScrollPos, GUILayout.ExpandWidth (true), GUILayout.ExpandHeight (true));
					if (propertiesDrawerCache.ContainsKey (selectedType)) {
						EditablePropertyDrawer[] props = propertiesDrawerCache [selectedType];
						for (int i=0; i<props.Length; i++) {
							props [i].Draw (propertiesAreaWidth);
						}
					}
					EditorGUILayout.EndScrollView ();
				}

				GUILayout.Space (10);
			
				// Add property
				if (GUILayout.Button ("Add New Property", GUILayout.Height (30))) {
					int newIndex = 0;
					string strNewIndex = "";
					for (int i=0; i<selectedProperties.Length; i++) {
						if (selectedProperties [i].LastName.Contains ("NewProperty"))
							newIndex++;
					}
					if (newIndex > 0)
						strNewIndex = newIndex.ToString ();
					CustomPropertyInfo newProperty = new CustomPropertyInfo ("", typeof(string));
					newProperty.LastName = "NewProperty" + strNewIndex;
					ArrayUtility.Add<CustomPropertyInfo> (ref selectedProperties, newProperty);
					CachePropertiesDrawer ();
				}

				//Save all change
				CustomPropertyInfo[] changeList = new CustomPropertyInfo[0];
				CustomPropertyInfo[] selectedList = new CustomPropertyInfo[0];
				for (int i=0; i<selectedProperties.Length; i++) {
					if (selectedProperties [i].HasChange)
						ArrayUtility.Add (ref changeList, selectedProperties [i]);
					if (selectedProperties [i].IsSelected)
						ArrayUtility.Add (ref selectedList, selectedProperties [i]);
				}

				GUILayout.Space (10);
				LineHelper.Draw (Color.gray);
				GUILayout.Space (5);

				if (changeList.Length > 0) {
					if (GUILayout.Button ("Save All Changes", GUILayout.Height (30))) {
						for (int i=0; i<changeList.Length; i++) {
							changeList [i].CommitChange ();
						}
						SaveCurrentType (true, baseTypePopup.SelectedItem);
					}
				}

				if (selectedList.Length > 0) {
					if (GUILayout.Button ("Delete Selected Properties", GUILayout.Height (30))) {
						for (int i=0; i<selectedList.Length; i++) {
							ArrayUtility.Remove (ref selectedProperties, selectedList [i]);
						}
						SaveCurrentType (true, baseTypePopup.SelectedItem);
						CachePropertiesDrawer (true);
					}
				}

				if (selectedProperties.Length > 0) {
					if (GUILayout.Button ("Delete All Properties", GUILayout.Height (30))) {
						while (selectedProperties.Length > 0) {
							ArrayUtility.Clear (ref selectedProperties);
							SaveCurrentType ();
							CachePropertiesDrawer (true);
						}
					}
				}
			} else {
				GUILayout.Label ("NO DATA FOR PREVIEW!");
			}


			GUILayout.EndVertical ();
		}

		void CachePropertiesDrawer (bool clearCurrentCache = false)
		{
			if (selectedType == null)
				return;
			if (clearCurrentCache)
				propertiesDrawerCache.Clear ();
			if (!propertiesDrawerCache.ContainsKey (selectedType)) {
				propertiesDrawerCache.Add (selectedType, new EditablePropertyDrawer[0]);
			}
			EditablePropertyDrawer[] drawers = new EditablePropertyDrawer[0];
			for (int i=0; i<selectedProperties.Length; i++) {
				EditablePropertyDrawer drawer = new EditablePropertyDrawer (selectedType, selectedProperties [i], OnApplyPropertyChanged, OnPropertyDelete);
				ArrayUtility.Add<EditablePropertyDrawer> (ref drawers, drawer);						
			}
			propertiesDrawerCache [selectedType] = drawers;
		}

		public void OnSearchType (string keyword)
		{
		}

		public void OnClickCreateType (object obj)
		{
			GetWindow<TypeCreatorPopup> (true, "Create new type", true);
		}

		public void OnChangeBaseType (string newBaseType)
		{
			SaveCurrentType (true, newBaseType);
		}

		public void OnSelecType (string typeName)
		{
			config.selectedType = typeName;
			selectedType = ReflectUtils.GetTypeByName (typeName);
			selectedProperties = selectedType.GetUIManProperties ();
			baseTypePopup = new EditablePopup (arrSupportType, selectedType.BaseType.Name, OnChangeBaseType);
			currentScriptPath = CodeGenerationHelper.GetScriptPathByType (selectedType);
			handlerScriptPath = CodeGenerationHelper.GeneratPathWithSubfix (currentScriptPath, ".Handler.cs");
			CachePropertiesDrawer ();
		}

		public void OnApplyPropertyChanged (CustomPropertyInfo newInfo)
		{
			SaveCurrentType (true);
		}

		public void OnPropertyDelete (CustomPropertyInfo property)
		{
			ArrayUtility.Remove<CustomPropertyInfo> (ref selectedProperties, property);
			SaveCurrentType ();
			CachePropertiesDrawer ();
		}

		public void SaveCurrentType (bool warning = false, string baseType = null)
		{

			// Verify properties list
			for (int i=0; i<selectedProperties.Length; i++) {
				CustomPropertyInfo property = selectedProperties [i];
				if (string.IsNullOrEmpty (property.Name) || Char.IsNumber (property.Name [0])) {
					property.Name = "";
					if (warning)
						EditorUtility.DisplayDialog ("Save script error", "Property name cannot be a digit, null or empty!", "OK");
					return;
				}

				for (int j=0; j<selectedProperties.Length; j++) {
					if (j != i && selectedProperties [i].Name.ToLower () == selectedProperties [j].Name.ToLower ()) {
						selectedProperties [j].Name = "";
						if (warning)
							EditorUtility.DisplayDialog ("Save script error", "There are one or more properties are have the same name!", "OK");
						return;
					}
				}
			}

			if (baseType == null)
				baseType = selectedType.BaseType.Name;

			if (!string.IsNullOrEmpty (currentScriptPath)) {
				string backupCode = CodeGenerationHelper.DeleteScript (handlerScriptPath);
				string code = CodeGenerationHelper.GenerateScript (selectedType.Name, baseType, selectedProperties);

				bool saved = CodeGenerationHelper.SaveScript (currentScriptPath, code, true);

				if (baseType != "ObservableModel") {
					GenerateViewModelHandler (backupCode, baseType);
					saved = false;
				}

				if (saved) {
					AssetDatabase.Refresh (ImportAssetOptions.Default);
				}
			}
		}

		public void GenerateViewModelHandler (string backupCode, string baseType = null)
		{
			if (string.IsNullOrEmpty (baseType))
				baseType = selectedType.BaseType.Name;

			string handlerCode = backupCode;
			if (string.IsNullOrEmpty (handlerCode))
				handlerCode = CodeGenerationHelper.GenerateViewModelHandler (selectedType.Name, baseType);
			else
				handlerCode = handlerCode.Replace (": " + selectedType.BaseType.Name, ": " + baseType);
			bool saved = CodeGenerationHelper.SaveScript (handlerScriptPath, handlerCode, false, selectedType.BaseType.Name, baseType);
			if (saved) {
				AssetDatabase.Refresh (ImportAssetOptions.Default);
			}
		}

		void MakePrefab () {
			if (!string.IsNullOrEmpty (config.generatingType)) {
				GameObject prefabTemplate = FindAssetObject<GameObject> ("@UI_PREFAB_TEMPLATE", PREFAB_EXT);
				if (prefabTemplate != null) {
					GameObject newPrefab = Instantiate (prefabTemplate);
					Type generatedType = ReflectUtils.GetTypeByName (config.generatingType);
					if (generatedType != null) {
						ViewModelBehaviour newVM = (ViewModelBehaviour)newPrefab.AddComponent (generatedType);
						newPrefab.name = config.generatingType;
						newPrefab.GetComponent<DataContext> ().viewModel = newVM;
					}

					string newPrefabPath = "Assets/" + (config.generatingTypeIsDialog ? config.dialogPrefabFolder : config.screenPrefabFolder);
					EditorUtils.CreatePath (newPrefabPath);
					PrefabUtility.CreatePrefab (Path.Combine(newPrefabPath, config.generatingType + PREFAB_EXT) , newPrefab);

					DestroyImmediate (newPrefab);
				}

				config.generatingType = null;
			}
		}

		T FindAssetObject<T> (string name, string extension) where T : UnityEngine.Object {
			T obj = default(T);
			string[] files = AssetDatabase.FindAssets (name);
			if (files != null && files.Length > 0) {
				foreach (string file in files) {
					string filePath = AssetDatabase.GUIDToAssetPath (file);
					if (filePath.EndsWith (extension)) {
						obj = AssetDatabase.LoadAssetAtPath<T> (filePath);
						break;
					}
				}
			}

			return obj;
		}

		string GetUIPrefabPath (Type uiType, bool isDialog) {
			string url = "";
			object[] attributes = uiType.GetCustomAttributes(typeof(UIDescriptor), true);
			if (attributes != null && attributes.Length > 0) {
				url = ((UIDescriptor)attributes [0]).URL;
			} else {
				if (isDialog) {
					url = config.dialogPrefabFolder;
				} else {
					url = config.screenPrefabFolder;
				}
			}

			return url;
		}

		[UnityEditor.Callbacks.DidReloadScripts]
		static void OnScriptsReloaded ()
		{
			ReflectUtils.RefreshAssembly (true);
		}
	}
}