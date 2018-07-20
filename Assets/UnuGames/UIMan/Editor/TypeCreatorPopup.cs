using UnityEngine;
using System.Collections;
using UnityEditor;
using UnuGames;
using System;
using System.Text.RegularExpressions;
using System.IO;

namespace UnuGames
{
	public class TypeCreatorPopup : EditorWindow
	{
		string baseType = "UIManDialog";

		EditablePopup baseTypePopup;
		string[] arrSupportType = new string[3] {
			"ObservableModel",
			"UIManScreen",
			"UIManDialog"
		};

		bool inited = false;
		string typeName = "NewViewModel";

		void Init ()
		{
			if (!inited) {
				if (baseTypePopup == null) {
					baseTypePopup = new EditablePopup (arrSupportType, "UIManDialog", null);
				}
				minSize = new Vector2 (300, 80);
				maxSize = minSize;
				inited = true;
			}
		}

		void OnGUI ()
		{
			Init ();
			GUILayout.Space (10);
			baseTypePopup.Draw ();
			LineHelper.Draw (Color.black);
			GUILayout.Space (10);

			if (ColorButton.Draw ("Create", CommonColor.LightGreen, GUILayout.Height (30))) {

				string lastPath = "";
				UIManConfig uiManConfig = Resources.Load<UIManConfig> ("UIManConfig");
				if (uiManConfig != null) {
					if (baseTypePopup.SelectedItem == arrSupportType [0])
						lastPath = uiManConfig.modelScriptFolder;
					else if (baseTypePopup.SelectedItem == arrSupportType [1])
						lastPath = uiManConfig.screenScriptFolder;
					else if (baseTypePopup.SelectedItem == arrSupportType [2])
						lastPath = uiManConfig.dialogScriptFolder;
				}

				lastPath = EditorUtility.SaveFilePanel ("Save script", Application.dataPath + lastPath, typeName, "cs");

				if (!string.IsNullOrEmpty (lastPath)) {
					typeName = Path.GetFileNameWithoutExtension (lastPath);

					lastPath = Path.GetDirectoryName (lastPath).Replace (Application.dataPath, "");
					if (baseTypePopup.SelectedItem == arrSupportType [0]) {
						uiManConfig.modelScriptFolder = lastPath;
						uiManConfig.generatingTypeIsDialog = false;
					} else if (baseTypePopup.SelectedItem == arrSupportType [1]) {
						uiManConfig.screenScriptFolder = lastPath;
						uiManConfig.generatingTypeIsDialog = false;
					} else if (baseTypePopup.SelectedItem == arrSupportType [2]) {
						uiManConfig.dialogScriptFolder = lastPath;
						uiManConfig.generatingTypeIsDialog = true;
					}
					EditorUtility.SetDirty(uiManConfig);

					GenerateViewModel ();
				}
			}
		}

		public void GenerateViewModel ()
		{
			if (typeName.Contains (" ")) {
				EditorUtility.DisplayDialog ("Error", "View model name cannot constain special character", "OK");
				return;
			}

			bool warn = false;
			
			if (typeName.Length <= 1 || (!typeName.Substring (0, 2).Equals ("UI") && !baseTypePopup.SelectedItem.Equals (UIGenerator.GetSupportTypeName (0)))) {
				typeName = "UI" + typeName;
				warn = true;
			}

			baseType = baseTypePopup.SelectedItem;


			UIManConfig config = Resources.Load<UIManConfig> ("UIManConfig");
			
			string savePath = "";
			if (baseType.Equals (UIGenerator.GetSupportTypeName (0))) {
				savePath = config.modelScriptFolder;
				config.generatingTypeIsDialog = false;
			}
			else if (baseType.Equals (UIGenerator.GetSupportTypeName (1))) {
				savePath = config.screenScriptFolder;
				config.generatingTypeIsDialog = false;
			} else if (baseType.Equals (UIGenerator.GetSupportTypeName (2))) {
				savePath = config.dialogScriptFolder;
				config.generatingTypeIsDialog = true;
			}
			
			savePath = Application.dataPath + "/" + savePath + "/" + typeName + ".cs";
			if (File.Exists (savePath) || UIGenerator.IsViewModelExisted(typeName)) {
				EditorUtility.DisplayDialog ("Error", "View model name is already exist, please input other name!", "OK");
				return;
			}

			string[] paths = Regex.Split (savePath, "/");
			string scriptName = paths [paths.Length - 1];
			scriptName = scriptName.Replace (".cs", "");

			if(baseType != arrSupportType[0])
				config.generatingType = typeName;

			string code = CodeGenerationHelper.GenerateScript (typeName, baseType);
			CodeGenerationHelper.SaveScript (savePath, code, true);

			if(baseType != arrSupportType[0])
				GenerateViewModelHandler (savePath);

			AssetDatabase.Refresh (ImportAssetOptions.Default);

			if (warn) {
				Debug.LogWarning ("Code generation warning: Invalid name detected, auto generate is activated!");
			}

			Close ();
		}

		public void GenerateViewModelHandler (string scriptPath)
		{
			string handlerScriptPath = CodeGenerationHelper.GeneratPathWithSubfix (scriptPath, ".Handler.cs");
			string handlerCode = "";
			if (string.IsNullOrEmpty (handlerCode))
				handlerCode = CodeGenerationHelper.GenerateViewModelHandler (typeName, baseType);
			else
				handlerCode = handlerCode.Replace (": " + typeName, ": " + baseType);
			CodeGenerationHelper.SaveScript (handlerScriptPath, handlerCode, false, typeName, baseType);
		}
	}
}