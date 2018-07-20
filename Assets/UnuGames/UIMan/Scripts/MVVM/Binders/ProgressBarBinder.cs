using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
	[RequireComponent(typeof(UIProgressBar))]
	[DisallowMultipleComponent]
	public class ProgressBarBinder : BinderBase
	{
		protected UIProgressBar value;
		[HideInInspector]
		public BindingField Value = new BindingField ("float");
		public bool tweenValueChange;
		public float changeTime = 0.1f;
    
		public override void Init (bool forceInit)
		{
			if (CheckInit (forceInit)) {
				value = GetComponent<UIProgressBar> ();
				SubscribeOnChangedEvent (Value, OnUpdateValue);
			}
		}

		public void OnUpdateValue (object val)
		{
			if (val == null)
				return;

			string tempValue = val.ToString ();

			float valChange = float.Parse (tempValue);
			float time = 0;
			if (tweenValueChange) {
				time = changeTime;
			}
			UITweener.Value (gameObject, time, value.CurrentValue, valChange).SetOnUpdate (UpdateValue);
		}

		private void UpdateValue (float val)
		{
			value.UpdateValue (val);
		}
	}
}