using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace UnuGames
{
	public class UILoading : MonoBehaviour
	{
		const string LOADING_TEXT = "Loading...";
		public GameObject loadingIcon;
		public Image backgroundImage;
		public Image loadingCover;
		public Text progressValue;
		public Text tipText;

		AsyncOperation asyncLoading;
		IEnumerator coroutineLoading;
		WWW wwwLoading;
		public bool isLoading = false;
		Action<object[]> loadingCallback;
		object[] loadingCallbackArgs;

		public void Setup (Transform root) {
			transform.SetParent (root, false);

		}

		IEnumerator WaitTask (IEnumerator coroutine) {
			yield return StartCoroutine (coroutine);
			DoCallback ();
			Hide ();
		}

		IEnumerator WaitTask (AsyncOperation asyncTask) {
			yield return asyncTask;
			DoCallback ();
			Hide ();
		}

		IEnumerator WaitTask (WWW www) {
			yield return www;
			DoCallback ();
			Hide ();
		}

		void DoCallback () {
			if (loadingCallback != null) {
				loadingCallback (loadingCallbackArgs);
				loadingCallback = null;
			}
		}

		void Update ()
		{
			return;
			if (asyncLoading != null && asyncLoading.isDone && isLoading) {
				if (loadingCallback != null) {
					loadingCallback (loadingCallbackArgs);
					loadingCallback = null;
				}
				Hide ();
			}
			if (wwwLoading != null) {
				if (wwwLoading.isDone && isLoading) {
					if (loadingCallback != null) {
						loadingCallback (loadingCallbackArgs);
						loadingCallback = null;
					}
					Hide ();
				} else {
					ShowValue (Mathf.FloorToInt (wwwLoading.progress * 100).ToString () + "%");
				}
			}
		}

		void Setting (bool showIcon, bool showCover, bool showBackground, bool showProgress, string tip) {
			loadingIcon.SetActive (showIcon);
			loadingCover.enabled = showCover;
			backgroundImage.enabled  = showBackground;
			progressValue.enabled = showProgress;
			tipText.text = tip;
		}

		public void Show (bool showIcon = true, bool showCover = true, bool showBackground = false, bool showProgress = false, string tip = "")
		{
			isLoading = true;
			Setting(showIcon, showCover, showBackground, showProgress, tip);
		}
	
		public void Show (AsyncOperation task, bool showIcon = true, bool showCover = true, bool showBackground = false, bool showProgress = false, string tip = "", Action<object[]> callBacks = null, params object[] args)
		{
			loadingCallback = callBacks;
			loadingCallbackArgs = args;
			asyncLoading = task;
			isLoading = true;
			Setting(showIcon, showCover, showBackground, showProgress, tip);
			StartCoroutine (WaitTask (task));
		}

		public void Show (IEnumerator task, bool showIcon = true, bool showCover = true, bool showBackground = false, bool showProgress = false, string tip = "", Action<object[]> callBacks = null, params object[] args)
		{
			loadingCallback = callBacks;
			loadingCallbackArgs = args;
			coroutineLoading = task;
			isLoading = true;
			Setting(showIcon, showCover, showBackground, showProgress, tip);
			StartCoroutine (WaitTask (task));
		}

		public void Show (WWW www, bool showIcon = true, bool showCover = true, bool showBackground = false, bool showProgress = false, string tip = "", Action<object[]> callBacks = null, params object[] args)
		{
			loadingCallback = callBacks;
			loadingCallbackArgs = args;
			wwwLoading = www;
			isLoading = true;
			Setting(showIcon, showCover, showBackground, showProgress, tip);
			StartCoroutine (WaitTask (www));
		}

		public void Hide ()
		{
			asyncLoading = null;
			coroutineLoading = null;
			wwwLoading = null;
			isLoading = false;
			Setting (false, false, false, false, "");
		}

		public void ShowTip (string tip)
		{
			tipText.text = tip;
		}

		public void ShowValue (string value)
		{
			progressValue.text = value;
		}

		public void ShowImage (Sprite sprite) {
			backgroundImage.enabled = true;
			backgroundImage.sprite = sprite;
		}

		public void ShowImage (string spritePath) {
			backgroundImage.enabled = true;
			ResourceFactory.LoadAsync<Sprite> (spritePath, OnLoadBackground);
		}

		void OnLoadBackground (Sprite sprite, object[] args) {
			backgroundImage.sprite = sprite;
		}
	}
}
