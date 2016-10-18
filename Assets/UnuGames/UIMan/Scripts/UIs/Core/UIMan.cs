/// <summary>
/// UnuGames - UIMan - Fast and flexible solution for development and UI management with MVVM pattern
/// @Author: Dang Minh Du
/// @Email: cp.dev.minhdu@gmail.com
/// @Git: https://github.com/minhdu/
/// @This plugin is completely free for all purpose, please do not remove the author's information
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;

namespace UnuGames
{
	[StartupAttribute(StartupType.PREFAB)]
	public class UIMan : SingletonBehaviour<UIMan>
	{

		// Configuration
		UIManConfig config;

		// Caches
		Dictionary<Type, UIManScreen> screenDict = new Dictionary<Type, UIManScreen> ();
		Dictionary<Type, UIManDialog> dialogDict = new Dictionary<Type, UIManDialog> ();
		Dictionary<Type, string> prefabURLCache = new Dictionary<Type, string> ();

		// Transition queue
		List<UIManScreen> screenQueue = new List<UIManScreen> ();
		Queue<UIDialogQueueData> dialogQueue = new Queue<UIDialogQueueData> ();
		Queue<Type> activeDialog = new Queue<Type> ();

		// Assignable field
		public Transform uiRoot;
		public Transform screenRoot;
		public Transform dialogRoot;
		public Image background;
		Transform bgTrans;
		RectTransform bgRectTrans;
		public Transform cover;

		// Properties
		public bool IsInDialogTransition { get; set; }

		public bool IsLoadingDialog { get; set; }

		UIManScreen _mCurrentScreen;
		public UIManScreen CurrentScreen {
			get {
				return _mCurrentScreen;
			}
			set {
				_mCurrentScreen = value;
			}
		}

		string _mCurrentUnityScene;
		public string CurrentUnityScene {
			get {
				return _mCurrentUnityScene;
			}
			set {
				_mCurrentUnityScene = value;
			}
		}

		static UILoading _uiLoading;
		static public UILoading Loading {
			get {
				if (_uiLoading == null)
					_uiLoading = Instance.GetComponentInChildren<UILoading> ();
				return _uiLoading;
			}
		}

		// Initialize
		public override void Init () {
			_uiLoading = GetComponentInChildren<UILoading> ();
			config = Resources.Load<UIManConfig> ("UIManConfig");
			bgTrans = background.GetComponent<Transform> ();
			bgRectTrans = background.GetComponent<RectTransform> ();

			UIManScreen[] screens = GetComponentsInChildren<UIManScreen> ();
			if (screens.Length > 0) {
				for (int i = 0; i < screens.Length; i++) {
					screenDict.Add (screens [i].UIType, screens [i]);
				}
				CurrentScreen = screenDict [screens[screens.Length-1].UIType];
			}
		}

	#region Layer indexer

		/// <summary>
		/// Brings to front.
		/// </summary>
		/// <param name="root">Root.</param>
		/// <param name="ui">User interface.</param>
		/// <param name="step">Step.</param>
		static void BringToFront (Transform root, Transform ui, int step)
		{
			int uiCount = root.transform.childCount;
			ui.SetSiblingIndex (uiCount + step);
		}

		/// <summary>
		/// Brings to layer.
		/// </summary>
		/// <param name="root">Root.</param>
		/// <param name="ui">User interface.</param>
		/// <param name="step">Step.</param>
		static void BringToLayer (Transform root, Transform ui, int layer)
		{
			ui.SetSiblingIndex (layer);
		}

		/// <summary>
		/// Sends to back.
		/// </summary>
		/// <param name="root">Root.</param>
		/// <param name="ui">User interface.</param>
		static void SendToBack (Transform root, Transform ui)
		{
			ui.SetSiblingIndex (0);
		}

	#endregion

	#region Features

		/// <summary>
		/// 
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="seal">If set to <c>true</c> seal.</param>
		/// <param name="args">Arguments.</param>
		void ShowScreen (Type uiType, bool seal, params object[] args)
		{
			if (CurrentScreen != null && CurrentScreen.UIType == uiType)
				return;

			if (CurrentScreen != null && CurrentScreen.State != UIState.BUSY && CurrentScreen.State != UIState.HIDE)
				CurrentScreen.HideMe ();

			UIManScreen screen = null;
			if (!screenDict.TryGetValue (uiType, out screen)) {
				string prefabPath = Path.Combine (GetUIPrefabPath (uiType, false), uiType.Name);
				ResourceFactory.LoadAsync<GameObject> (prefabPath, PreprocessUI, uiType, seal, args);
				return;
			}

			if (screen.useBackground) {
				background.gameObject.SetActive (true);
				string bgName = config.backgroundRootFolder + screen.backgroundType.ToString ();
				ResourceFactory.LoadAsync<Texture2D> (bgName, SetScreenBackground);

				BringToFront (screenRoot, bgTrans, 1);
			}

			BringToFront (screenRoot, screen.transform, 2);

			screen.OnShow (args);
			OnShowUI (uiType, args);
			DoAnimShow (screen);

			CurrentScreen = screen;
			if (!seal)
				screenQueue.Add (screen);
		}

