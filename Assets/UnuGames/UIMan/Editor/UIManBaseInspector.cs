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
		GUIContent background = new GUIContent ("Use Background", "Setting background image behine your screen elements");

		public override void OnInspectorGUI ()
		{
			GUILayout.BeginHorizontal ("Box");
			LabelHelper.HeaderLabel ("UIMan View Model");
			GUILayout.EndHorizontal ();
			LineHelper.Draw (Color.gray);

			UIManBase uiManBase = (UIManBase)target;
		
			EditorGUILayout.Space ();
			LabelHelper.HeaderLabel ("General");
			GUILayout.BeginVertical ("Box");

			if (uiManBase is UIManDialog) {
				UIManDialog dialog = (UIManDialog)uiManBase;
				dialog.useCover = EditorGUILayout.Toggle (cover, dialog.useCover);
				EditorUtility.SetDirty (target);
			}
			else if (uiManBase is UIManBase) {
				UIManScreen screen = (UIManScreen)uiManBase;
				screen.useBackground = EditorGUILayout.Toggle (background, screen.useBackground);
				if(screen.useBackground)
					screen.backgroundType = EditorGUILayout.TextField (screen.backgroundType);
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
			
			UIMotion[] motions = new UIMotion[3] {uiManBase.motionShow, uiManBase.motionHide, uiManBase.motionIdle};
			bool haveMecanimAnim = false;
			bool haveTweenAnim = false;
			foreach (UIMotion m in motions) {
				if ((int)m == 7)
					haveMecanimAnim = true;
				else
					haveTweenAnim = true;
			}
			if(haveTweenAnim && haveMecanimAnim) {
				GUILayout.BeginHorizontal ("Box");
				EditorGUILayout.LabelField ("<color=red><b>Warning: </b>Your motion type is not match with each others so it maybe cause unexpected error!\nPlease select all motion type as Mecanim if you want to make you animation manually with Unity animation editor!</color>", EditorGUIHelper.RichText (true));
				GUILayout.EndHorizontal ();
			}
			
			if (uiManBase.motionIdle != UIMotion.CUSTOM_MECANIM_ANIMATION && uiManBase.motionIdle != UIMotion.NONE) {
				GUILayout.BeginHorizontal ("Box");
				EditorGUILayout.LabelField ("<color=red><b>Warning: </b>Idle motion is now only support Mecanim animation!</color>", EditorGUIHelper.RichText (true));
				GUILayout.EndHorizontal ();
			}

			uiManBase.animTime = EditorGUILayout.FloatField (time, uiManBase.animTime);
			uiManBase.showPosition = EditorGUILayout.Vector3Field (position, uiManBase.showPosition);

			GUILayout.EndVertical ();
			LineHelper.Draw (Color.gray);

			EditorGUILayout.Space ();
			LabelHelper.HeaderLabel ("Custom fields");
			GUILayout.BeginVertical ("Box");
			DrawDefaultInspector ();
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
			if (GUILayout.Button ("Edit Logic", GUILayout.Height(25))) {
				string handler = CodeGenerationHelper.GetScriptPathByType (target.GetType ());
				handler = handler.Replace (".cs", ".Handler.cs");
				UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal (handler, 1);
			}

		}
	}
}