using System;

namespace UnuGames.MVVM
{
	public interface IObservable
	{
		void OnPropertyChanged ();

		void NotifyPropertyChanged (string propertyName, object value);

		void SubscribeAction (string propertyName, Action<object> updateAction);

		void UnSubscribeAction (string propertyName, Action<object> updateAction);

		void SetValue (string propertyName, object value);
	}
}
