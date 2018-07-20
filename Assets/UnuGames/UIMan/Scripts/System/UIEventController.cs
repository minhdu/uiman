using System.Collections.Generic;
using System;

namespace UnuGames {
	public class UIEventController
	{
		//
		// Properties
		//
		public Dictionary<string, Delegate> Routers { get; set; }
		public List<string> PermanentEvents { get; set; }
		
		//
		// Constructors
		//
		public UIEventController ()
		{
			Routers = new Dictionary<string, Delegate> ();
			PermanentEvents = new List<string> ();
		}
		
		//
		// Methods
		//
		public void AddEventListener<T> (string eventType, Action<T> handler)
		{
			OnListenerAdding (eventType, handler);
			Routers [eventType] = Delegate.Combine (Routers [eventType], handler);
		}
		
		public void AddEventListener<T0, T1, T2, T3> (string eventType, Action<T0, T1, T2, T3> handler)
		{
			OnListenerAdding (eventType, handler);
			Routers [eventType] = Delegate.Combine (Routers [eventType], handler);
		}
		
		public void AddEventListener<T0, T1, T2> (string eventType, Action<T0, T1, T2> handler)
		{
			OnListenerAdding (eventType, handler);
			Routers [eventType] = Delegate.Combine (Routers [eventType], handler);
		}
		
		public void AddEventListener<T0, T1> (string eventType, Action<T0, T1> handler)
		{
			OnListenerAdding (eventType, handler);
			Routers [eventType] = Delegate.Combine (Routers [eventType], handler);
		}
		
		public void AddEventListener (string eventType, Action handler)
		{
			OnListenerAdding (eventType, handler);
			Routers [eventType] = Delegate.Combine (Routers [eventType], handler);
		}
		
		public void Cleanup ()
		{
			List<string> list = new List<string> ();
			foreach (KeyValuePair<string, Delegate> current in Routers)
			{
				bool flag = false;
				foreach (string current2 in PermanentEvents)
				{
					if (current.Key == current2)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list.Add (current.Key);
				}
			}
			foreach (string current2 in list)
			{
				Routers.Remove (current2);
			}
		}
		
		public bool ContainsEvent (string eventType)
		{
			return Routers.ContainsKey (eventType);
		}
		
		public void MarkAsPermanent (string eventType)
		{
			PermanentEvents.Add (eventType);
		}
		
		public void RemoveEventListener<T0, T1, T2, T3> (string eventType, Action<T0, T1, T2, T3> handler)
		{
			if (OnListenerRemoving (eventType, handler))
			{
				Routers [eventType] = Delegate.Remove (Routers [eventType], handler);
				OnListenerRemoved (eventType);
			}
		}
		
		public void RemoveEventListener (string eventType, Action handler)
		{
			if (OnListenerRemoving (eventType, handler))
			{
				Routers [eventType] = Delegate.Remove (Routers [eventType], handler);
				OnListenerRemoved (eventType);
			}
		}
		
		public void RemoveEventListener<T0, T1, T2> (string eventType, Action<T0, T1, T2> handler)
		{
			if (OnListenerRemoving (eventType, handler))
			{
				Routers [eventType] = Delegate.Remove (Routers [eventType] as Action<T0, T1, T2>, handler) as Action<T0, T1, T2>;
				OnListenerRemoved (eventType);
			}
		}
		
		public void RemoveEventListener<T> (string eventType, Action<T> handler)
		{
			if (OnListenerRemoving (eventType, handler))
			{
				Routers [eventType] = Delegate.Remove (Routers [eventType] as Action<T>, handler) as Action<T>;
				OnListenerRemoved (eventType);
			}
		}
		
		public void RemoveEventListener<T0, T1> (string eventType, Action<T0, T1> handler)
		{
			if (OnListenerRemoving (eventType, handler))
			{
				Routers [eventType] = Delegate.Remove (Routers [eventType] as Action<T0, T1>, handler) as Action<T0, T1>;
				OnListenerRemoved (eventType);
			}
		}
		
		public void TriggerEvent<T0, T1, T2, T3> (string eventType, T0 arg1, T1 arg2, T2 arg3, T3 arg4)
		{
			Delegate @delegate;
			if (Routers.TryGetValue (eventType, out @delegate))
			{
				Delegate[] invocationList = @delegate.GetInvocationList ();
				for (int i = 0; i < invocationList.Length; i++)
				{
					Action<T0, T1, T2, T3> action = invocationList [i] as Action<T0, T1, T2, T3>;
					if (action == null)
					{
						throw new EventException (string.Format ("TriggerEvent {0} error: types of parameters are not match.", eventType));
					}
					try
					{
						action (arg1, arg2, arg3, arg4);
					}
					catch (Exception ex)
					{
						UnuLogger.LogError(ex.StackTrace);
					}
				}
			}
		}
		
