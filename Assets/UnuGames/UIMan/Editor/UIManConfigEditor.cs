using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using UnuGames;

[CustomEditor(typeof(UIManConfig))]
public class UIManConfigEditor : Editor {

	PathBrowser screenPath;
	PathBrowser dialogPath;
	PathBrowser bgPath;

	GUIContent screenGUI = new GUIContent("Screen: ", "The default path for the system find and load screen's prefab");
	GUIContent dialogGUI = new GUIContent("Dialog: ", "The default path for the system find and load dialog's prefab");
	GUIContent bgGUI = new GUIContent("Background: ", "The default path for the system find and load background image");
	string rootUrl = Application.dataPath;

	public override void OnInspectorGUI ()
	{
		UIManConfig config = target as UIManConfig;

		if (screenPath == null || dialogPath == null || bgPath == null) {
			screenPath = new PathBrowser (config.screenPrefabFolder, rootUrl);
			dialogPath = new PathBrowser (config.dialogPrefabFolder, rootUrl);
			bgPath = new PathBrowser (config.backgroundRootFolder, rootUrl);
		}

		LableHelper.TitleLabel ("UIMan Configuration");
		LineHelper.Draw (Color.blue);
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Box");
		config.screenPrefabFolder = screenPath.Draw (screenGUI);
		config.dialogPrefabFolder = dialogPath.Draw (dialogGUI);
		config.backgroundRootFolder = bgPath.Draw (bgGUI);
		GUILayout.EndVertical ();

		GUILayout.BeginHorizontal ("Box");
		GUILayout.Label ("<b>Warning:</b> This configuration use to set default path of prefabs/images for UI, all url must be child of Unity's Resources folder\n\n" +
			"If you don't want to use this fault path for your Screen/Dialog, apply UIDescriptor to your class to define custom path", EditorGUIHelper.RichText(true));
		GUILayout.EndHorizontal ();

		EditorUtility.SetDirty (target);
	}
}
