using UnityEngine;
using UnuGames;
using UnuGames.MVVM;

public partial class UIExample3 : UIManScreen {

#region Fields
	// Your fields here
	public ObservableList<Friend> friends = new ObservableList<Friend>();
#endregion

#region Built-in Events
	public override void OnShow (params object[] args)
	{
		base.OnShow (args);

		friends.Add(new Friend() {Name = "1", Age = 1});
		friends.Add(new Friend() {Name = "2", Age = 2});
		friends.Add(new Friend() {Name = "3", Age = 3});
		friends.Add(new Friend() {Name = "4", Age = 4});

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

	// Add new friend with name & age is friend's count+1
	public void Add () {
		friends.Add (new Friend () { Name = (friends.Count + 1).ToString(), Age = friends.Count + 1 });
	}

	// Remove last friend
	public void Remove () {
		friends.RemoveAt (friends.Count - 1);
	}

	// Insert new friend in frienf list at index = 1 with name & age is friend's count+1
	public void Insert () {
		friends.Insert (1, new Friend () { Name = (friends.Count + 1).ToString(), Age = friends.Count + 1 });
	}

	// Clear all friends
	public void Clear () {
		friends.Clear ();
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
