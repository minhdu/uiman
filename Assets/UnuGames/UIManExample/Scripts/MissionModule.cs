using UnityEngine;
using System.Collections;
using UnuGames;

public class MissionModule : UIManModule<MissionModel> {

	public GameObject[] stars;

	public override MissionModel DataInstance {
		get {
			return base.DataInstance;
		}
		set {
			base.DataInstance = value;
			OnChange (value);
		}
	}

	void OnChange (MissionModel newData) {
		for (int i = 0; i < newData.Star; i++) {
			stars [i].SetActive (true);
		}
		for (int i = newData.Star; i < 3; i++) {
			stars [i].SetActive (false);
		}
	}

	public void OnClick () {
		UIMan.Instance.ShowScreen<UIGameplay> ();
	}
}
