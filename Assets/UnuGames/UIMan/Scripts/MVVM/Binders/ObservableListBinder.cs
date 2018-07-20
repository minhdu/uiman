using UnityEngine;
using UnityEngine.UI;
using UnuGames;
using UnuGames.MVVM;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using UnityEngine.EventSystems;

namespace UnuGames.MVVM
{
	[RequireComponent (typeof(ScrollRect))]
	[DisallowMultipleComponent]
	public class ObservableListBinder : BinderBase
	{
		public struct Cell {
			public int column;
			public int row;
			public override string ToString ()
			{
				return string.Format ("({0},{1})", column, row);
			}

			public static bool operator != (Cell c1, Cell c2)
			{
				return (c1.column != c2.column || c1.row != c2.row);
			}

			public static bool operator == (Cell c1, Cell c2)
			{
				return (c1.column == c2.column && c1.row == c2.row);
			}

			public override bool Equals (object obj)
			{
				return base.Equals (obj);
			}

			public override int GetHashCode ()
			{
				return base.GetHashCode ();
			}
		}

		const int BOUND_BUFFERS = 3;

		public RectTransform contentRect;
		public RectTransform viewPort;
		[HideInInspector]
		public Rect viewPortRect;
		ScrollRect scrollRect;
		public float contentWidth;
		public float contentHeight;
		public Vector2 contentSpacing;
		public Vector2 padding;
		public Vector2 grouping;

		[HideInInspector]
		public BindingField observableList = new BindingField ("Data Source");
		public GameObject contentPrefab;
		int poolSize = 10;
		IObservaleCollection dataList;
		Queue<IModule> modulesPool = new Queue<IModule> ();
		List<IModule> listModules = new List<IModule> ();
		Dictionary<Cell, IModule> activeCells = new Dictionary<Cell, IModule>();
		Dictionary<Cell, object> dataDict = new Dictionary<Cell, object>();
		List<object> orgDataList = new List<object> ();
		MemberInfo sourceMember;
		Vector3 HIDE_POSITION = new Vector3 (999999999, 999999999, 0);

		#if UNITY_EDITOR
		void Update () {
			if (viewPort != null)
				viewPortRect = viewPort.rect;
		}
		#endif

		public override void Init (bool forceInit)
		{
			if (CheckInit (forceInit)) {
				scrollRect = GetComponent<ScrollRect> ();
				if (grouping == Vector2.zero)
					grouping = Vector2.one;
				RectTransform contentItemRect = contentPrefab.GetComponent<RectTransform> ();
				if (contentWidth == 0)
					contentWidth = contentItemRect.sizeDelta.x;
				if (contentHeight == 0)
					contentHeight = contentItemRect.sizeDelta.y;

				if (contentRect == null)
					contentRect = scrollRect.content;
				if (viewPort == null)
					viewPort = scrollRect.viewport;
				
				InitPool ();

				sourceMember = mDataContext.viewModel.GetMemberInfo (observableList.member);
				if (sourceMember is FieldInfo) {
					FieldInfo sourceField = sourceMember.ToField ();
					dataList = (IObservaleCollection)sourceField.GetValue (mDataContext.viewModel);

				} else {
					PropertyInfo sourceProperty = sourceMember.ToProperty ();
					dataList = (IObservaleCollection)sourceProperty.GetValue (mDataContext.viewModel, null);
				}
				if (dataList != null) {
					dataList.OnAddObject += HandleOnAdd;
					dataList.OnRemoveObject += HandleOnRemove;
					dataList.OnRemoveAt += HandleOnRemoveAt;
					dataList.OnInsertObject += HandleOnInsert;
					dataList.OnClearObjects += HandleOnClear;
					dataList.OnChangeObject += HandleOnChange;
				}
				scrollRect.onValueChanged.AddListener (OnScroll);

				contentPrefab.SetActive (false);
			}
		}

		#region Pooling

		void InitPool ()
		{
			if (scrollRect.horizontal) {
				float columnWidth = contentWidth + contentSpacing.x;
				int column = Mathf.RoundToInt(Mathf.Abs(viewPortRect.width) / columnWidth);
				poolSize = (column + 2*BOUND_BUFFERS) * (int)grouping.y;
			} else {
				float rowHeight = contentHeight + contentSpacing.y;
				int row = Mathf.RoundToInt(Mathf.Abs(viewPortRect.height) / rowHeight);
				poolSize = (row + 2*BOUND_BUFFERS) * (int)grouping.x;
			}

			for (int i = 0; i < poolSize; i++) {
				GameObject obj = Instantiate (contentPrefab, HIDE_POSITION, Quaternion.identity) as GameObject;
				ViewModelBehaviour vm = obj.GetComponent<ViewModelBehaviour> ();
				IModule module = (IModule)vm;
				modulesPool.Enqueue (module);
				vm.Recttransform.SetParent (contentRect, true);
				vm.Recttransform.localScale = Vector3.one;
			}
		}

