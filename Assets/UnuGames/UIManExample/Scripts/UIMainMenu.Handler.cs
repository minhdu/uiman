using UnityEngine;
using UnuGames;
using UnuGames.MVVM;
using System.Collections;

public partial class UIMainMenu : UIManScreen {

#region Fields

	// Your fields here
#endregion

#region Built-in Events
	public override void OnShow (params object[] args)
	{
		base.OnShow (args);
		SoundOff = false;
	}

	public override void OnShowComplete ()
	{
		base.OnShowComplete ();
	}

	public override void OnHide ()
	{
		base.OnHide ();
	}

	public override void OnHideComplete ()
	{
		base.OnHideComplete ();
	}
#endregion

#region Custom implementation

	// Your custom code here
	public void Play () {
		UIMan.Instance.ShowScreen<UIMissionList> ();
	}

	public void About () {
		UIMan.Instance.ShowPopup ("About", "UIMan version 1.0.1 \n Best UI Development & Management System", "OK");
	}

	public void OnOffSound () {
		SoundOff = !SoundOff;
		FindObjectOfType<AudioSource> ().enabled = !SoundOff;
	}

	public void Ranking () {
		UIMan.Loading.Show (FakeLoadRanking (), true, true, false, true, "Fetching data from server... please wait!");
	}

	IEnumerator FakeLoadRanking () {
		yield return new WaitForSeconds(3);
		UIMan.Instance.ShowDialog<UIRanking> (999);
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
