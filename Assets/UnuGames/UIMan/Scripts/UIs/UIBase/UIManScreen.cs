/// <summary>
/// UnuGames - UIMan - Fast and flexible solution for development and UI management with MVVM pattern
/// @Author: Dang Minh Du
/// @Email: cp.dev.minhdu@gmail.com
/// @Git: https://github.com/minhdu/
/// @This plugin is completely free for all purpose, please do not remove the author's information
/// </summary>
using UnityEngine;
using System.Collections;

namespace UnuGames
{
	[DisallowMultipleComponent]
	public class UIManScreen : UIManBase
	{
		public bool useBackground = false;
		public UIScreenBG backgroundType = UIScreenBG.COMMON;

		public override UIBaseType GetUIBaseType ()
		{
			return UIBaseType.SCREEN;
		}
	}
}
