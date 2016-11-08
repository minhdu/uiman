using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
	[DisallowMultipleComponent]
	public class BinderTemplate : BinderBase
	{

		[HideInInspector]
		public BindingField yourValue = new BindingField ("Text");
		//Define any field for binding as you want, just copy above fied

		public override void Init (bool forceInit)
		{
			if (CheckInit (forceInit)) {
				// Get view's components here

				SubscribeOnChangedEvent (yourValue, OnUpdateValue);
			}
		}

		public void OnUpdateValue (object newValue)
		{
			if (newValue == null) {
				// Do what you want for null value
				return;
			}

			// Cast newValue into your binding type and assign to view components
		}

		public override void OnDisable ()
		{
			throw new System.NotImplementedException ();
		}
	}
}