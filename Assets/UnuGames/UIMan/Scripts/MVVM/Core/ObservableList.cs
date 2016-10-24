using System;
using System.Collections;
using System.Collections.Generic;

namespace UnuGames.MVVM
{
	public class ObservableList<T> : IEnumerable<T>, IEnumerable, IObservaleCollection
	{

		List<T> _list = new List<T> ();
		Action<object> _onAddObject = delegate {
		};

		public event Action<object> OnAddObject {
			add {
				_onAddObject = (Action<object>)Delegate.Combine (_onAddObject, value);
			}
			remove {
				_onAddObject = (Action<object>)Delegate.Remove (_onAddObject, value);
			}
		}

		Action<int, object> _onInsertObject = delegate {
		};

		public event Action<int, object> OnInsertObject {
			add {
				_onInsertObject = (Action<int, object>)Delegate.Combine (_onInsertObject, value);
			}
			remove {
				_onInsertObject = (Action<int, object>)Delegate.Remove (_onInsertObject, value);
			}
		}

		Action<object> _onRemoveObject = delegate {
		};

		public event Action<object> OnRemoveObject {
			add {
				_onRemoveObject = (Action<object>)Delegate.Combine (_onRemoveObject, value);
			}
			remove {
				_onRemoveObject = (Action<object>)Delegate.Remove (_onRemoveObject, value);
			}
		}

		Action _onClearObjects = delegate {
		};

		public event Action OnClearObjects {
			add {
				_onClearObjects = (Action)Delegate.Combine (_onClearObjects, value);
			}
			remove {
				_onClearObjects = (Action)Delegate.Remove (_onClearObjects, value);
			}
		}

		Action<int, object> _onChangeObject = delegate {
		};

		public event Action<int, object> OnChangeObject {
			add {
				_onChangeObject = (Action<int, object>)Delegate.Combine (_onChangeObject, value);
			}
			remove {
				_onChangeObject = (Action<int, object>)Delegate.Remove (_onChangeObject, value);
			}
		}

		Action<T> _onAdd = delegate {
		};
		Action<int, T> _onInsert = delegate {
		};
		Action<T> _onRemove = delegate {
		};
		Action _onClear = delegate {
		};

		public int Count {
			get {
				return _list.Count;
			}
		}

		public T this [int i] {
			get {
				return _list [i];
			}
			set {
				_list [i] = value;
				if (_onChangeObject != null)
					_onChangeObject (i, value);
			}
		}

		public ObservableList ()
		{
		}
	
		public ObservableList (IEnumerable<T> set)
		{
			AddRange (set);
		}

		public void Add (T o)
		{
			_list.Add (o);
			if (_onAdd != null) {
				_onAdd (o);
			}
			if (_onAddObject != null) {
				_onAddObject (o);
			}
		}

		public void AddRange (IEnumerable<T> list)
		{
			IEnumerator ienumerator = list.GetEnumerator ();
			while (ienumerator.MoveNext()) {
				Add ((T)ienumerator.Current);
			}
		}
	
		public void Clear ()
		{
			_list.Clear ();
			if (_onClear != null) {
				_onClear ();
			}
			_onClearObjects ();
		}
	
		public bool Contains (T item)
		{
			return _list.Contains (item);
		}
	
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return _list.GetEnumerator ();
		}
	
		IEnumerator<T> IEnumerable<T>.GetEnumerator ()
		{
			return _list.GetEnumerator ();
		}
	
		public List<T>.Enumerator GetEnumerator ()
		{
			return _list.GetEnumerator ();
		}

		public void Insert (int index, T o)
		{
			_list.Insert (index, o);
			if (_onInsert != null) {
				_onInsert (index, o);
			}
			if (_onInsertObject != null) {
				_onInsertObject (index, o);
			}
		}

		public void Remove (T o)
		{
			if (_list.Remove (o)) {
				if (_onRemove != null) {
					_onRemove (o);
				}
				if (_onRemoveObject != null) {
					_onRemoveObject (o);
				}
			}
		}

		public void RemoveAt (int index)
		{
			T obj = _list [index];
			Remove (obj);
		}
	
		public void RemoveRange (IEnumerable<T> list)
		{
			IEnumerator ienumerator = list.GetEnumerator ();
			while (ienumerator.MoveNext()) {
				Remove ((T)ienumerator.Current);
			}
		}

		public int IndexOf (object obj)
		{
			return _list.IndexOf ((T)obj);
		}
	}
}
