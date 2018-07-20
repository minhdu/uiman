using UnityEngine;
using UnuGames;
using UnuGames.MVVM;
using System.Collections;

public partial class UIGameplay : UIManScreen {

#region Fields

	// Your fields here
	float timer;
	float circleTimer;
	float circleMultiplier = 1;
	public HeaderModule header;
#endregion

#region Built-in Events
	public override void OnShow (params object[] args)
	{
		base.OnShow (args);
		StartCoroutine ("SimulateSlider");
	}

	public override void OnShowComplete ()
	{
		base.OnShowComplete ();
	}

	public override void OnHide ()
	{
		base.OnHide ();
		StopCoroutine ("SimulateSlider");
	}

	public override void OnHideComplete ()
	{
		base.OnHideComplete ();
	}
#endregion

#region Custom implementation

	IEnumerator SimulateSlider () {
		while (true) {
			Progress = 1;
			yield return new WaitForSeconds (5);
			Progress = 0;
			yield return new WaitForSeconds (5);
		}
	}

	// Your custom code here
	void Update () {
		if (State != UIState.SHOW)
			return;

		circleTimer += circleMultiplier * Time.deltaTime * 10;
		if (circleTimer > 10) {
			circleMultiplier = -1;
			header.DataInstance.Gold += Random.Range (500, 5000);
		} else if (circleTimer < 0) {
			circleMultiplier = 1;
		}
		CircleProgress = circleTimer / 10;

		timer += Time.deltaTime;
		if (timer > 10) {
			timer = 0;
			if(!UIMan.Instance.IsShowingDialog<UIMissionComplete>())
				UIMan.Instance.ShowDialog<UIMissionComplete> ();
		}

		Timer = Mathf.FloorToInt (timer);
	}

	public void Replay() {
		StopCoroutine ("SimulateSlider");
		Progress = 0;
		CircleProgress = 0;
		StartCoroutine ("SimulateSlider");
		timer = 0;
	}
#endregion

#region Override animations
	/* Uncommend this for override show/hide animation of Screen/Dialog use tweening code
	public override IEnumerator AnimationShow ()
	{
		return base.AnimationShow ();
	}

	public override IEnumerator AnimationHide ()
	{
		return base.AnimationHide ();
	}

	public override IEnumerator AnimationIdle ()
	{
		return base.AnimationHide ();
	}
	*/
#endregion
}
