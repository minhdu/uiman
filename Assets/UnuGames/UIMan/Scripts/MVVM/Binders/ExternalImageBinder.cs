using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
	[RequireComponent(typeof(Image))]
	[DisallowMultipleComponent]
	public class ExternalImageBinder : BinderBase
	{

		protected Image image;
		[HideInInspector]
		public BindingField
			imageValue = new BindingField ("Image");
		[HideInInspector]
		public BindingField
			imageColor = new BindingField ("Color");
		public string resourcePath = "/Images/";

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
			if (newImage == null)
				return;
			ImageFactory.Instance.LoadSprite ("file:///" + Application.persistentDataPath + resourcePath + newImage.ToString (), OnLoadComplete);
		}

		void OnLoadComplete (Sprite sprite)
		{
			image.sprite = sprite;
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