using UnityEngine;
using System.Collections;
using UnuGames;

public class Example1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UIMan.Instance.ShowPopup ("1", "2", "OK", "Cancel", OnOK);
		UIMan.Instance.ShowPopup ("A", "B", "Yes", "No", OnOK);

	}
	
	void OnOK (object[] args) {
		Debug.LogError ("Clicked!");
	}
}
