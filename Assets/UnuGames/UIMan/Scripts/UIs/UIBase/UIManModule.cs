using UnityEngine;
using System.Collections;
using UnuGames.MVVM;
namespace UnuGames
{
	public class UIManModule<T> : ViewModelBehaviour, IModule where T : new()
	{

		T _dataInstance = new T();

		[UIManProperty]
		public virtual T DataInstance {
			get {
				return _dataInstance;
			}
			set {
				_dataInstance = value;
				NotifyModelChange (DataInstance);
			}
		}

		public object OriginalData {
			get {
				return (object)DataInstance;
			}
			set {
				DataInstance = (T)value;
			}
		}
	}
}