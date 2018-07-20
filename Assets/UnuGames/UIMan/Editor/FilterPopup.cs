using UnityEngine;
using UnityEditor;
using System;

namespace UnuGames
{
	public class FilterPopup : EditorWindow
	{

		static private string[] mItems;

		static private Action<string> OnSelected { get; set; }

		const int MEMBER_HEIGHT = 37;
		static Rect inspectorRect;
		static Vector2 inspectorPos;
		static Vector2 scrollPosition;
		static UISearchField searchField;
		static ListView listView;

		/// <summary>
		/// Show window as dropdown popup
		/// </summary>
		private static void Popup ()
		{
			FilterPopup fp = ScriptableObject.CreateInstance (typeof(FilterPopup)) as FilterPopup;

			int minHeight = mItems.Length * MEMBER_HEIGHT + MEMBER_HEIGHT * 2;
			int bestHeight = (int)(Screen.currentResolution.height / 2.5f);
			if (minHeight > bestHeight)
				minHeight = bestHeight;

			inspectorPos = GUIUtility.GUIToScreenPoint (new Vector2 (inspectorRect.x, inspectorRect.y));
			fp.ShowAsDropDown (new Rect (inspectorPos, inspectorRect.size), new Vector2 (inspectorRect.width, minHeight));
		}

		/// <summary>
		/// Browse for field/property
		/// </summary>
		/// <param name="binderEditor"></param>
		/// <param name="field"></param>
		static public void Browse (string[] items, Action<string> onSelected)
		{

			searchField = new UISearchField (Filter, null, null);
			OnSelected = onSelected;
			mItems = items;

			if (items != null && items.Length > 0)
				Popup ();
		}

		void OnGUI ()
		{
			if (Event.current.keyCode == KeyCode.Escape)
				Close ();

			if (mItems == null)
				return;

			if (listView == null)
				listView = new ListView ();

			//Search field
			searchField.Draw ();
			listView.SetData (mItems, true, OnSelected, searchField.KeyWord, this);
			listView.Draw ();
		}

		/// <summary>
		/// Set the window's rectangle
		/// </summary>
		/// <param name="rect"></param>
		static public void SetPopupRect (Rect rect)
		{
			inspectorRect = rect;
		}

		static public void SetShowPosition ()
		{
			SetPopupRect (new Rect (GUILayoutUtility.GetLastRect().x, Event.current.mousePosition.y, GUILayoutUtility.GetLastRect ().width, 10));
		}

		/// <summary>
		/// Filter items by keyword
		/// </summary>
		/// <param name="keyWord"></param>
		static void Filter (string keyWord)
		{

		}

		new static public void Close ()
		{
			if (listView != null && listView.Window != null)
				listView.Window.Close ();
		}
	}
}