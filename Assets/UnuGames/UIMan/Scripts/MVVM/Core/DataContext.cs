using System;
using System.Reflection;
using UnityEngine;

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

        public void Clear () {
            viewModel = null;
			propertyName = null;
			propertyInfo = null;
        }

		void Awake () {
			GetPropertyInfo ();
			if (propertyInfo != null) {
				model = propertyInfo.GetValue (viewModel, null);
				if(model != null)
					viewModel.SubcriptObjectAction (model);
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
