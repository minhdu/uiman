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
		/// Initializes a new instance of the <see cref="UnuGames.ObservableModel"/> class.
		/// </summary>
		static public T New<T> () where T : ObservableModel {
			return (T)new ObservableModel ();
		} 

		/// <summary>
		/// Initializes a new instance of the <see cref="UnuGames.ObservableModel"/> class.
		/// </summary>
		/// <param name="instance">Instance.</param>
		static public T New<T> (T instance) where T : ObservableModel, new() {
			Dictionary<string, Action<object>> actionDict = instance.actionDict;
			Dictionary<string, PropertyInfo> propertyCache = instance.propertyCache;
			instance = new T ();
			instance.actionDict = actionDict;
			foreach (KeyValuePair<string, PropertyInfo> prop in propertyCache) {
				instance.propertyCache.Add (prop.Key, instance.GetType ().GetProperty (prop.Value.Name));
			}

			return instance;
		}

		/// <summary>
		/// Notify the property which has change to all binder that has been subcribed with property name and value.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		public virtual void NotifyPropertyChanged (string propertyName, object value)
		{
			Action<object> actions = null;
			if (actionDict.TryGetValue (propertyName, out actions)) {
				try
				{
					if(actions != null)
						actions(value);
				}
				catch(Exception e)
				{
					UnuLogger.LogError(e.Message);
				}
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
			if(propertyCache.Count == 0)
				DataContext.Rebind (this);

			string propertyName = GetCaller ();
			PropertyInfo property = null;
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

		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <param name="value">Value.</param>
		public void SetValue (string propertyName, object value)
		{
			PropertyInfo property = null;
			if (propertyCache.TryGetValue (propertyName, out property)) {
				property.SetValue (this, value, null);
			}
		}
	}
}