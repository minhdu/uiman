using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UnuGames.MVVM
{
	public class VisibleBinder : BinderBase
	{
		public List<GameObject> enableOnTrue = new List<GameObject> ();
		public List<GameObject> disableOnTrue = new List<GameObject> ();
		[HideInInspector]
		public BindingField
			Value = new BindingField ("bool");

		public override void Init (bool forceInit)
		{
			if (CheckInit (forceInit)) {
				SubscribeOnChangedEvent (Value, OnUpdateValue);
			}
		}

		public void OnUpdateValue (object val)
		{
			if (val == null)
				return;

			bool valChange = (bool)val;

			if (enableOnTrue != null && enableOnTrue.Count > 0) {
				for (int i = 0; i < enableOnTrue.Count; i++) {
					enableOnTrue [i].SetActive (valChange);
				}
			}

			if (disableOnTrue != null && disableOnTrue.Count > 0) {
				for (int i = 0; i < disableOnTrue.Count; i++) {
					disableOnTrue [i].SetActive (!valChange);
				}
			}
		}
	}
}