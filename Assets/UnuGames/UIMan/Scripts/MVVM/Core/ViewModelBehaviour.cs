using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.ComponentModel;
using System;
using System.Reflection;

namespace UnuGames {

	/// <summary>
	/// ViewModel behavior (part of MVVM pattern).
	/// </summary>
	public class ViewModelBehaviour : MonoBehaviour, IObservable {

        Dictionary<string, Action<object>> actionDict = new Dictionary<string, Action<object>>();
		Dictionary<string, PropertyInfo> propertyCache = new Dictionary<string, PropertyInfo>();
		List<MemberInfo> notifyableMembers = new List<MemberInfo>();

		RectTransform mRecttransform;
		public RectTransform Recttransform {
			get {
				if(mRecttransform == null)
					mRecttransform = GetComponent<RectTransform>();
				return mRecttransform;
			}
		}

		Transform mTransform;
		public Transform Transform {
			get {
				if(mTransform == null)
					mTransform = GetComponent<Transform>();
				return mRecttransform;
			}
		}

		/// <summary>
		/// Notify the property which has change to all binder that has been subcribed with property name and value.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
        public virtual void NotifyPropertyChanged (string propertyName, object value) {
			Action<object> actions = null;
            if(actionDict.TryGetValue(propertyName, out actions))
            {
                try
                {
                    actions(value);
                }
                catch(Exception e)
                {
					UnuLogger.LogError(e.Message);
                }
            }
            else {
                //UnuLogger.LogWarning(BindingDefine.NO_BINDER_REGISTERED);
            }
        }

		/// <summary>
		/// Raise the change event automatically without name and value,
		/// only use this function in property getter
		/// </summary>
		public void OnPropertyChanged () {
			string propertyName = GetCaller();
			PropertyInfo property = null;
			if(propertyCache.TryGetValue(propertyName, out property)) {
				object newValue = property.GetValue(this, null);
				NotifyPropertyChanged(propertyName, newValue);
			}
		}

		/// <summary>
		/// Get the caller of current function
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		static string GetCaller(int level = 2)
		{
			StackFrame sf = new StackFrame(level);
			return sf.GetMethod().Name;
		}

		/// <summary>
		/// Subcribe action to notify on property changed
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="updateAction"></param>
		public void SubscribeAction (string propertyName, Action<object> updateAction) {
			string propertyKey = "set_" + propertyName;
			if(actionDict.ContainsKey(propertyKey)) {
                actionDict[propertyKey] += updateAction;
			}
            else {
                actionDict.Add(propertyKey, updateAction);
                propertyCache.Add(propertyKey, this.GetCachedType().GetProperty(propertyName));
			}
		}

        /// <summary>
        /// Unsubcribe action from notify on property changed
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="updateAction"></param>
        public void UnSubscribeAction (string propertyName, Action<object> updateAction) {
            string propertyKey = "set_" + propertyName;
			if(actionDict.ContainsKey(propertyKey)) {
                actionDict[propertyKey] -= updateAction;
            }
        }

		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <param name="value">Value.</param>
		public void SetValue (string propertyName, object value) {
			PropertyInfo property = null;
			if(propertyCache.TryGetValue("set_" + propertyName, out property)) {
				property.SetValue(this, value, null);
			}
		}

		/// <summary>
		/// Notifies the model change.
		/// </summary>
		/// <param name="obj">Object.</param>
		public void NotifyModelChange (object obj) {
			if (notifyableMembers == null)
				return;
			for (int i = 0; i < notifyableMembers.Count; i++) {
				object value = null;
				if (notifyableMembers [i] is FieldInfo) {
					FieldInfo field = notifyableMembers [i].ToField();
					value = field.GetValue (obj);
				} else {
					PropertyInfo property = notifyableMembers [i].ToProperty();
					value = property.GetValue (obj, null);
				}
				NotifyPropertyChanged ("set_" + notifyableMembers[i].Name, value);
			}
		}

		/// <summary>
		/// Subcripts the object action.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="onChange">On change.</param>
		public void SubcriptObjectAction (object obj) {
			MemberInfo[] members = obj.GetCachedType ().GetMembers ();
			for (int i = 0; i < members.Length; i++) {
				if (members [i] is FieldInfo || members [i] is PropertyInfo) {
					notifyableMembers.Add (members [i]);
				}
			}
		}

		/// <summary>
		/// Subcripts the object action.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="onChange">On change.</param>
		public void SubcriptObjectAction (PropertyInfo property) {
			MemberInfo[] members = property.GetType().GetMembers ();
			for (int i = 0; i < members.Length; i++) {
				if (members [i] is FieldInfo || members [i] is PropertyInfo) {
					notifyableMembers.Add (members [i]);
				}
			}
		}
	}

	/// <summary>
	/// Binding type.
	/// </summary>
	public enum ContextType {

		[DescriptionAttribute("Nothing")]
		NONE = 0,

		[DescriptionAttribute("MonoBehaviour")]
		MONO_BEHAVIOR,

		[DescriptionAttribute("Type Instance")]
		PROPERTY
	}
}