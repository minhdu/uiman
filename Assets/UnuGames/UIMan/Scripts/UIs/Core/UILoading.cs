using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace UnuGames
{
	public class UILoading : MonoBehaviour
	{
		public GameObject loadingIcon;
		public Image backgroundImage;
		public Image loadingCover;
		public Text progressValue;
		public Text tipText;

		public bool isLoading = false;
		Action<object[]> loadingCallback;
		object[] loadingCallbackArgs;

		float progress;
		public float Progress {
			get {
				return progress;
			}
		}

		public void Setup (Transform root) {
			transform.SetParent (root, false);
		}

		IEnumerator WaitTask (IEnumerator coroutine) {
			yield return StartCoroutine (coroutine);
			DoCallback ();
			Hide ();
		}

		IEnumerator WaitTask (AsyncOperation asyncTask) {
			while (!asyncTask.isDone) {
				progress = asyncTask.progress;
				if (progressValue.enabled)
					ShowValue (Mathf.FloorToInt (progress).ToString() + "%");
				yield return null;
			}
			DoCallback ();
			Hide ();
		}

		IEnumerator WaitTask (WWW www) {
			while (!www.isDone) {
				progress = www.progress;
				if (progressValue.enabled)
					ShowValue (Mathf.FloorToInt (progress).ToString() + "%");
				yield return null;
			}
			DoCallback ();
			Hide ();
		}

		void DoCallback () {
			if (loadingCallback != null) {
				loadingCallback (loadingCallbackArgs);
				loadingCallback = null;
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
			isLoading = true;
			Setting(showIcon, showCover, showBackground, showProgress, tip);
			StartCoroutine (WaitTask (task));
		}

		public void Show (IEnumerator task, bool showIcon = true, bool showCover = true, bool showBackground = false, bool showProgress = false, string tip = "", Action<object[]> callBacks = null, params object[] args)
		{
			loadingCallback = callBacks;
			loadingCallbackArgs = args;
			isLoading = true;
			Setting(showIcon, showCover, showBackground, showProgress, tip);
			StartCoroutine (WaitTask (task));
		}

		public void Show (WWW www, bool showIcon = true, bool showCover = true, bool showBackground = false, bool showProgress = false, string tip = "", Action<object[]> callBacks = null, params object[] args)
		{
			loadingCallback = callBacks;
			loadingCallbackArgs = args;
			isLoading = true;
			Setting(showIcon, showCover, showBackground, showProgress, tip);
			StartCoroutine (WaitTask (www));
		}

		public void Hide ()
		{
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

		public void HideImage () {
			backgroundImage.enabled = false;
		}

		void OnLoadBackground (Sprite sprite, object[] args) {
			backgroundImage.sprite = sprite;
		}
	}
}
