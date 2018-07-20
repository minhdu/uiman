using System;

namespace UnuGames {
	public class UIEventDispatcher
	{
		//
		// Static Properties
		//
		static UIEventController mEventController = new UIEventController();
		public static UIEventController Controller {
			get {
				return mEventController;
			}
			set {
				mEventController = value;
			}
		}
		
		//
		// Static Methods
		//
		public static void AddEventListener<T0, T1> (string eventType, Action<T0, T1> handler)
		{
			Controller.AddEventListener<T0, T1> (eventType, handler);
		}
		
		public static void AddEventListener<T0, T1, T2> (string eventType, Action<T0, T1, T2> handler)
		{
			Controller.AddEventListener<T0, T1, T2> (eventType, handler);
		}
		
		public static void AddEventListener<T0, T1, T2, T3> (string eventType, Action<T0, T1, T2, T3> handler)
		{
			Controller.AddEventListener<T0, T1, T2, T3> (eventType, handler);
		}
		
		public static void AddEventListener<T> (string eventType, Action<T> handler)
		{
			Controller.AddEventListener<T> (eventType, handler);
		}
		
		public static void AddEventListener (string eventType, Action handler)
		{
			Controller.AddEventListener (eventType, handler);
		}
		
		public static void Cleanup ()
		{
			Controller.Cleanup ();
		}
		
		public static void MarkAsPermanent (string eventType)
		{
			Controller.MarkAsPermanent (eventType);
		}
		
		public static void RemoveEventListener<T0, T1, T2> (string eventType, Action<T0, T1, T2> handler)
		{
			Controller.RemoveEventListener<T0, T1, T2> (eventType, handler);
		}
		
		public static void RemoveEventListener<T0, T1, T2, T3> (string eventType, Action<T0, T1, T2, T3> handler)
		{
			Controller.RemoveEventListener<T0, T1, T2, T3> (eventType, handler);
		}
		
		public static void RemoveEventListener<T0, T1> (string eventType, Action<T0, T1> handler)
		{
			Controller.RemoveEventListener<T0, T1> (eventType, handler);
		}
		
		public static void RemoveEventListener (string eventType, Action handler)
		{
			Controller.RemoveEventListener (eventType, handler);
		}
		
		public static void RemoveEventListener<T> (string eventType, Action<T> handler)
		{
			Controller.RemoveEventListener<T> (eventType, handler);
		}
		
		public static void TriggerEvent<T0, T1, T2> (string eventType, T0 arg1, T1 arg2, T2 arg3)
		{
			Controller.TriggerEvent<T0, T1, T2> (eventType, arg1, arg2, arg3);
		}
		
		public static void TriggerEvent<T0, T1, T2, T3> (string eventType, T0 arg1, T1 arg2, T2 arg3, T3 arg4)
		{
			Controller.TriggerEvent<T0, T1, T2, T3> (eventType, arg1, arg2, arg3, arg4);
		}
		
		public static void TriggerEvent<T0, T1> (string eventType, T0 arg1, T1 arg2)
		{
			Controller.TriggerEvent<T0, T1> (eventType, arg1, arg2);
		}
		
		public static void TriggerEvent (string eventType)
		{
			Controller.TriggerEvent (eventType);
		}
		
		public static void TriggerEvent<T> (string eventType, T arg1)
		{
			Controller.TriggerEvent<T> (eventType, arg1);
		}
	}
}