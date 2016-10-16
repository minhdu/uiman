using System.Collections.Generic;
using System.Reflection;
using System;
using System.Diagnostics;

namespace UnuGames
{
	public class ObservableModel : IObservable
	{
		Dictionary<string, Action<object>> actionDict = new Dictionary<string, Action<object>> ();
		Dictionary<string, PropertyInfo> propertyCache = new Dictionary<string, PropertyInfo> ();

		/// <summary>
		/// Notify the property which has change to all binder that has been subcribed with property name and value.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		public virtual void NotifyPropertyChanged (string propertyName, object value)
		{
			Action<object> actions = null;
			UnityEngine.Debug.LogError ("change " + propertyName);
			if (actionDict.TryGetValue (propertyName, out actions)) {
				actions (value);
			} else {
				UnuLogger.LogWarning (BindingDefine.NO_BINDER_REGISTERED);
			}
		}

		/// <summary>
		/// Raise the change event automatically without name and value,
		/// only use this function in property getter
		/// </summary>
		public void OnPropertyChanged ()
		{
			string propertyName = GetCaller ();
			PropertyInfo property = null;
			UnityEngine.Debug.LogError (propertyName + propertyCache.Count);
			if (propertyCache.TryGetValue (propertyName, out property)) {
				object newValue = property.GetValue (this, null);
				NotifyPropertyChanged (propertyName, newValue);
			}
		}

		/// <summary>
		/// Get the caller of current function
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		static string GetCaller (int level = 2)
		{
			StackFrame sf = new StackFrame (level);
			return sf.GetMethod ().Name;
		}

		/// <summary>
		/// Subcribe action to notify on property changed
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="updateAction"></param>
		public void SubscribeAction (string propertyName, Action<object> updateAction)
		{
			string propertyKey = "set_" + propertyName;
			if (actionDict.ContainsKey (propertyKey)) {
				actionDict [propertyKey] += updateAction;
			} else {
				actionDict.Add (propertyKey, updateAction);
				propertyCache.Add (propertyKey, this.GetCachedType ().GetProperty (propertyName));
			}
		}

		/// <summary>
		/// Unsubcribe action from notify on property changed
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="updateAction"></param>
		public void UnSubscribeAction (string propertyName, Action<object> updateAction)
		{
			string propertyKey = "set_" + propertyName;
			if (actionDict.ContainsKey (propertyKey)) {
				actionDict [propertyKey] -= updateAction;
			}
		}

		public void SetValue (string propertyName, object value)
		{
			PropertyInfo property = null;
			if (propertyCache.TryGetValue (propertyName, out property)) {
				property.SetValue (this, value, null);
			}
		}

		public void NotifyAllChange ()
		{
		
		}
	}
}