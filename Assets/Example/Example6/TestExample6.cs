using UnityEngine;
using System.Collections;
using UnuGames;

public class TestExample6 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UIMan.Instance.RegisterOnShowUI (OnAnyUIShow);
	}

	void OnAnyUIShow (UIManBase handler, object[] args) {
		Debug.Log (handler.UIType + " is show!!!!");
	}

	public void ShowExample2 () {
		UIMan.Instance.ShowDialog<UIExample2> (new UICallback(OnClose1, OnClose2), "Message", "Close", Color.green, "uiman-icon-1", false, 1000, "uiman-icon-1.png");
	}

	void OnClose1 (object[] args) {
		Debug.Log (args[0].ToString());
	}

	void OnClose2 (object[] args) {
		Debug.Log (args[0].ToString());
	}

	public void ShowPopup () {
		UIMan.Instance.ShowPopup ("This popup", "This is message");
	}

	public void GetHandler () {
		((UIExample2)UIMan.Instance.GetHandler<UIExample2> ()).Log ();
	}
}
