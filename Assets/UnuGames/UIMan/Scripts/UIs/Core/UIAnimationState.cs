using UnityEngine;
using System.Collections;

namespace UnuGames
{
	public class UIAnimationState : StateMachineBehaviour
	{

		[SerializeField]
		bool isResetDialogTransitionStatus = true;
		[SerializeField]
		bool isDequeueDialog = false;
		[SerializeField]
		bool autoPlayIdle = true;
		[SerializeField]
		UIAnimationType type;
		UIManBase cachedUI;

		public void Init (UIAnimationType anim, bool resetDialogTransitionStatus, bool dequeueDialog)
		{
			type = anim;
			isResetDialogTransitionStatus = resetDialogTransitionStatus;
			isDequeueDialog = dequeueDialog;
		}

		public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (stateInfo.normalizedTime >= 1.0f) {
				if (cachedUI == null)
					cachedUI = animator.GetComponent<UIManBase> ();
				if (cachedUI.GetUIBaseType () == UIBaseType.DIALOG) {
					if (isResetDialogTransitionStatus)
						UIMan.Instance.IsInDialogTransition = false;
					if (isDequeueDialog)
						UIMan.Instance.DequeueDialog ();
				}

				if (type == UIAnimationType.SHOW) {//TODO: bug!?
					cachedUI.UnlockInput ();
					cachedUI.OnShowComplete ();
				} else if (type == UIAnimationType.HIDE) {
					cachedUI.OnHideComplete ();
				}

				if (autoPlayIdle && cachedUI.motionIdle == UIMotion.CUSTOM_MECANIM_ANIMATION)
					UIMan.Instance.DoAnimIdle (cachedUI);
			}
		}
	}
}
