using UnityEngine;
using System.Collections;
using System.ComponentModel;

namespace UnuGames
{
	public class UIManEnum
	{

	}

	public enum UIMotion
	{
		NONE 			   			= 0,
		HIDDEN 			   			= 1,
		MOVE_RIGHT_TO_LEFT 			= 2,
		MOVE_LEFT_TO_RIGHT 			= 3,
		MOVE_BOTTOM_TO_TOP 			= 4,
		MOVE_TOP_TO_BOTTOM 			= 5,
		CUSTOM_SCRIPT_ANIMATION   	= 6,
		CUSTOM_MECANIM_ANIMATION  	= 7
	}

	public enum UIBaseType
	{
		SCREEN,
		DIALOG
	}

	public enum UIAnimationType
	{
		SHOW,
		HIDE,
		IDLE
	}

	public enum UIState
	{
		SHOW,
		HIDE,
		BUSY
	}
}