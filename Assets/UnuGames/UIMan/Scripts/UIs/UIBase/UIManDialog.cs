/// <summary>
/// UnuGames - UIMan - Fast and flexible solution for development and UI management with MVVM pattern
/// @Author: Dang Minh Du
/// @Email: cp.dev.minhdu@gmail.com
/// @Git: https://github.com/minhdu/
/// @This plugin is completely free for all purpose, please do not remove the author's information
/// </summary>
using UnityEngine;
using System.Collections;
using UnuGames;
using System;

namespace UnuGames
{
	[DisallowMultipleComponent]
	public class UIManDialog : UIManBase
	{

		[HideInInspector]
		public bool
			useCover = true;
		private UICallback mCallbacks;

		public override UIBaseType GetUIBaseType ()
		{
			return UIBaseType.DIALOG;
		}

		public void SetCallbacks (UICallback callbacks)
		{
			mCallbacks = callbacks;
		}

		public void Callback (int index, params object[] args)
		{
			if (mCallbacks == null || index > mCallbacks.Callbacks.Count - 1) {
				if(State != UIState.BUSY && State != UIState.HIDE)
					HideMe ();

				return;
			}

			if(State != UIState.BUSY && State != UIState.HIDE)
				HideMe ();

			if (mCallbacks.Callbacks [index] != null)
				mCallbacks.Callbacks [index] (args);
		}
	}
}