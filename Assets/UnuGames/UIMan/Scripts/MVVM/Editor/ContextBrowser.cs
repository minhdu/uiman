using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using System;

namespace UnuGames.MVVM
{
	public class ContextBrowser
	{

		static BinderBaseEditor curBinderEditor;
		static BindingField curField;
		static string[] members;

		/// <summary>
		/// Browse for field/property
		/// </summary>
		/// <param name="binderEditor"></param>
		/// <param name="field"></param>
		static public void Browse (BinderBaseEditor binderEditor, BindingField field)
		{

			curBinderEditor = binderEditor;
			curField = field;

			members = binderEditor.binder.GetMembers (MemberTypes.Field, MemberTypes.Property);

			FilterPopup.Browse (members, OnMemberSelected);
		}

		static public void Browse (string[] members, Action<string> onSelected)
		{
			FilterPopup.Browse (members, onSelected);
		}

		static public void OnMemberSelected (string member)
		{
			curField.member = member;
			curBinderEditor.Apply ();
			FilterPopup.Close ();
		}
	}
}