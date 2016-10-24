using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
	[RequireComponent(typeof(TilingProgressBar))]
	[DisallowMultipleComponent]
	public class TilingProgressBarBinder : BinderBase
	{
		protected TilingProgressBar value;
		[HideInInspector]
		public BindingField
			Value = new BindingField ("float");
		public float changeSpeed = 0.01f;
    
		public override void Init (bool forceInit)
		{
			if (CheckInit (forceInit)) {
				value = GetComponent<TilingProgressBar> ();

				SubscribeOnChangedEvent (Value, OnUpdateValue);
			}
		}

		public void OnUpdateValue (object val)
		{
			if (val == null)
				return;

			string tempValue = val.ToString ();

			float valChange = float.Parse (tempValue);

			LeanTween.value (gameObject, value.CurrentValue, valChange, changeSpeed * (value.CurrentValue - valChange) * 100).setOnUpdate (UpdateValue)
            .setEase (LeanTweenType.easeInOutQuart);
		}

		private void UpdateValue (float val)
		{
			value.UpdateValue (val);
		}

		public override void OnDisable ()
		{
			UnSubscribeOnChangedEvent (Value, OnUpdateValue);
		}
	}
}