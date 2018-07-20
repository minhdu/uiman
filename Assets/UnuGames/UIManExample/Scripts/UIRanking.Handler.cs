using UnityEngine;
using UnuGames;
using UnuGames.MVVM;

public partial class UIRanking : UIManDialog {

#region Fields

	// Your fields here
	public ObservableList<UserModel> users = new ObservableList<UserModel>();
#endregion

#region Built-in Events
	public override void OnShow (params object[] args)
	{
		base.OnShow (args);
		if (args != null && args.Length > 0) {
			int fakeUserCount = (int)args [0];

			users.Clear ();
			for (int i = 0; i < fakeUserCount; i++) {
				users.Add (new UserModel () {
					Name = "User " + i.ToString(),
					Avatar = "user_avatar",
					Level = fakeUserCount-i,
					Rank = i + 1
				});
			}
		}
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
