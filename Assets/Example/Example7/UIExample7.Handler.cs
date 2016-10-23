using UnityEngine;
using UnuGames;
using UnuGames.MVVM;

public partial class UIExample7 : UIManScreen {

#region Fields

	// Your fields here
	public UserModule user;
#endregion

#region Built-in Events
	public override void OnShow (params object[] args)
	{
		base.OnShow (args);
		user.DataInstance = new UserModel ();
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
		user.DataInstance.Name = "Minh Du";
		user.DataInstance.Avatar = null;
		user.DataInstance.Age = 18;
	}

	public void Change2 () {
		user.DataInstance = new UserModel () { Name = "Dang Minh Du", Age = 25, Avatar = "uiman-icon-1"};
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