		IModule GetModuleFromPool ()
		{
			if(modulesPool.Count > 0)
				return modulesPool.Dequeue ();
			return null;
		}

		void ReleaseModule (IModule module)
		{
			module.VM.transform.position = HIDE_POSITION;
			modulesPool.Enqueue (module);
		}

		#endregion

		void OnScroll (Vector2 velocity)
		{
			Vector2 rectBounds = GetScrollRectBounds ();

			// Check for hidden cell and push to pool
			List <Cell> releaseCells = new List<Cell> ();
			foreach (var cell in activeCells) {
				if (!IsVisible (cell.Key, rectBounds)) {
					ReleaseModule (cell.Value);
					listModules.Remove (cell.Value);
					releaseCells.Add (cell.Key);
				}
			}

			// Remove hidden cell from active list
			for (int i = 0; i < releaseCells.Count; i++) {
				activeCells.Remove (releaseCells [i]);
			}

			// Check and get cell from pool to fill blank cell
			Vector2 colRange = new Vector2 (rectBounds.x, rectBounds.y);
			Vector2 rowRange = new Vector2 (0, grouping.y);
			if (scrollRect.vertical) {
				colRange = new Vector2 (0, grouping.x);
				rowRange = new Vector2 (rectBounds.x, rectBounds.y);
			}
				
			for (int i = (int)colRange.x; i < colRange.y; i++) {
				for (int j = (int)rowRange.x; j < rowRange.y; j++) {
					if (i < 0 || j < 0)
						continue;
					Cell cell = new Cell () {
						column = i,
						row = j
					};

					if (!activeCells.ContainsKey(cell) && IsVisible (cell, rectBounds) && dataDict.ContainsKey(cell)) {
						IModule module = GetModuleFromPool ();
						if (module != null) {
							module.VM.Recttransform.anchoredPosition = GetPositionByCell (cell);
							if (activeCells.ContainsKey (cell))
								activeCells [cell] = module;
							else
								activeCells.Add (cell, module);
							listModules.Add (module);
							module.OriginalData = dataDict [cell];
						}
					}
				}
			}
		}

		void RecalculateBounds ()
		{
			int pageCount = Mathf.CeilToInt(orgDataList.Count / (grouping.x * grouping.y));
			if (scrollRect.horizontal) {
				contentRect.sizeDelta = new Vector2 (contentWidth * pageCount * grouping.x + contentSpacing.x * pageCount * grouping.x,
					contentHeight * grouping.y + contentSpacing.y * grouping.y);
			} else if (scrollRect.vertical) {
				contentRect.sizeDelta = new Vector2 (contentWidth * grouping.x + contentSpacing.x * grouping.x,
					contentHeight * pageCount * grouping.y + contentSpacing.y * pageCount * grouping.y);
			}
		}

		Vector2 GetScrollRectBounds () {
			if (scrollRect.horizontal) {
				float rectPos = contentRect.anchoredPosition.x;
				float columnWidth = contentWidth + contentSpacing.x;
				int minColumn = Mathf.FloorToInt (Mathf.Abs(rectPos) / columnWidth);
				int maxColumn = minColumn + Mathf.RoundToInt(viewPortRect.width / columnWidth);
				return new Vector2 (minColumn - BOUND_BUFFERS, maxColumn + BOUND_BUFFERS);
			} else {
				float rectPos = contentRect.anchoredPosition.y;
				float rowHeight = contentHeight + contentSpacing.y;
				int minRow = Mathf.FloorToInt (Mathf.Abs(rectPos) / rowHeight);
				int maxRow = minRow + Mathf.RoundToInt(viewPortRect.height / rowHeight);
				return new Vector2 (minRow - BOUND_BUFFERS, maxRow + BOUND_BUFFERS);
			}
		}