		/// <summary>
		/// Shows the screen.
		/// </summary>
		/// <param name="seal">If set to <c>true</c> seal.</param>
		/// <param name="args">Arguments.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void ShowScreen<T> (bool seal, params object[] args) {
			ShowScreen (typeof(T), seal, args);
		}

		/// <summary>
		/// Shows the screen.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="args">Arguments.</param>
		void ShowScreen (Type uiType, params object[] args)
		{
			ShowScreen (uiType, false, args);
		}

		/// <summary>
		/// Shows the screen.
		/// </summary>
		/// <param name="args">Arguments.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void ShowScreen<T> (params object[] args)
		{
			ShowScreen (typeof(T), args);
		}

		/// <summary>
		/// Backs the screen.
		/// </summary>
		/// <param name="args">Arguments.</param>
		public void BackScreen (params object[] args)
		{
			if (screenQueue.Count <= 1) {
				UnuLogger.LogWarning ("UI Error: There are no scene has been loaded before this scene!");
				return;
			}
        
			CurrentScreen.HideMe ();
			UIManScreen beforeScreen = screenQueue [screenQueue.Count - 2];

			OnBack (CurrentScreen.UIType, beforeScreen.UIType, args);

			screenQueue.RemoveAt (screenQueue.Count - 1);
			ShowScreen (beforeScreen.UIType, true, args);
		}

		/// <summary>
		/// Hides the screen.
		/// </summary>
		/// <param name="content">Content.</param>
		public void HideScreen (Type uiType)
		{
			UIManScreen screen = null;
			if (screenDict.TryGetValue (uiType, out screen)) {
				screen.OnHide ();
				OnHideUI (uiType);
				DoAnimHide (screen);
			} else {
				UnuLogger.LogFormatWarning ("There are no UI of {0} has been show!", uiType.Name);
				return;
			}
		}

