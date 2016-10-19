using UnityEngine;
using System.Collections;

namespace UnuGames
{
	public class UIManModule<T> : ViewModelBehaviour, IModule
	{

		T _dataInstance = default(T);

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