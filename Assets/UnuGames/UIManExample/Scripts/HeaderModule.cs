using UnityEngine;
using System.Collections;
using UnuGames;

public class HeaderModule : UIManModule<PlayerModel> {

	void Start () {
		DataInstance.Gold = 0;
	}

	public void OnClickHome () {
		UIMan.Instance.BackScreen ();
	}
}