		Cell GetCellByIndex (int targetIndex) {
			Cell cell = new Cell();
			int index = 0;
			int pageCount = Mathf.CeilToInt(orgDataList.Count / (grouping.x * grouping.y));
			for(int page=0; page < pageCount; page++) {
				for (int i = 0; i < grouping.y; i++) {
					for (int j = 0; j < grouping.x; j++) {
						if (index == targetIndex) {
							if (scrollRect.horizontal) {
								cell.row = i;
								cell.column = page * (int)grouping.x + j;
							} else if (scrollRect.vertical) {
								cell.row = page * (int)grouping.y + i;
								cell.column = j;
							}
							return cell;
						}
						index++;
					}
				}
			}
			return cell;
		}

		Vector2 GetPositionByCell (Cell cell) {
			Vector2 position = Vector2.zero;
			position.x = contentWidth * cell.column + cell.column * contentSpacing.x + padding.x;
			position.y = - contentHeight * cell.row - cell.row * contentSpacing.y - padding.y;
			return position;
		}

		bool IsVisible (Cell cell, Vector2? rectBounds = null) {
			if(!rectBounds.HasValue)
				rectBounds = GetScrollRectBounds ();
			if (scrollRect.horizontal) {
				if (cell.column > rectBounds.Value.x && cell.column < rectBounds.Value.y)
					return true;
				else
					return false;
			} else {
				if (cell.row > rectBounds.Value.x && cell.row < rectBounds.Value.y)
					return true;
				else
					return false;
			}
		}

		void RefreshDataDict (int index) {
			for (int i = index; i < orgDataList.Count; i++) {
				Cell cell = GetCellByIndex (i);
				if (dataDict.ContainsKey (cell))
					dataDict [cell] = orgDataList [i];
				else
					dataDict.Add (cell, orgDataList[i]);
				if (activeCells.ContainsKey (cell))
					activeCells [cell].OriginalData = orgDataList [i];
			}
		}

		void HandleOnInsert (int index, object obj)
		{
			orgDataList.Insert (index, obj);
			RecalculateBounds ();
			RefreshDataDict (index);
			OnScroll (Vector2.zero);
		}

		void HandleOnClear ()
		{
			foreach (var cell in activeCells) {
				ReleaseModule (cell.Value);
			}
			listModules.Clear ();
			dataDict.Clear ();
			activeCells.Clear ();
			orgDataList.Clear ();
		}

		void HandleOnRemove (object obj)
		{
			int indexToRemove = 0;
			for (int i = 0; i < listModules.Count; i++) {
				if (listModules [i].OriginalData == obj) {
					indexToRemove = i;
					break;
				}
			}

			if (indexToRemove >= 0) {
				HandleOnRemoveAt (indexToRemove);
			}
		}

		void HandleOnRemoveAt (int index)
		{
			Cell cell = GetCellByIndex (orgDataList.Count-1);
			if (activeCells.ContainsKey (cell)) {
				ReleaseModule ((IModule)activeCells [cell].VM);
				activeCells [cell].VM.Recttransform.anchoredPosition = HIDE_POSITION;
			}
			listModules.RemoveAt (index);
			dataDict.Remove (cell);
			orgDataList.RemoveAt (index);
			RecalculateBounds ();
			RefreshDataDict (index);
			OnScroll (Vector2.zero);
		}

		void HandleOnAdd (object obj)
		{
			orgDataList.Add (obj);
			RecalculateBounds ();
			Cell cell = GetCellByIndex (orgDataList.Count-1);
			if (IsVisible (cell)) {
				IModule module = GetModuleFromPool ();
				if (module != null) {
					listModules.Add (module);
					module.OriginalData = obj;
					module.VM.Recttransform.anchoredPosition = GetPositionByCell (cell);
					if (activeCells.ContainsKey (cell))
						activeCells [cell] = module;
					else
						activeCells.Add (cell, module);
				}
			}
			dataDict.Add (cell, obj);
		}

		void HandleOnChange (int index, object obj)
		{
			listModules [index].OriginalData = obj;
			orgDataList [index] = obj;
			Cell cell = GetCellByIndex (index);
			dataDict[cell] = obj;
		}

		public bool IsActive (GameObject item)
		{
			IModule module = item.GetComponent<IModule> ();
			if (module != null) {
				foreach (var cell in activeCells) {
					if (cell.Value == module)
						return true;
				}
			}
			return false;
		}

		public ViewModelBehaviour GetItem (int index)
		{
			Cell cell = GetCellByIndex(index);
			return GetItem(cell);
		}

		public ViewModelBehaviour GetItem (Cell cell)
		{
			if(activeCells.ContainsKey(cell))
				return activeCells[cell].VM;
			return null;
		}
	}
}