		/// <summary>
		/// Hides the screen.
		/// </summary>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void HideScreen<T> () {
			HideScreen (typeof(T));
		}

		/// <summary>
		/// Shows the dialog.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="callbacks">Callbacks.</param>
		/// <param name="args">Arguments.</param>
		void ShowDialog (Type uiType, UICallback callbacks, params object[] args)
		{
			if (IsInDialogTransition || IsLoadingDialog) {
				EnqueueDialog (uiType, UITransitionType.SHOW, args, callbacks);
				return;
			}

			UIManDialog dialog = null;
			if (!dialogDict.TryGetValue (uiType, out dialog)) {
				IsLoadingDialog = true;
				string prefabPath = Path.Combine (GetUIPrefabPath (uiType, true), uiType.Name);
				ResourceFactory.LoadAsync<GameObject> (prefabPath, PreprocessUI, uiType, callbacks, args);
				return;
			}

			if (dialog.IsActive)
				return;

			if (dialog.useCover) {
				cover.gameObject.SetActive (true);
				BringToFront (dialogRoot, cover, 1);
			}

			BringToFront (dialogRoot, dialog.transform, 2);
			activeDialog.Enqueue (uiType);
			IsInDialogTransition = true;
			dialog.SetCallbacks (callbacks);
			dialog.OnShow (args);
			OnShowUI (uiType, args);
			DoAnimShow (dialog);
		}

		/// <summary>
		/// Shows the dialog.
		/// </summary>
		/// <param name="callbacks">Callbacks.</param>
		/// <param name="args">Arguments.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void ShowDialog<T> (UICallback callbacks, params object[] args) {
			ShowDialog (typeof(T), callbacks, args);
		}

		/// <summary>
		/// Shows the dialog.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="args">Arguments.</param>
		public void ShowDialog (Type uiType, params object[] args)
		{
			ShowDialog (uiType, null, args);
		}

		public void ShowDialog<T> (params object[] args)
		{
			ShowDialog (typeof(T), null, args);
		}

		/// <summary>
		/// Display popup as message dialog.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		/// <param name="button">Button.</param>
		/// <param name="onOK">On O.</param>
		/// <param name="callbackArgs">Callback arguments.</param>
		public void ShowPopup (string title, string message, string button = "OK", System.Action<object[]> onOK = null, params object[] callbackArgs)
		{
			UICallback uiCallbacks = new UICallback (onOK);
			ShowDialog<UIPopupDialog> (uiCallbacks, title, message, button, callbackArgs); 
		}

		/// <summary>
		/// Display popup as confirm dialog.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		/// <param name="buttonYes">Button yes.</param>
		/// <param name="buttonNo">Button no.</param>
		/// <param name="onYes">On yes.</param>
		/// <param name="onNo">On no.</param>
		/// <param name="callbackArgs">Callback arguments.</param>
		public void ShowPopup (string title, string message, string buttonYes, string buttonNo, Action<object[]> onYes, Action<object[]> onNo = null, params object[] callbackArgs)
		{
			UICallback uiCallbacks = new UICallback (onYes, onNo);
			ShowDialog<UIPopupDialog> (uiCallbacks, title, message, buttonYes, buttonNo, callbackArgs);
		}

		/// <summary>
		/// Hides the dialog.
		/// </summary>
		/// <param name="content">Content.</param>
		public void HideDialog (Type uiType)
		{
			if (IsInDialogTransition) {
				EnqueueDialog (uiType, UITransitionType.HIDE, null, null);
				return;
			}
			UIManDialog dialog = null;
			if (dialogDict.TryGetValue (uiType, out dialog)) {
				if (activeDialog.Count > 0)
					activeDialog.Dequeue ();
				if (dialog.useCover) {
					UIManDialog prevDialog = null;
					if (dialogQueue.Count > 0)
						dialogDict.TryGetValue (dialogQueue.Peek ().UIType, out prevDialog);
					if (activeDialog.Count > 0 && prevDialog != null && prevDialog.useCover) {
						BringToLayer (dialogRoot, cover, cover.GetSiblingIndex () - 1);
					} else {
						cover.gameObject.SetActive (false);
					}
				}
				IsInDialogTransition = true;
				dialog.OnHide ();
				OnHideUI (uiType);
				DoAnimHide (dialog);
			} else {
				UnuLogger.LogFormatWarning ("There are no UI of {0} has been show!", uiType.Name);
				return;
			}
		}

		/// <summary>
		/// Loads the unity scene.
		/// </summary>
		/// <param name="name">Name.</param>
		void LoadUnityScene (string name, Type screen, bool showLoading, params object[] args)
		{
			Instance.cover.gameObject.SetActive (false);
			if (showLoading)
				Loading.Show (SceneManager.LoadSceneAsync (name), true, "", OnLoadUnitySceneComplete, screen, args);
			else
				StartCoroutine (LoadUnityScene (name, screen, args));
		}

		/// <summary>
		/// Loads the unity scene.
		/// </summary>
		/// <returns>The unity scene.</returns>
		/// <param name="name">Name.</param>
		/// <param name="screen">Screen.</param>
		/// <param name="args">Arguments.</param>
		IEnumerator LoadUnityScene (string name, Type screen, params object[] args)
		{
			yield return SceneManager.LoadSceneAsync (name);
			OnLoadUnitySceneComplete (screen, args);
		}

		/// <summary>
		/// Raises the load unity scene complete event.
		/// </summary>
		/// <param name="args">Arguments.</param>
		void OnLoadUnitySceneComplete (params object[] args)
		{
			StartCoroutine (WaitForTransitionComplete (args));
		}

		IEnumerator WaitForTransitionComplete (params object[] args)
		{
			while (CurrentScreen != null && CurrentScreen.State != UIState.HIDE) {
				yield return null;
			}
			Type screen = (Type)args [0];
			object[] screenArgs = null;
			if (args.Length > 1)
				screenArgs = (object[])args [1];
			Instance.ShowScreen (screen, screenArgs);
		}

		/// <summary>
		/// Sets the native loading.
		/// </summary>
		/// <param name="isLoading">If set to <c>true</c> is loading.</param>
		static public void SetNativeLoading (bool isLoading)
		{
#if UNITY_IOS || UNITY_ANDROID
		if(isLoading)
		Handheld.StartActivityIndicator();
		else
		Handheld.StopActivityIndicator();
#endif
		}

		/// <summary>
		/// Registers the on back.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void RegisterOnBack (System.Action<Type, Type, object[]> callback)
		{
			UIEventDispatcher.AddEventListener<Type, Type, object[]> (UIManEvents.UIMan.OnBack, callback);
		}

		/// <summary>
		/// Registers the on show U.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void RegisterOnShowUI (System.Action<Type, object[]> callback)
		{
			UIEventDispatcher.AddEventListener<Type, object[]> (UIManEvents.UIMan.OnShowUI, callback);
		}

		/// <summary>
		/// Registers the on show user interface complete.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void RegisterOnShowUIComplete (System.Action<Type, object[]> callback)
		{
			UIEventDispatcher.AddEventListener<Type, object[]> (UIManEvents.UIMan.OnShowUIComplete, callback);
		}

		/// <summary>
		/// Registers the on hide U.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void RegisterOnHideUI (System.Action<Type> callback)
		{
			UIEventDispatcher.AddEventListener<Type> (UIManEvents.UIMan.OnHideUI, callback);
		}

		/// <summary>
		/// Registers the on hide user interface complete.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void RegisterOnHideUIComplete (System.Action<Type> callback)
		{
			UIEventDispatcher.AddEventListener<Type> (UIManEvents.UIMan.OnHideUIComplete, callback);
		}

	#endregion

	#region Events
	
		/// <summary>
		/// Raises the back event.
		/// </summary>
		/// <param name="before">Before.</param>
		/// <param name="after">After.</param>
		/// <param name="args">Arguments.</param>
		void OnBack (Type before, Type after, params object[] args)
		{
			UIEventDispatcher.TriggerEvent<Type, Type, object[]> (UIManEvents.UIMan.OnBack, before, after, args);
		}

		/// <summary>
		/// Raises the show UI event.
		/// </summary>
		/// <param name="dialog">Dialog.</param>
		/// <param name="args">Arguments.</param>
		void OnShowUI (Type ui, params object[] args)
		{
			UIEventDispatcher.TriggerEvent<Type, object[]> (UIManEvents.UIMan.OnShowUI, ui, args);
		}
	
		/// <summary>
		/// Raises the show user interface complete event.
		/// </summary>
		/// <param name="ui">User interface.</param>
		/// <param name="args">Arguments.</param>
		void OnShowUIComplete (Type ui, params object[] args)
		{
			UIEventDispatcher.TriggerEvent<Type, object[]> (UIManEvents.UIMan.OnShowUIComplete, ui, args);
		}

		/// <summary>
		/// Raises the hide U event.
		/// </summary>
		/// <param name="ui">User interface.</param>
		void OnHideUI (Type ui)
		{
			UIEventDispatcher.TriggerEvent<Type> (UIManEvents.UIMan.OnHideUI, ui);
		}

		/// <summary>
		/// Raises the hide user interface complete event.
		/// </summary>
		/// <param name="ui">User interface.</param>
		void OnHideUIComplete (Type ui)
		{
			UIEventDispatcher.TriggerEvent<Type> (UIManEvents.UIMan.OnHideUIComplete, ui);
		}

	#endregion

	#region Utils

		/// <summary>
		/// Gets the user interface prefab UR.
		/// </summary>
		/// <returns>The user interface prefab UR.</returns>
		/// <param name="uiType">User interface type.</param>
		/// <param name="isDialog">If set to <c>true</c> is dialog.</param>
		string GetUIPrefabPath (Type uiType, bool isDialog) {
			string url = "";
			if (!prefabURLCache.TryGetValue (uiType, out url)) {
				object[] attributes = uiType.GetCustomAttributes(typeof(UIDescriptor), true);
				if (attributes != null && attributes.Length > 0) {
					url = ((UIDescriptor)attributes [0]).URL;
				} else {
					if (isDialog) {
						url = config.dialogPrefabFolder;
					} else {
						url = config.screenPrefabFolder;
					}

					if(!string.IsNullOrEmpty(url)) {
						int resFolderIndex = url.LastIndexOf ("Resources/");
						if (resFolderIndex > -1)
							url = url.Substring (resFolderIndex+10);
					}
				}
				prefabURLCache.Add (uiType, url);
			}

			return url;
		}

		/// <summary>
		/// Preprocesses the UI.
		/// </summary>
		/// <param name="prefab">Prefab.</param>
		/// <param name="args">Arguments.</param>
		void PreprocessUI (GameObject prefab, object[] args)
		{
			Type uiType = (Type)args [0];
			if (prefab == null) {
				UnuLogger.LogFormatWarning ("UI Error: cannot find {0}, make sure you have put UI prefab in Resources folder!", uiType.Name);
				return;
			}

			GameObject uiObj = Instantiate (prefab) as GameObject;
			uiObj.name = uiType.Name;

			UIManBase uiBase = uiObj.GetComponent<UIManBase> ();
			if (uiBase is UIManScreen) {
				uiBase.Trans.SetParent (screenRoot, false);
				uiBase.RectTrans.localScale = Vector3.one;
				if(!screenDict.ContainsKey(uiType))
					screenDict.Add (uiType, uiBase as UIManScreen);
				bool seal = (bool)args [1];
				object[] param = (object[])args [2];
				ShowScreen (uiType, seal, param);
			} else if (uiBase is UIManDialog) {
				uiBase.Trans.SetParent (dialogRoot, false);
				uiBase.RectTrans.localScale = Vector3.one;
				dialogDict.Add (uiType, uiBase as UIManDialog);
				UICallback callbacks = (UICallback)args [1];
				object[] param = (object[])args [2];
				IsLoadingDialog = false;
				ShowDialog (uiType, callbacks, param);
			}
		}

		/// <summary>
		/// Sets the screen background.
		/// </summary>
		/// <param name="texture">Texture.</param>
		void SetScreenBackground (Texture2D texture, object[] args)
		{
			background.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);
			LeanTween.alpha (bgRectTrans, 1, 0.25f).setEase (LeanTweenType.linear);
		}

		/// <summary>
		/// Dos the animation show.
		/// </summary>
		/// <param name="ui">User interface.</param>
		void DoAnimShow (UIManBase ui)
		{
			ui.LockInput ();

			if (ui.motionShow == UIMotion.CUSTOM_MECANIM_ANIMATION) { //Custom animation use animator
				ui.animRoot.EnableAndPlay (UIManDefine.ANIM_SHOW);
			} else if (ui.motionShow == UIMotion.CUSTOM_SCRIPT_ANIMATION) { //Custom animation use overrided function
				StartCoroutine (DelayDequeueDialog (ui.AnimationShow (), ui, true));
			} else { // Simple tween
				Vector3 initPos = GetTargetPosition (ui.motionShow, UIManDefine.ARR_SHOW_TARGET_POS);
			
				ui.RectTrans.localPosition = initPos;
				ui.GroupCanvas.alpha = 0;
			
				// Tween position
				if (ui.motionShow != UIMotion.NONE) {
					LeanTween.move (ui.RectTrans, ui.showPosition, ui.animTime)
					.setEase (LeanTweenType.linear);
				}
			
				// Tween alpha
				LeanTween.value (ui.gameObject, ui.UpdateAlpha, 0.0f, 1.0f, ui.animTime)
				.setEase (LeanTweenType.linear)
					.setOnComplete (show => {
					ui.OnShowComplete ();
					OnShowUIComplete (ui.UIType);
					if (ui.GetUIBaseType () == UIBaseType.DIALOG) {
						IsInDialogTransition = false;
					}
					ui.UnlockInput ();
					DoAnimIdle (ui);
				});
			}
		}

		/// <summary>
		/// Dos the animation hide.
		/// </summary>
		/// <param name="ui">User interface.</param>
		void DoAnimHide (UIManBase ui)
		{
			ui.LockInput ();
			if (ui.motionHide == UIMotion.CUSTOM_MECANIM_ANIMATION) { //Custom animation use animator
				ui.animRoot.EnableAndPlay (UIManDefine.ANIM_HIDE);
			} else if (ui.motionHide == UIMotion.CUSTOM_SCRIPT_ANIMATION) { //Custom animation use overrided function
				StartCoroutine (DelayDequeueDialog (ui.AnimationHide (), ui, false));
			} else { // Simple tween

				// Stop current tween if exist
				LeanTween.cancel (ui.gameObject);
				LeanTween.cancel (bgRectTrans.gameObject);

				Vector3 hidePos = GetTargetPosition (ui.motionHide, UIManDefine.ARR_HIDE_TARGET_POS);
			
				// Tween position
				if (ui.motionHide != UIMotion.NONE) {
					LeanTween.move (ui.RectTrans, hidePos, ui.animTime).setEase (LeanTweenType.linear);
				}
			
				// Tween alpha
				LeanTween.value (ui.gameObject, ui.UpdateAlpha, 1.0f, 0.0f, ui.animTime)
				.setEase (LeanTweenType.linear)
					.setOnComplete (hide => {
					ui.RectTrans.anchoredPosition3D = hidePos;
					ui.OnHideComplete ();
					OnHideUIComplete (ui.UIType);
					if (ui.GetUIBaseType () == UIBaseType.DIALOG) {
						IsInDialogTransition = false;
						DequeueDialog ();
					}
				});
			
				LeanTween.alpha (bgRectTrans, 0, 0.25f).setEase (LeanTweenType.linear);
			}
		}

		/// <summary>
		/// Dos the animation idle.
		/// </summary>
		/// <param name="ui">User interface.</param>
		public void DoAnimIdle (UIManBase ui)
		{
			if (ui.motionIdle == UIMotion.CUSTOM_MECANIM_ANIMATION) { //Custom animation use animator
				ui.animRoot.EnableAndPlay (UIManDefine.ANIM_IDLE);
			} else if (ui.motionHide == UIMotion.CUSTOM_SCRIPT_ANIMATION) { //Custom animation use overrided function
				StartCoroutine (DelayDequeueDialog (ui.AnimationIdle (), ui, false));
			} else { // Simple tween
				//UnuUnuLogger.Log("UIMan does not support simple tween animation for idle yet!");
			}
		}

		/// <summary>
		/// Gets the target position.
		/// </summary>
		/// <returns>The target position.</returns>
		/// <param name="motion">Motion.</param>
		/// <param name="arrTargetPosition">Arr target position.</param>
		Vector3 GetTargetPosition (UIMotion motion, Vector3[] arrTargetPosition)
		{
			return arrTargetPosition [(int)motion];
		}

		/// <summary>
		/// Enqueues the dialog.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="transition">Transition.</param>
		/// <param name="args">Arguments.</param>
		/// <param name="callback">Callback.</param>
		void EnqueueDialog (Type uiType, UITransitionType transition, object[] args, UICallback callback)
		{
			UIDialogQueueData data = new UIDialogQueueData (uiType, transition, args, callback);
			dialogQueue.Enqueue (data);
		}

		/// <summary>
		/// Delaies the dequeue dialog.
		/// </summary>
		/// <returns>The dequeue dialog.</returns>
		/// <param name="coroutine">Coroutine.</param>
		/// <param name="ui">User interface.</param>
		/// <param name="resetDialogTransitionStatus">If set to <c>true</c> reset dialog transition status.</param>
		IEnumerator DelayDequeueDialog (IEnumerator coroutine, UIManBase ui, bool resetDialogTransitionStatus)
		{
			yield return StartCoroutine (coroutine);
			IsInDialogTransition = false;
			ui.UnlockInput ();
			ui.OnHideComplete ();
			if (ui.GetUIBaseType () == UIBaseType.DIALOG && !resetDialogTransitionStatus)
				DequeueDialog ();
		}

		/// <summary>
		/// Dequeues the dialog.
		/// </summary>
		public void DequeueDialog ()
		{
			if (dialogQueue.Count > 0) {
				UIDialogQueueData transition = dialogQueue.Dequeue ();
				if (transition.TransitionType == UITransitionType.SHOW) {
					ShowDialog (transition.UIType, transition.Callbacks, transition.Args);
				} else if (transition.TransitionType == UITransitionType.HIDE) {
					HideDialog (transition.UIType);					
				}
			}
		}

		public bool IsShowingDialog<T> ()
		{
			Type uiType = typeof(T);
			foreach (KeyValuePair<Type, UIManDialog> dlg in dialogDict) {
				if (dlg.Key == uiType && dlg.Value.IsActive)
					return true;
			}
			return false;
		}

		public void DestroyUI<T> (bool dialog)
		{
			Type uiType = typeof(T);
			UIManBase ui = null;
			if (dialog) {
				if (dialogDict.ContainsKey (uiType)) {
					ui = dialogDict [uiType];
					dialogDict.Remove (uiType);
				}
			} else {
				if (screenDict.ContainsKey (uiType)) {
					ui = screenDict [uiType];
					screenDict.Remove (uiType);
				}
			}

			if (ui != null) {
				Destroy (ui.gameObject);
			}
		}

		public UIManBase GetHandler<T> (bool dialog) {
			Type uiType = typeof(T);
			if (dialog) {
				if (dialogDict.ContainsKey (uiType))
					return dialogDict [uiType];
				else
					return null;
			} else {
				if (screenDict.ContainsKey (uiType))
					return screenDict [uiType];
				else
					return null;
			}
		}

	#endregion
	}
}
