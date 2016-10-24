using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;

namespace UnuGames
{
	[CustomEditor (typeof(UIMan))]
	public class UIManInspector : Editor
	{
		GUIContent uiRoot = new GUIContent ("UI Root", "The root transform of all UI element");
		GUIContent screenRoot = new GUIContent ("Screen Root", "The root transform of all screen");
		GUIContent dialogRoot = new GUIContent ("Dialog Root", "The root transform of all dialog");
		GUIContent backgroundImg = new GUIContent ("Background", "The Image to render background of any Screen");
		GUIContent coverTrans = new GUIContent ("Cover", "The transform of object cover all UI behind Dialog");

		public override void OnInspectorGUI ()
		{
		
			UIMan uiManager = (UIMan)target;

			GUILayout.BeginHorizontal ("Box");
			LabelHelper.HeaderLabel ("UIMan Root");
			GUILayout.EndHorizontal ();

			GUILayout.BeginVertical ("Box");
			uiManager.uiRoot = EditorGUILayout.ObjectField (uiRoot, uiManager.uiRoot, typeof(Transform), true) as Transform;
			uiManager.screenRoot = EditorGUILayout.ObjectField (screenRoot, uiManager.screenRoot, typeof(Transform), true) as Transform;
			uiManager.dialogRoot = EditorGUILayout.ObjectField (dialogRoot, uiManager.dialogRoot, typeof(Transform), true) as Transform;
			uiManager.background = EditorGUILayout.ObjectField (backgroundImg, uiManager.background, typeof(Image), true) as Image;
			uiManager.cover = EditorGUILayout.ObjectField (coverTrans, uiManager.cover, typeof(RectTransform), true) as RectTransform;
			GUILayout.EndHorizontal ();
		}
	}
}