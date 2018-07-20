using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace UnuGames
{
	public class UITweener : MonoBehaviour {

		enum UITweenType {
			VALUE,
			MOVE,
			ALPHA
		}

		UITweenType tweenType;
		float time;
		Action onComplete;
		Action<float> onUpdate;

		bool isRunning = false;
		public bool IsReady {
			get {
				return !isRunning;
			}
		}

		float lastTime;
		float t;

		Vector3 originalPosition;
		Vector3 targetPosition;
		float startValue;
		float endValue;
		float currentValue;

		CanvasGroup canvasGroup;
		Image image;

		// Use this for initialization
		void Run () {
			lastTime = Time.realtimeSinceStartup;
			t = 0;
			originalPosition = transform.localPosition;
			canvasGroup = GetComponent<CanvasGroup> ();
			image = GetComponent<Image> ();
			isRunning = true;
		}
		
		// Update is called once per frame
		void Update () {
			if (!isRunning)
				return;

			float deltaTime = Time.realtimeSinceStartup - lastTime;
			lastTime = Time.realtimeSinceStartup;
			if (time > 0)
				t += deltaTime / time;
			else
				t = 1;
			switch(tweenType) {
			case UITweenType.VALUE:
			case UITweenType.ALPHA:
				currentValue = Mathf.Lerp (startValue, endValue, t);
				if (tweenType == UITweenType.ALPHA) {
					if (canvasGroup != null)
						canvasGroup.alpha = currentValue;
					else if (image != null)
						image.color = new Color (image.color.r, image.color.g, image.color.b, currentValue);
					else
						UnuLogger.LogWarning (gameObject.name + " have no CanvasGroup or Image. TweenAlpha require component that contain alpha value!");
				}
				break;
			case UITweenType.MOVE:
				transform.localPosition = Vector3.Lerp (originalPosition, targetPosition, t);
				break;
			}

			if (t >= 1f) {
				isRunning = false;
				if (onUpdate != null)
					onUpdate (currentValue);
				if (onComplete != null)
					onComplete ();
			} else {
				if (onUpdate != null)
					onUpdate (currentValue);
			}
		}

		public UITweener SetOnComplete (Action onComplete) {
			this.onComplete = onComplete;
			return this;
		}

		public UITweener SetOnUpdate (Action<float> onUpdate) {
			this.onUpdate = onUpdate;
			return this;
		}

		static UITweener DoTween(GameObject targetObject, UITweenType tweenType, float time, params object[] tweenArgs) {

			UITweener tweener = null;

			UITweener[] tweeners = targetObject.GetComponents<UITweener> ();
			for (int i = 0; i < tweeners.Length; i++) {
				if (tweeners [i].IsReady || tweeners[i].tweenType == tweenType) {
					tweener = tweeners [i];
					break;
				}
			}

			if(tweener == null)
				tweener = targetObject.AddComponent<UITweener> ();
			tweener.tweenType = tweenType;
			tweener.time = time;

			switch(tweenType) {
			case UITweenType.VALUE:
			case UITweenType.ALPHA:
				tweener.startValue = (float)tweenArgs [0];
				tweener.endValue = (float)tweenArgs [1];
				break;
			case UITweenType.MOVE:
				tweener.targetPosition = (Vector3)tweenArgs [0];
				break;
			}

			tweener.Run ();
			return tweener;
		}

		static public UITweener Move (GameObject target, float time, Vector3 position) {
			return DoTween(target, UITweenType.MOVE, time, position);
		}

		static public UITweener Value (GameObject target, float time, float startValue, float endValue) {
			return DoTween (target, UITweenType.VALUE, time, startValue, endValue);
		}

		static public UITweener Alpha (GameObject target, float time, float startAlpha, float endAlpha) {
			return DoTween (target, UITweenType.ALPHA, time, startAlpha, endAlpha);
		}
	}
}