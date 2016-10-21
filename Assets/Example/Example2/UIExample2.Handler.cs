using UnityEngine;
using UnuGames;
using UnuGames.MVVM;
using System.Collections;

public partial class UIExample2 : UIManDialog {

#region Fields

	// Your fields here

#endregion

#region Built-in Events
	public override void OnShow (params object[] args)
	{
		base.OnShow (args);
		Messsage = (string)args [0];
		ButtonText = (string)args [1];
		TextColor = (Color)args [2];
		InternalImageName = (string)args [3];
		ShowImageLeft = (bool)args [4];
		NumberValue = (int)args [5];
		ExternalImageName = (string)args [6];
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
	public void Close1 () {
		this.Callback (0, "This is callback from dialog button 1!");
		UIMan.Loading.Show (WaitAbit ());
	}

	IEnumerator WaitAbit () {
		yield return new WaitForSeconds (10);
	}

	public void Close2 () {
		//this.Callback (1, "This is callback from dialog button 2!");
		UIMan.Loading.Show (true, true, false, true, "I love you!");
		StartCoroutine (ShowNextTip ());
	}

	IEnumerator ShowNextTip () {
		yield return new WaitForSeconds (1);
		UIMan.Loading.ShowImage ("Images/background4");
		UIMan.Loading.ShowTip ("You love me!");
		UIMan.Loading.ShowValue ("100%");
	}

	public void Log () {
		Debug.Log ("This is log from Example2!");
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
