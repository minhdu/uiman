using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UnuGames
{
	[CustomEditor(typeof(UIManBase), true)]
	public class UIManBaseInspector : Editor
	{

		GUIContent animator = new GUIContent ("Animator", "Animator component to do custom animation");
		GUIContent show = new GUIContent ("Show", "Animation to do when UI is show");
		GUIContent hide = new GUIContent ("Hide", "Animation to do when UI is hide");
		GUIContent idle = new GUIContent ("Idle", "Animation to do when UI is idle");
		GUIContent time = new GUIContent ("Time", "Total time to do built-in or custom script animation");
		GUIContent position = new GUIContent ("Position", "Target position to show UI");
		GUIContent cover = new GUIContent ("Use Cover", "Show gray cover after dialog to prevent click behind elements");

		public override void OnInspectorGUI ()
		{
			DrawDefaultInspector ();

			UIManBase uiManBase = (UIManBase)target;
		
			if (uiManBase is UIManDialog) {
				UIManDialog dialog = (UIManDialog)uiManBase;
				dialog.useCover = EditorGUILayout.Toggle (cover, dialog.useCover);
				EditorUtility.SetDirty (target);
			}

			if (uiManBase.motionShow == UIMotion.CUSTOM_MECANIM_ANIMATION || uiManBase.motionHide == UIMotion.CUSTOM_MECANIM_ANIMATION) {
				if (uiManBase.gameObject != null) {
					uiManBase.animRoot = uiManBase.gameObject.GetComponent<Animator> ();
				}

				uiManBase.animRoot = EditorGUILayout.ObjectField (animator, uiManBase.animRoot, typeof(Animator), true) as Animator;

				if (uiManBase.animRoot == null || uiManBase.animRoot.runtimeAnimatorController == null) {
					if (GUILayout.Button ("Generate Animator")) {
						AnimationEditorUtils.GenerateAnimator (uiManBase.gameObject, UIManDefine.ANIM_SHOW, UIManDefine.ANIM_HIDE, UIManDefine.ANIM_IDLE);
					}
				}
			}

			uiManBase.motionShow = (UIMotion)EditorGUILayout.EnumPopup (show, uiManBase.motionShow);
			uiManBase.motionHide = (UIMotion)EditorGUILayout.EnumPopup (hide, uiManBase.motionHide);
			uiManBase.motionIdle = (UIMotion)EditorGUILayout.EnumPopup (idle, uiManBase.motionIdle);
			uiManBase.animTime = EditorGUILayout.FloatField (time, uiManBase.animTime);
			uiManBase.showPosition = EditorGUILayout.Vector3Field (position, uiManBase.showPosition);

			if (GUILayout.Button ("Edit Logic")) {
				string handler = CodeGenerationHelper.GetScriptPathByType (target.GetType ());
				handler = handler.Replace (".cs", ".Handler.cs");
				UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal (handler, 1);
			}
		}
	}
}