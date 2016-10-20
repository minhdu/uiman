using UnityEngine;
using System.Collections;
using UnuGames;

public class TestExample2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UIMan.Instance.ShowDialog<UIExample2> (new UICallback(OnClose1, OnClose2), "Message", "Close", Color.green, "uiman-icon-1", false, 1000, "uiman-icon-1.png");
	}
	
	void OnClose1 (object[] args) {
		Debug.Log (args[0].ToString());
	}

	void OnClose2 (object[] args) {
		Debug.Log (args[0].ToString());
	}
}
