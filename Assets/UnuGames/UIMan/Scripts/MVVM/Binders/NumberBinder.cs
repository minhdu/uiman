
using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
	[RequireComponent(typeof(Text))]
	[DisallowMultipleComponent]
	public class NumberBinder : BinderBase
	{
		protected Text text;
		[HideInInspector]
		public BindingField
			textValue = new BindingField ("Text");
		public string Format;
		public float timeChange = 0.25f;

		public override void Init (bool forceInit)
		{
			if (CheckInit (forceInit)) {
				text = GetComponent<Text> ();
				SubscribeOnChangedEvent (textValue, OnUpdateText);
			}
		}

		public void OnUpdateText (object newText)
		{
			if (newText == null)
				return;

			double val = 0;
			double.TryParse (text.text, out val);

			double change = 0;
			double.TryParse (newText.ToString (), out change);

			LeanTween.value (gameObject, (float)val, (float)change, timeChange).setOnUpdate (UpdateText).setOnComplete (() =>
			{
				text.text = newText.ToString ();
			});
		}

		void UpdateText (float val)
		{
			long valueChange = (long)val;

			if (string.IsNullOrEmpty (Format)) {
				text.text = valueChange.ToString ();
			} else {
				text.text = string.Format (Format, valueChange.ToString ());
			}
		}

		public override void OnDisable ()
		{
		}
	}
}