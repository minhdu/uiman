using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Director;

namespace UnuGames
{
	public class UIAnimationState : StateMachineBehaviour
	{

		[SerializeField]
		bool isResetDialogTransitionStatus = true;
		[SerializeField]
		bool isDequeueDialog = false;
		[SerializeField]
		bool autoPlayIdle = false;
		[SerializeField]
		UIAnimationType type;
		UIManBase cachedUI;

		public void Init (bool resetDialogTransitionStatus, bool dequeueDialog)
		{
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

				animator.enabled = false;

				if (type == UIAnimationType.SHOW)
					cachedUI.UnlockInput ();

				if (type == UIAnimationType.SHOW) {
					cachedUI.OnShowComplete ();
				} else if (type == UIAnimationType.HIDE) {
					cachedUI.OnHideComplete ();
				}

				if (autoPlayIdle)
					UIMan.Instance.DoAnimIdle (cachedUI);
			}
		}
	}
}
