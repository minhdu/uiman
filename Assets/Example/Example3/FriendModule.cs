using UnityEngine;
using System.Collections;
using UnuGames;

public class FriendModule : UIManModule<Friend> {

	public override Friend DataInstance {
		get {
			return base.DataInstance;
		}
		set {
			base.DataInstance = value;
		}
	}

	public void OnClick () {
		UIMan.Instance.ShowPopup ("Friend Name", DataInstance.Name);
	}
}
