using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
	[RequireComponent(typeof(InputField))]
	[DisallowMultipleComponent]
	public class InputFieldBinder : BinderBase
	{

		protected InputField input;
		[HideInInspector]
		public BindingField
			textValue = new BindingField ("Text");

		public override void Init (bool forceInit)
		{
			if (CheckInit (forceInit)) {
				input = GetComponent<InputField> ();

				SubscribeOnChangedEvent (textValue, OnUpdateText);
			}
		}

		public void OnUpdateText (object newText)
		{
			//if(newText == null)
			//	return;
			//input.text = newText.ToString();
		}

		public override void OnDisable ()
		{
			UnSubscribeOnChangedEvent (textValue, OnUpdateText);
		}

		string oldText = "";

		void Update ()
		{
			if (input.text != oldText) {
				oldText = input.text;
				if (oldText.Contains ("\t")) {
					oldText.Replace ("\t", string.Empty);
				}
				input.text = oldText;
				SetValue (textValue.member, oldText);
			}
		}

		public void EndEdit ()
		{
			input.text = oldText;
		}
	}
}