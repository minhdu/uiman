using UnityEngine;
using UnuGames;
using UnuGames.MVVM;

public partial class UIExample7 : UIManScreen {

#region Fields

	// Your fields here
#endregion

#region Built-in Events
	public override void OnShow (params object[] args)
	{
		base.OnShow (args);
		User = new UserModel ();
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
	public void Change1 () {
		User.Name = "Minh Du";
		User.Avatar = "";
		User.Age = 18;
	}

	public void Change2 () {
		User = new UserModel () { Name = "Dang Minh Du", Age = 25, Avatar = "uiman-icon-1"};
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