		public void TriggerEvent<T0, T1> (string eventType, T0 arg1, T1 arg2)
		{
			Delegate @delegate;
			if (Routers.TryGetValue (eventType, out @delegate))
			{
				Delegate[] invocationList = @delegate.GetInvocationList ();
				for (int i = 0; i < invocationList.Length; i++)
				{
					Action<T0, T1> action = invocationList [i] as Action<T0, T1>;
					if (action == null)
					{
						throw new EventException (string.Format ("TriggerEvent {0} error: types of parameters are not match.", eventType));
					}
					try
					{
						action (arg1, arg2);
					}
					catch (Exception ex)
					{
						UnuLogger.LogError(ex.StackTrace);
					}
				}
			}
		}
		
		public void TriggerEvent<T0, T1, T2> (string eventType, T0 arg1, T1 arg2, T2 arg3)
		{
			Delegate @delegate;
			if (Routers.TryGetValue (eventType, out @delegate))
			{
				Delegate[] invocationList = @delegate.GetInvocationList ();
				for (int i = 0; i < invocationList.Length; i++)
				{
					Action<T0, T1, T2> action = invocationList [i] as Action<T0, T1, T2>;
					if (action == null)
					{
						throw new EventException (string.Format ("TriggerEvent {0} error: types of parameters are not match.", eventType));
					}
					try
					{
						action (arg1, arg2, arg3);
					}
					catch (Exception ex)
					{
						UnuLogger.LogError(ex.StackTrace);
					}
				}
			}
		}
		
		public void TriggerEvent<T> (string eventType, T arg1)
		{
			Delegate @delegate;
			if (Routers.TryGetValue (eventType, out @delegate))
			{
				Delegate[] invocationList = @delegate.GetInvocationList ();
				for (int i = 0; i < invocationList.Length; i++)
				{
					Action<T> action = invocationList [i] as Action<T>;
					if (action == null)
					{
						throw new EventException (string.Format ("TriggerEvent {0} error: types of parameters are not match.", eventType));
					}
					try
					{
						action (arg1);
					}
					catch (Exception ex)
					{
						UnuLogger.LogError(ex.StackTrace);
					}
				}
			}
		}
		
		public void TriggerEvent (string eventType)
		{
			Delegate @delegate;
			if (Routers.TryGetValue (eventType, out @delegate))
			{
				Delegate[] invocationList = @delegate.GetInvocationList ();
				for (int i = 0; i < invocationList.Length; i++)
				{
					Action action = invocationList [i] as Action;
					if (action == null)
					{
						throw new EventException (string.Format ("TriggerEvent {0} error: types of parameters are not match.", eventType));
					}
					try
					{
						action ();
					}
					catch (Exception ex)
					{
						UnuLogger.LogError(ex.StackTrace);
					}
				}
			}
		}

		public void OnListenerAdding(string eventType, Delegate listenerBeingAdded) {
			#if LOG_ALL_MESSAGES || LOG_ADD_LISTENER
			Debug.Log("MESSENGER OnListenerAdding \t\"" + eventType + "\"\t{" + listenerBeingAdded.Target + " -> " + listenerBeingAdded.Method + "}");
			#endif
			
			if (!Routers.ContainsKey(eventType)) {
				Routers.Add(eventType, null );
			}
			
			Delegate d = Routers[eventType];
			if (d != null && d.GetType() != listenerBeingAdded.GetType()) {
				throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
			}
		}

		public bool OnListenerRemoving(string eventType, Delegate listenerBeingRemoved) {
			#if LOG_ALL_MESSAGES
			Debug.Log("MESSENGER OnListenerRemoving \t\"" + eventType + "\"\t{" + listenerBeingRemoved.Target + " -> " + listenerBeingRemoved.Method + "}");
			#endif

			bool success = true;
			if (Routers.ContainsKey(eventType)) {
				Delegate d = Routers[eventType];
				
				if (d == null) {
					success = false;
					throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
				} else if (d.GetType() != listenerBeingRemoved.GetType()) {
					success = false;
					throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
				}
			} else {
				success = false;
				throw new ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
			}

			return success;
		}
		
		public void OnListenerRemoved(string eventType) {
			if (Routers[eventType] == null) {
				Routers.Remove(eventType);
			}
		}
	}

	public class ListenerException: Exception
	{
		public ListenerException () {
		}

		public ListenerException (string exception) {
		}
	}

	public class EventException: Exception
	{
		public EventException () {
		}
		
		public EventException (string exception) {
		}
	}
}