using UnityEngine;
using System.Collections;
using UnityEditor;
using UnuGames;

[CustomEditor(typeof(UIActivity))]
public class UIActivityInspector : Editor {

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginHorizontal ("Box");
		LabelHelper.HeaderLabel ("UIMan Activity Indicator");
		GUILayout.EndHorizontal ();

		GUILayout.BeginVertical ("Box");
		DrawDefaultInspector ();
		GUILayout.EndVertical ();
	}
}
