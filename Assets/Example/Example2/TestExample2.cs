using UnityEngine;
using System.Collections;
using UnuGames;

public class TestExample2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UIMan.Instance.ShowDialog<UIExample2> ("Message", "Close", Color.green);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
