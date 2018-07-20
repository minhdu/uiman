/// <summary>
/// UnuGames - UIMan - Fast and flexible solution for development and UI management with MVVM pattern
/// @Author: Dang Minh Du
/// @Email: cp.dev.minhdu@gmail.com
/// </summary>
using UnityEngine;
using System.Collections;

namespace UnuGames
{
	[DisallowMultipleComponent]
	public class UIManScreen : UIManBase
	{
		[HideInInspector]
		public bool useBackground = false;
		[HideInInspector]
		public string backgroundType = "";

		public override UIBaseType GetUIBaseType ()
		{
			return UIBaseType.SCREEN;
		}
	}
}
