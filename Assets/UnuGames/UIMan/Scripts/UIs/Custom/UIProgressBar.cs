using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIProgressBar : MonoBehaviour {

	public RectTransform foreground;
	public RectTransform thumb;
	public Image.Type type;
	Image foreGroundImg;

	[SerializeField]
	float maxWidth;

	[SerializeField]
	[Range(0, 1)]
	float currentValue;
	public float CurrentValue {
		get {
			return currentValue;
		}
		set {
			currentValue = value;
		}
	}

	void Awake () {
		Transform fg = transform.FindChild ("FG");
		if(fg != null)
			foreground = fg.GetComponent<RectTransform>();
		foreGroundImg = foreground.GetComponent<Image> ();
		Transform fgThumb = transform.FindChild ("FGThumb");
		if(fgThumb != null)
			foreground = fg.GetComponent<RectTransform>();
		if(fgThumb != null)
			thumb = fgThumb.GetComponent<RectTransform> ();
	}

	public void UpdateValue (float value) {
		CurrentValue = value;
		if (type == Image.Type.Filled) {
			foreGroundImg.fillAmount = value;
		} else {
			float newWidth = value * maxWidth;
			Vector2 newRect = foreground.sizeDelta;
			newRect.x = newWidth;
			foreground.sizeDelta = newRect;
		}

		if (thumb != null) {
			float newWidth = value * maxWidth;
			Vector2 newRect = thumb.sizeDelta;
			newRect.x = newWidth;
			thumb.sizeDelta = newRect;
		}
	}

#if UNITY_EDITOR
	void Update () {
		if (!Application.isPlaying) {
			UpdateValue (currentValue);
		}
	}
#endif
}
