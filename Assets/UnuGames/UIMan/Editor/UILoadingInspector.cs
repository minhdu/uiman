using UnityEngine;
using System.Collections;
using UnityEditor;
using UnuGames;

[CustomEditor(typeof(UILoading))]
public class UILoadingInspector : Editor {

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginHorizontal ("Box");
		LableHelper.HeaderLabel ("UIMan Loading Indicator");
		GUILayout.EndHorizontal ();

		GUILayout.BeginVertical ("Box");
		DrawDefaultInspector ();
		GUILayout.EndVertical ();
	}
}
