using UnityEngine;
using System.Collections;
using UnuGames;

public class StartGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UIMan.Instance.ShowScreen<UIMainMenu> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
