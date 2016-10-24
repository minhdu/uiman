using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
	[RequireComponent(typeof(Image))]
	[DisallowMultipleComponent]
	public class ImageFillAmountBinder : BinderBase
	{
		protected Image image;
		[HideInInspector]
		public BindingField
			Value = new BindingField ("float");
		float timeChangeValue = 0.75f;

		public override void Init (bool forceInit)
		{
			if (CheckInit (forceInit)) {
				image = GetComponent<Image> ();

				SubscribeOnChangedEvent (Value, OnUpdateValue);
			}
		}

		public void OnUpdateValue (object val)
		{
			if (val == null)
				return;

			float valChange = (float)val;

			LeanTween.value (this.gameObject, image.fillAmount, valChange, timeChangeValue).setOnUpdate (UpdateValue).setEase (LeanTweenType.easeInOutQuart);
		}

		private void UpdateValue (float val)
		{
			image.fillAmount = val;
		}

		public override void OnDisable ()
		{
			UnSubscribeOnChangedEvent (Value, OnUpdateValue);
		}
	}
}