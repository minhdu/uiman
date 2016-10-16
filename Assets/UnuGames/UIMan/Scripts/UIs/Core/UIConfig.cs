using UnityEngine;
using System.Collections;

namespace UnuGames
{
	public class UIConfig
	{

		public const string SFX_KEY = "sfx";
		public const string BGM_KEY = "bgm";
		static bool isBGMOn = true;

		static public bool IsBGMOn {
			get { return isBGMOn;}
			set { isBGMOn = value;}
		}

		static bool isSFXOn = true;

		static public bool IsSFXOn {
			get { return isSFXOn;}
			set { isSFXOn = value;}
		}

		static public void Save (string key, string val)
		{
		}
	}
}
