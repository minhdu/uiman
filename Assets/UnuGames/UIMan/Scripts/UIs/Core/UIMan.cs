/// <summary>
/// UnuGames - UIMan - Fast and flexible solution for development and UI management with MVVM pattern
/// @Author: Dang Minh Du
/// @Email: cp.dev.minhdu@gmail.com
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
	[StartupAttribute (StartupType.PREFAB)]
	public class UIMan : SingletonBehaviour<UIMan>
	{
		// Constants
		const string ACTIVITY_INDICATOR_NAME = "UIActivityIndicator";

		// Configuration
		UIManConfig config;

		// Caches
		Dictionary<Type, UIManScreen> screenDict = new Dictionary<Type, UIManScreen> ();
		Dictionary<Type, UIManDialog> dialogDict = new Dictionary<Type, UIManDialog> ();
		Dictionary<Type, string> prefabURLCache = new Dictionary<Type, string> ();

		// Transition queue
		List<UIManScreen> screenQueue = new List<UIManScreen> ();
		Queue<UIDialogQueueData> dialogQueue = new Queue<UIDialogQueueData> ();
		Stack<Type> activeDialog = new Stack<Type> ();

		// Assignable field
		public Transform uiRoot;
		public Transform screenRoot;
		public Transform dialogRoot;
		public Image background;
		RectTransform bgRectTrans;
		public Transform cover;

		// Properties
		public bool IsInDialogTransition { get; set; }

		public bool IsLoadingDialog { get; set; }

		public bool IsLoadingUnityScene { get; set; }

		UIManScreen _mCurrentScreen;

		public UIManScreen CurrentScreen {
			get {
				return _mCurrentScreen;
			}
			set {
				_mCurrentScreen = value;
			}
		}

		public UIManDialog TopDialog {
			get {
				Transform lastTrans = null;
				int lastSibIndex = -1;
				for (int i = 0; i < dialogRoot.transform.childCount; i++) {
					Transform child = dialogRoot.GetChild(i);
					UIManDialog curDlg = child.GetComponent<UIManDialog>();
					if (curDlg != null && curDlg.State == UIState.SHOW && child.GetSiblingIndex () > lastSibIndex) {
						lastTrans = child;
						lastSibIndex = lastTrans.GetSiblingIndex();
					}
				}

				if(lastTrans != null)
					return lastTrans.GetComponent<UIManDialog>();

				return null;
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

		static UIActivity _uiLoading;

		static public UIActivity Loading {
			get {
				if (_uiLoading == null) {
					GameObject loadingObj = ResourceFactory.Load<GameObject> (ACTIVITY_INDICATOR_NAME);
					loadingObj = Instantiate (loadingObj) as GameObject;
					loadingObj.name = ACTIVITY_INDICATOR_NAME;
					_uiLoading = loadingObj.GetComponent<UIActivity> ();
					_uiLoading.Setup (UIMan.Instance.transform);
				}
				return _uiLoading;
			}
		}

		// Initialize
		public override void Init ()
		{
			_uiLoading = GetComponentInChildren<UIActivity> ();
			config = Resources.Load<UIManConfig> ("UIManConfig");
			bgRectTrans = background.GetComponent<RectTransform> ();

			UIManScreen[] screens = GetComponentsInChildren<UIManScreen> ();
			if (screens.Length > 0) {
				for (int i = 0; i < screens.Length; i++) {
					screenDict.Add (screens [i].UIType, screens [i]);
				}
				CurrentScreen = screenDict [screens [screens.Length - 1].UIType];
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
			if (CurrentScreen != null && CurrentScreen.State != UIState.BUSY && CurrentScreen.State != UIState.HIDE)
				CurrentScreen.HideMe ();

			UIManScreen screen = null;
			if (!screenDict.TryGetValue (uiType, out screen)) {
				string prefabPath = Path.Combine (GetUIPrefabPath (uiType, false), uiType.Name);
				ResourceFactory.LoadAsync<GameObject> (prefabPath, PreprocessUI, uiType, seal, args);
				return;
			}

			if (!screen.gameObject.activeInHierarchy)
				screen.gameObject.SetActive (true);


			if (screen.useBackground) {
				background.gameObject.SetActive (true);
				string bgName = "";
				if (!string.IsNullOrEmpty (config.backgroundRootFolder)) {
					int resFolderIndex = config.backgroundRootFolder.LastIndexOf (UIManDefine.RESOURCES_FOLDER);
					if (resFolderIndex > -1)
						bgName = config.backgroundRootFolder.Substring (resFolderIndex + 10);
				}
				bgName = bgName + screen.backgroundType;
				ResourceFactory.LoadAsync<Texture2D> (bgName, SetScreenBackground);
			}

			BringToFront (screenRoot, screen.transform, 2);

			screen.OnShow (args);
			OnShowUI (screen, args);
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
		public void ShowScreen<T> (bool seal, params object[] args)
		{
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

			OnBack (CurrentScreen, beforeScreen, args);

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
				OnHideUI (screen);
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
		public void HideScreen<T> ()
		{
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
			activeDialog.Push (uiType);
			IsInDialogTransition = true;
			dialog.SetCallbacks (callbacks);
			dialog.OnShow (args);
			OnShowUI (dialog, args);
			DoAnimShow (dialog);
		}

		/// <summary>
		/// Shows the dialog.
		/// </summary>
		/// <param name="callbacks">Callbacks.</param>
		/// <param name="args">Arguments.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void ShowDialog<T> (UICallback callbacks, params object[] args)
		{
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
		public void ShowPopup (string title, string message, string button = "OK", Action<object[]> onOK = null, params object[] callbackArgs)
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
				if (dialog.State == UIState.HIDE)
					return;

				if (activeDialog.Count > 0)
					activeDialog.Pop ();

				BringToLayer (dialogRoot, cover, cover.GetSiblingIndex () - 1);
				BringToLayer (dialogRoot, dialog.transform, cover.GetSiblingIndex () - 1);

				UIManDialog prevDialog = null;
				if (activeDialog.Count > 0)
					dialogDict.TryGetValue (activeDialog.Peek (), out prevDialog);
				if (prevDialog != null && prevDialog.useCover) {
					cover.gameObject.SetActive (true);
				} else {
					cover.gameObject.SetActive(false);
				}

				IsInDialogTransition = true;
				dialog.OnHide ();
				OnHideUI (dialog);
				DoAnimHide (dialog);
			} else {
				UnuLogger.LogFormatWarning ("There are no UI of {0} has been show!", uiType.Name);
				return;
			}
		}

		/// <summary>
		/// Hides the dialog.
		/// </summary>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void HideDialog<T> ()
		{
			HideDialog (typeof(T));
		}

		/// <summary>
		/// Loads the unity scene.
		/// </summary>
		/// <param name="name">Name.</param>
		public void LoadUnityScene (string name, Type screen, bool showLoading, params object[] args)
		{
			Instance.cover.gameObject.SetActive (false);
			if (showLoading)
				Loading.Show (SceneManager.LoadSceneAsync (name), true, false, false, false, "", OnLoadUnitySceneComplete, screen, args);
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
			IsLoadingUnityScene = true;
			yield return SceneManager.LoadSceneAsync (name);
			IsLoadingUnityScene = false;
			if(CurrentScreen != null)
				HideScreen (CurrentScreen.UIType);
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
		public void RegisterOnBack (Action<UIManBase, UIManBase, object[]> callback)
		{
			UIEventDispatcher.AddEventListener<UIManBase, UIManBase, object[]> (UIManEvents.UIMan.OnBack, callback);
		}

		/// <summary>
		/// Registers the on show U.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void RegisterOnShowUI (Action<UIManBase, object[]> callback)
		{
			UIEventDispatcher.AddEventListener<UIManBase, object[]> (UIManEvents.UIMan.OnShowUI, callback);
		}

		/// <summary>
		/// Registers the on show user interface complete.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void RegisterOnShowUIComplete (Action<UIManBase, object[]> callback)
		{
			UIEventDispatcher.AddEventListener<UIManBase, object[]> (UIManEvents.UIMan.OnShowUIComplete, callback);
		}

		/// <summary>
		/// Registers the on hide U.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void RegisterOnHideUI (Action<UIManBase> callback)
		{
			UIEventDispatcher.AddEventListener<UIManBase> (UIManEvents.UIMan.OnHideUI, callback);
		}

		/// <summary>
		/// Registers the on hide user interface complete.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void RegisterOnHideUIComplete (Action<UIManBase> callback)
		{
			UIEventDispatcher.AddEventListener<UIManBase> (UIManEvents.UIMan.OnHideUIComplete, callback);
		}

		#endregion

		#region Events

		/// <summary>
		/// Raises the back event.
		/// </summary>
		/// <param name="before">Before.</param>
		/// <param name="after">After.</param>
		/// <param name="args">Arguments.</param>
		void OnBack (UIManBase handlerBefore, UIManBase handlerAfter, params object[] args)
		{
			UIEventDispatcher.TriggerEvent<UIManBase, UIManBase, object[]> (UIManEvents.UIMan.OnBack, handlerBefore, handlerAfter, args);
		}

		/// <summary>
		/// Raises the show UI event.
		/// </summary>
		/// <param name="dialog">Dialog.</param>
		/// <param name="args">Arguments.</param>
		void OnShowUI (UIManBase handler, params object[] args)
		{
			UIEventDispatcher.TriggerEvent<UIManBase, object[]> (UIManEvents.UIMan.OnShowUI, handler, args);
		}

		/// <summary>
		/// Raises the show user interface complete event.
		/// </summary>
		/// <param name="ui">User interface.</param>
		/// <param name="args">Arguments.</param>
		void OnShowUIComplete (UIManBase handler, params object[] args)
		{
			UIEventDispatcher.TriggerEvent<UIManBase, object[]> (UIManEvents.UIMan.OnShowUIComplete, handler, args);
		}

		/// <summary>
		/// Raises the hide U event.
		/// </summary>
		/// <param name="ui">User interface.</param>
		void OnHideUI (UIManBase handler)
		{
			UIEventDispatcher.TriggerEvent<UIManBase> (UIManEvents.UIMan.OnHideUI, handler);
		}

		/// <summary>
		/// Raises the hide user interface complete event.
		/// </summary>
		/// <param name="ui">User interface.</param>
		void OnHideUIComplete (UIManBase handler)
		{
			UIEventDispatcher.TriggerEvent<UIManBase> (UIManEvents.UIMan.OnHideUIComplete, handler);
		}

		#endregion

		#region Utils

		/// <summary>
		/// Gets the user interface prefab UR.
		/// </summary>
		/// <returns>The user interface prefab UR.</returns>
		/// <param name="uiType">User interface type.</param>
		/// <param name="isDialog">If set to <c>true</c> is dialog.</param>
		string GetUIPrefabPath (Type uiType, bool isDialog)
		{
			string url = "";
			if (!prefabURLCache.TryGetValue (uiType, out url)) {
				object[] attributes = uiType.GetCustomAttributes (typeof(UIDescriptor), true);
				if (attributes != null && attributes.Length > 0) {
					url = ((UIDescriptor)attributes [0]).URL;
				} else {
					if (isDialog) {
						url = config.dialogPrefabFolder;
					} else {
						url = config.screenPrefabFolder;
					}

					if (!string.IsNullOrEmpty (url)) {
						int resFolderIndex = url.LastIndexOf (UIManDefine.RESOURCES_FOLDER);
						if (resFolderIndex > -1)
							url = url.Substring (resFolderIndex + 10);
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
				if (!screenDict.ContainsKey (uiType))
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
			if (texture != null) {
				background.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);
			}
			UITweener.Alpha (bgRectTrans.gameObject, 0.25f, 0, 1);
		}

		/// <summary>
		/// Dos the animation show.
		/// </summary>
		/// <param name="ui">User interface.</param>
		void DoAnimShow (UIManBase ui)
		{
			ui.LockInput ();

			if (ui.motionShow == UIMotion.CUSTOM_MECANIM_ANIMATION) { //Custom animation use animator
				ui.GroupCanvas.alpha = 1;
				ui.animRoot.EnableAndPlay (UIManDefine.ANIM_SHOW);
			} else if (ui.motionShow == UIMotion.CUSTOM_SCRIPT_ANIMATION) { //Custom animation use overrided function
				ui.animRoot.Disable ();				
				StartCoroutine (DelayDequeueDialog (ui.AnimationShow (), ui, true));
			} else { // Simple tween
				ui.animRoot.Disable ();
				Vector3 initPos = GetTargetPosition (ui.motionShow, UIManDefine.ARR_SHOW_TARGET_POS);
			
				ui.RectTrans.localPosition = initPos;
				ui.GroupCanvas.alpha = 0;
				// Tween position
				if (ui.motionShow != UIMotion.NONE) {
					UITweener.Move(ui.gameObject, ui.animTime, ui.showPosition);
				}
				UITweener.Alpha(ui.gameObject, ui.animTime, 0f, 1f).SetOnComplete(()=> {
					ui.OnShowComplete ();
					OnShowUIComplete (ui);
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
				ui.animRoot.Disable ();
				StartCoroutine (DelayDequeueDialog (ui.AnimationHide (), ui, false));
			} else { // Simple tween

				ui.animRoot.Disable ();
				Vector3 hidePos = GetTargetPosition (ui.motionHide, UIManDefine.ARR_HIDE_TARGET_POS);
				// Tween position
				if (ui.motionHide != UIMotion.NONE) {
					UITweener.Move(ui.gameObject, ui.animTime, hidePos);
				}
				UITweener.Alpha(ui.gameObject, ui.animTime, 1f, 0f).SetOnComplete(() => {
					ui.RectTrans.anchoredPosition3D = hidePos;
					ui.OnHideComplete ();
					OnHideUIComplete (ui);
					if (ui.GetUIBaseType () == UIBaseType.DIALOG) {
						IsInDialogTransition = false;
						DequeueDialog ();
					}
				});
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
				ui.animRoot.Disable ();
				StartCoroutine (DelayDequeueDialog (ui.AnimationIdle (), ui, false));
			} else { // Simple tween
				ui.animRoot.Disable ();
				if (ui.motionIdle != UIMotion.NONE && ui.motionIdle != UIMotion.HIDDEN) {
					UnuLogger.LogWarning ("UIMan does not support simple tween animation for idle yet!");
				}
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

		public void DestroyUI<T> ()
		{
			Type uiType = typeof(T);
			bool dialog = uiType.BaseType == typeof(UIManDialog) ? true : false;

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

		public T GetHandler<T> () where T : UIManBase
		{
			Type uiType = typeof(T);
			bool dialog = uiType.BaseType == typeof(UIManDialog) ? true : false;
			if (dialog) {
				if (dialogDict.ContainsKey (uiType))
					return (T)(object)dialogDict [uiType];
				else
					return null;
			} else {
				if (screenDict.ContainsKey (uiType))
					return (T)(object)screenDict [uiType];
				else
					return null;
			}
		}

		/// <summary>
		/// Preload the specified uiman.
		/// </summary>
		/// <param name="uiman">Uiman.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void Preload<T> () {
			Preload (typeof(T));
		}

		/// <summary>
		/// Preload the specified uiType.
		/// </summary>
		/// <param name="uiType">User interface type.</param>
		public void Preload (Type uiType) {
			// Ignore if preloaded
			if (uiType.BaseType == typeof(UIManScreen)) {
				if (screenDict.ContainsKey (uiType))
					return;
			} else {
				if (dialogDict.ContainsKey (uiType))
					return;
			}

			// Preload
			string prefabPath = Path.Combine (GetUIPrefabPath (uiType, uiType.BaseType == typeof(UIManDialog)), uiType.Name);
			ResourceFactory.LoadAsync<GameObject> (prefabPath, PreprocessPreload, uiType);
		}

		void PreprocessPreload (GameObject prefab, object[] args) {
			Type uiType = (Type)args [0];
			if (prefab == null) {
				UnuLogger.LogFormatWarning ("UI Error: cannot find {0}, make sure you have put UI prefab in Resources folder!", uiType.Name);
				return;
			}

			GameObject uiObj = Instantiate (prefab) as GameObject;
			uiObj.name = uiType.Name;
			uiObj.GetComponent<CanvasGroup> ().alpha = 0;

			UIManBase uiBase = uiObj.GetComponent<UIManBase> ();
			if (uiBase is UIManScreen) {
				uiBase.Trans.SetParent (screenRoot, false);
				uiBase.RectTrans.localScale = Vector3.one;
				if (!screenDict.ContainsKey (uiType))
					screenDict.Add (uiType, uiBase as UIManScreen);
			} else if (uiBase is UIManDialog) {
				uiBase.Trans.SetParent (dialogRoot, false);
				uiBase.RectTrans.localScale = Vector3.one;
				if(!dialogDict.ContainsKey(uiType))
					dialogDict.Add (uiType, uiBase as UIManDialog);
			}
			uiBase.ForceState(UIState.HIDE);
		}
		#endregion
	}
}
