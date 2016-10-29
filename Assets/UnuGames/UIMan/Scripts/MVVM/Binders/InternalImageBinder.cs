using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
	[RequireComponent(typeof(Image))]
	[DisallowMultipleComponent]
	public class InternalImageBinder : BinderBase
	{

		protected Image image;
		[HideInInspector]
		public BindingField imageValue = new BindingField ("Image");
		[HideInInspector]
		public BindingField imageColor = new BindingField ("Color");
		public string resourcePath = "Images/";

		public bool autoCorrectSize;
		public bool zeroAlphaOnImageNull;

		public override void Init (bool forceInit)
		{
			if (CheckInit (forceInit)) {
				image = GetComponent<Image> ();

				SubscribeOnChangedEvent (imageValue, OnUpdateImage);
				SubscribeOnChangedEvent (imageColor, OnUpdateColor);
			}
		}

		public void OnUpdateImage (object newImage)
		{
			image.color = Color.white;
			image.sprite = ResourceFactory.Load<Sprite> (resourcePath + newImage.ToString ());
			if (autoCorrectSize)
				image.SetNativeSize ();
			if(zeroAlphaOnImageNull && image.sprite == null)
				image.color = new Color (image.color.r, image.color.g, image.color.b, 0);
		}

		public void OnUpdateColor (object newColor)
		{
			if (newColor == null)
				return;
			try {
				image.color = (Color)newColor;
			} catch {
				UnuLogger.LogWarning ("Binding field is not a color!");
			}
		}

		public override void OnDisable ()
		{
			UnSubscribeOnChangedEvent (imageValue, OnUpdateImage);
		}
	}
}