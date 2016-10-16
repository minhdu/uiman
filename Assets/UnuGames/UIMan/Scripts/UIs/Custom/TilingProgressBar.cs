using UnityEngine;

[ExecuteInEditMode]
public class TilingProgressBar : MonoBehaviour {

	public RectTransform foreground;

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
        //if (foreground != null) {
        //    maxWidth = foreground.sizeDelta.x;
        //} else
        {
			Transform fg = transform.FindChild ("FG");
			if(fg != null)
				foreground = fg.GetComponent<RectTransform>();
            //maxWidth = foreground.sizeDelta.x;
		}
	}

	public void UpdateValue (float value) {
		CurrentValue = value;
		float newWidth = value * maxWidth;
		Vector2 newRect = foreground.sizeDelta;
		newRect.x = newWidth;
		foreground.sizeDelta = newRect;
	}

#if UNITY_EDITOR
	void Update () {
		UpdateValue (currentValue);
	}
#endif
}
