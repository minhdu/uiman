using UnityEngine;
using System.Collections;
using UnuGames;

public class Example1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UIMan.Instance.ShowPopup ("Title1", "Content1", "OK", "Cancel", OnOK, null, "Click1");
		UIMan.Instance.ShowPopup ("Title2", "Content2", "Yes", "No", OnOK, null, "Click2");
		UIMan.Instance.ShowPopup ("Confirm", "Message");
	}
	
	void OnOK (object[] args) {
		Debug.Log (args[0].ToString());
	}
}
