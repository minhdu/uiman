using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
[AddComponentMenu("UIMan/Attach/UIFontPrefab")]
[ExecuteInEditMode]
public class UIFontPrefab : MonoBehaviour {

	public GameObject font;
	private UIFont uiFont;
	public GameObject target;
    private Text _mText;

#if UNITY_EDITOR
	public bool refresh = false;
#endif

	void Awake()
	{
		ProcessFont ();
	}

	public void ProcessFont()
	{
		if (target == null)
		{
            _mText = GetComponent<Text>();
		}
		else
		{
            _mText = target.GetComponent<Text>();
		}

		if(_mText != null && font != null)
		{
			if(uiFont == null)
			{
                uiFont = font.GetComponent<UIFont>();
			}

			_mText.font = null;
			_mText.font = uiFont.font;
		}
	}

#if UNITY_EDITOR
	void Update()
	{
		if(refresh)
		{
			refresh = false;
			ProcessFont();
		}
	}
#endif
}
