using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

namespace UnuGames.MVVM {

	/// <summary>
	/// Data context.
	/// </summary>
	public class DataContext : MonoBehaviour {

#region DataContext Factory
		static List<DataContext> contextsList = new List<DataContext> ();
		static public void NotifyObjectChange (object modelInstance) {
			for (int i = 0; i < contextsList.Count; i++) {
				DataContext context = contextsList [i];
				if (context.model != null && context.model is ObservableModel) {
					PropertyInfo propertyInfo = context.viewModel.IsBindingTo (modelInstance);
					if (propertyInfo != null) {
						context.viewModel.NotifyModelChange (modelInstance);
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

        public void Clear () {
            viewModel = null;
			propertyName = null;
			propertyInfo = null;
        }

		void Awake () {

			if(!contextsList.Contains(this))
				contextsList.Add (this);

			Init ();
			RegisterBindingMessage (false);
		}

		// Subscript for property change event
		public void Init () {
			GetPropertyInfo ();
			if (propertyInfo != null) {
				model = propertyInfo.GetValue (viewModel, null);
				if (model == null && type == ContextType.PROPERTY)
					model = ReflectUtils.GetCachedTypeInstance (propertyInfo.PropertyType);
				if (model != null) {
					viewModel.SubcriptObjectAction (model);
					viewModel.SubscribeAction (propertyName, viewModel.NotifyModelChange);
				}
			}
		}

		// Register binding message for child binders
		void RegisterBindingMessage (bool forceReinit = false) {
			BinderBase[] binders = GetComponentsInChildren<BinderBase> (true);
			for (int i = 0; i < binders.Length; i++) {
				BinderBase binder = binders [i];
				if (binder.mDataContext == this) {
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
