using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

namespace UnuGames {

	/// <summary>
	/// Data context.
	/// </summary>
	public class DataContext : MonoBehaviour {

		public ContextType type;
		public ViewModelBehaviour viewModel;
		public object model;

		public string propertyName;
		PropertyInfo propertyInfo;

		List<BinderBase> refBinders = new List<BinderBase>();

        public void Clear () {
            viewModel = null;
			propertyName = null;
			propertyInfo = null;
        }

		void Awake () {

			// Subscript for property change event
			GetPropertyInfo ();
			if (propertyInfo != null) {
				model = propertyInfo.GetValue (viewModel, null);
				if (model == null)
					model = ReflectUtils.GetCachedTypeInstance (propertyInfo.PropertyType);
				if(model != null)
					viewModel.SubcriptObjectAction (model);
			}

			// Register binding message for child binders
			BinderBase[] binders = GetComponentsInChildren<BinderBase> ();
			for (int i = 0; i < binders.Length; i++) {
				BinderBase binder = binders [i];
				if (binder.mDataContext == this) {
					refBinders.Add (binder);
					binder.Init ();
				}
			}
		}

		public PropertyInfo GetPropertyInfo () {
#if !UNITY_EDITOR
			if(propertyInfo == null)
				propertyInfo = viewModel.GetCachedType ().GetProperty (propertyName);
#else
			propertyInfo = viewModel.GetCachedType ().GetProperty (propertyName);
#endif
			return propertyInfo;
		}
	}
}
