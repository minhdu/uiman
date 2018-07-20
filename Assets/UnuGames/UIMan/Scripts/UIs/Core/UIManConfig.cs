using UnityEngine;
using System.Collections;

public class UIManConfig : ScriptableObject {

	public string dialogPrefabFolder;
	public string screenPrefabFolder;
	public string backgroundRootFolder;
	public string animRootFolder;

#if UNITY_EDITOR
	public string modelScriptFolder;
	public string dialogScriptFolder;
	public string screenScriptFolder;
	public string selectedType;
	public string generatingType;
	public bool generatingTypeIsDialog;
#endif
}
