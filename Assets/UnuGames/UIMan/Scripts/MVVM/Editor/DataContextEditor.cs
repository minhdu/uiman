using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace UnuGames.MVVM
{
	[CustomEditor(typeof(DataContext))]
	public class DataContextEditor : Editor
	{

		GUIContent lblType = new GUIContent ("Type");
		GUIContent lblContext = new GUIContent ("Context");
		int selected = 0;

		public override void OnInspectorGUI ()
		{

			DataContext context = (DataContext)target;

			context.type = (ContextType)EditorGUILayout.EnumPopup (lblType, context.type);

			if (context.type == ContextType.NONE) {
				context.Clear ();
				GUILayout.Label (BindingDefine.NO_CONTEXT_TYPE);
			} else if (context.type == ContextType.MONO_BEHAVIOR) {
				context.viewModel = (ViewModelBehaviour)EditorGUILayout.ObjectField (lblContext, (Object)context.viewModel, typeof(ViewModelBehaviour), true);
				if (context.viewModel.GetCachedType () != null) {
					GUILayout.BeginHorizontal ();
					EditorGUILayout.PrefixLabel (" ");
					EditorGUILayout.LabelField ("<color=blue>[" + context.viewModel.GetCachedType ().FullName + "]</color>", EditorGUIHelper.RichText ());
					GUILayout.EndHorizontal ();
				}
			} else if (context.type == ContextType.PROPERTY) {
			
				context.viewModel = (ViewModelBehaviour)EditorGUILayout.ObjectField (lblContext, (Object)context.viewModel, typeof(ViewModelBehaviour), true);

				string[] members = context.viewModel.GetAllMembers (MemberTypes.Field, MemberTypes.Property, MemberTypes.Field);
				if (members != null) {

					if (string.IsNullOrEmpty (context.propertyName)) {
						context.propertyName = members [0];
					} else {
						for (int i = 0; i < members.Length; i++) {
							if (members [i] == context.propertyName) {
								selected = i;
								break;
							}
						}
					}

					GUILayout.BeginVertical ();
					GUILayout.BeginHorizontal ();

					GUILayout.BeginVertical ();
					GUILayout.Space (5);
					int newSelected = EditorGUILayout.Popup ("Field/Property", selected, members);
					GUILayout.EndVertical ();

					if (selected != newSelected) {
						context.propertyName = members [newSelected];
						selected = newSelected;
					}

					if (EditorGUIHelper.QuickPickerButton ()) {
						ContextBrowser.Browse (members, selectedMember => {
							context.propertyName = selectedMember;
							FilterPopup.Close ();
						});
					}

					GUILayout.EndHorizontal ();
				
					MemberInfo curMember = context.viewModel.GetMemberInfo (members [selected], MemberTypes.Property, MemberTypes.Field);
					if (curMember != null) {
						object[] attributes = curMember.GetCustomAttributes (typeof(UIManProperty), false);
						if (attributes == null || attributes.Length == 0) {
							GUILayout.BeginHorizontal ();
							EditorGUILayout.PrefixLabel (" ");
							GUILayout.Label ("<color=red>None observable field/property!</color>", EditorGUIHelper.RichText ());
							GUILayout.EndHorizontal ();
						}
					}
					GUILayout.EndVertical ();
				}

				if (Event.current.type == EventType.Repaint) {
					FilterPopup.SetPopupRect (GUILayoutUtility.GetLastRect ());
				}
			}
		}
	}
}
