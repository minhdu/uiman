using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace UnuGames
{
	public class UILoading : MonoBehaviour
	{

		public GameObject loadingIndicator;
		public GameObject image;
		public Image loadingCover;
		public Text progressValue;
		public Text tipText;
		AsyncOperation asyncLoading;
		IEnumerator coroutineLoading;
		WWW wwwLoading;
		public bool isLoading = false;
		Action<object[]> loadingCallback;
		object[] loadingCallbackArgs;

		void Update ()
		{
			if (asyncLoading != null && asyncLoading.isDone && isLoading) {
				Hide ();
				if (loadingCallback != null) {
					loadingCallback (loadingCallbackArgs);
					loadingCallback = null;
				}
			}
			if (coroutineLoading != null && !coroutineLoading.MoveNext () && isLoading) {
				Hide ();
				if (loadingCallback != null) {
					loadingCallback (loadingCallbackArgs);
					loadingCallback = null;
				}
			}
			if (wwwLoading != null) {
				if (wwwLoading.isDone && isLoading) {
					Hide ();
					if (loadingCallback != null) {
						loadingCallback (loadingCallbackArgs);
						loadingCallback = null;
					}
				} else {
					ShowValue (Mathf.FloorToInt (wwwLoading.progress * 100).ToString () + "%");
				}
			}
		}

		public void Show (bool showCover = true, bool showImage = false)
		{
			progressValue.fontSize = 20;
			progressValue.text = "Loading...";
			isLoading = true;
			loadingIndicator.SetActive (true);
			loadingCover.enabled = showCover;
			image.SetActive (showImage);
		}
	
		public void Show (AsyncOperation task, bool showCover = true, string tip = "", Action<object[]> callBacks = null, params object[] args)
		{
			progressValue.fontSize = 20;
			progressValue.text = "Loading...";
			ShowTip (tip);
			loadingCallback = callBacks;
			loadingCallbackArgs = args;
			asyncLoading = task;
			isLoading = true;
			loadingIndicator.SetActive (true);
			loadingCover.enabled = showCover;
		}

		public void Show (IEnumerator task, bool showCover = true, string tip = "", Action<object[]> callBacks = null, params object[] args)
		{
			progressValue.fontSize = 20;
			progressValue.text = "Loading...";
			ShowTip (tip);
			loadingCallback = callBacks;
			loadingCallbackArgs = args;
			coroutineLoading = task;
			isLoading = true;
			loadingIndicator.SetActive (true);
			loadingCover.enabled = showCover;
		}

		public void Show (WWW www, bool showCover = true, string tip = "", Action<object[]> callBacks = null, params object[] args)
		{
			progressValue.fontSize = 32;
			ShowTip (tip);
			loadingCallback = callBacks;
			loadingCallbackArgs = args;
			wwwLoading = www;
			isLoading = true;
			loadingIndicator.SetActive (true);
			loadingCover.enabled = showCover;
		}

		public void Hide ()
		{
			asyncLoading = null;
			coroutineLoading = null;
			wwwLoading = null;
			isLoading = false;
			loadingIndicator.SetActive (false);
			loadingCover.enabled = false;
		}

		public void ShowTip (string tip)
		{
			tipText.text = tip;
		}

		public void ShowValue (string value)
		{
			progressValue.text = value;
		}
	}
}
