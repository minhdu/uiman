using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

namespace UnuGames {

	/// <summary>
	/// Data context.
	/// </summary>
	public class DataContext : MonoBehaviour {

#region DataContext Factory
		static List<DataContext> contextsList = new List<DataContext> ();
		static public void Rebind (object modelInstance) {
			for (int i = 0; i < contextsList.Count; i++) {
				DataContext context = contextsList [i];
				if (context.type == ContextType.PROPERTY && context.viewModel is IModule) {
					IModule module = context.viewModel as IModule;
					ObservableModel model = (ObservableModel)modelInstance;
					if(module.OriginalData.Equals (modelInstance)) {
						context.model = model;
						context.RegisterBindingMessage (true);
					}
				}
			}
		}
#endregion

#region Instance

		public ContextType type;
		public ViewModelBehaviour viewModel;
		public object model;

		public string propertyName;
		PropertyInfo propertyInfo;
		public PropertyInfo PropertyInfo {
			get {
				return propertyInfo;
			}
		}

		List<BinderBase> refBinders = new List<BinderBase>();

        public void Clear () {
            viewModel = null;
			propertyName = null;
			propertyInfo = null;
        }

		void Awake () {

			if(!contextsList.Contains(this))
				contextsList.Add (this);

			GetPropertyInfo ();
			if (propertyInfo != null) {
				model = propertyInfo.GetValue (viewModel, null);
				if (model == null && type == ContextType.PROPERTY)
					model = ReflectUtils.GetCachedTypeInstance (propertyInfo.PropertyType);
				// Subscript for property change event
				if(model != null)
					viewModel.SubcriptObjectAction (model);
			}

			RegisterBindingMessage (false);
		}

		public void Rebinding () {
			// Subscript for property change event
			GetPropertyInfo ();
			if (propertyInfo != null) {
				model = propertyInfo.GetValue (viewModel, null);
				if (model == null && type == ContextType.PROPERTY)
					model = ReflectUtils.GetCachedTypeInstance (propertyInfo.PropertyType);
				if(model != null)
					viewModel.SubcriptObjectAction (model);
			}
		}

		// Register binding message for child binders
		void RegisterBindingMessage (bool forceReinit = false) {
			BinderBase[] binders = GetComponentsInChildren<BinderBase> (true);
			for (int i = 0; i < binders.Length; i++) {
				BinderBase binder = binders [i];
				if (binder.mDataContext == this) {
					refBinders.Add (binder);
					binder.Init (forceReinit);
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

#endregion
}
