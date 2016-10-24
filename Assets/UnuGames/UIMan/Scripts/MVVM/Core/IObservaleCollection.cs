using UnityEngine;
using System.Collections;
using System;

namespace UnuGames.MVVM
{
	public interface IObservaleCollection
	{

		int Count {
			get;
		}

		int IndexOf (object o);

		event Action<object> OnAddObject;
		event Action<object> OnRemoveObject;
		event Action<int, object> OnInsertObject;
		event Action OnClearObjects;
		event Action<int, object> OnChangeObject;
	}
}