using UnityEngine;
using UnityEngine.UI;
using UnuGames;
using UnuGames.MVVM;
using System.Collections.Generic;
using System.Reflection;

namespace UnuGames
{
	[RequireComponent (typeof(ScrollRect))]
	[DisallowMultipleComponent]
	public class ObservableListBinder : BinderBase
	{

		public RectTransform contentRect;
		ScrollRect scrollRect;
		public float contentWidth;
		public float contentHeight;
		public float contentSpacing;
		[HideInInspector]
		public BindingField observableList = new BindingField ("Data Source");
		public GameObject contentPrefab;
		public int poolSize = 10;
		IObservaleCollection dataList;
		Queue<ViewModelBehaviour> vmsPool = new Queue<ViewModelBehaviour> ();
		Queue<IModule> modulesPool = new Queue<IModule> ();
		List<ViewModelBehaviour> listVMs = new List<ViewModelBehaviour> ();
		List<IModule> listModules = new List<IModule> ();
		MemberInfo sourceMember;
		Vector3 hidePosition = new Vector3 (10000, 0, 0);

		public override void Init ()
		{
			if (!Application.isPlaying)
				return;
			if (isInit)
				return;
			isInit = true;

			InitPool ();

			scrollRect = GetComponent<ScrollRect> ();
			RectTransform contentRect = contentPrefab.GetComponent<RectTransform> ();
			if (contentWidth == 0)
				contentWidth = contentRect.sizeDelta.x;
			if (contentHeight == 0)
				contentHeight = contentRect.sizeDelta.y;

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
				dataList.OnInsertObject += HandleOnInsert;
				dataList.OnClearObjects += HandleOnClear;
				dataList.OnChangeObject += HandleOnChange;
			}
			scrollRect.onValueChanged.AddListener (OnScroll);

			contentPrefab.SetActive (false);
		}

		#region Pooling

		void InitPool ()
		{
			for (int i = 0; i < poolSize; i++) {
				GameObject obj = Instantiate (contentPrefab, hidePosition, Quaternion.identity) as GameObject;
				ViewModelBehaviour vm = obj.GetComponent<ViewModelBehaviour> ();
				vmsPool.Enqueue (vm);
				IModule module = (IModule)vm;
				modulesPool.Enqueue (module);
				vm.Recttransform.SetParent (contentRect);
				vm.Recttransform.localScale = Vector3.one;
			}
		}

		ViewModelBehaviour GetVMFromPool ()
		{
			ViewModelBehaviour vm = vmsPool.Dequeue ();
			vm.gameObject.name = "showing";
			return vm;
		}

		void ReleaseVM (ViewModelBehaviour vm)
		{
			vmsPool.Enqueue (vm);
			vm.Recttransform.localPosition = hidePosition;
			vm.gameObject.name = "free";
		}

		IModule GetModuleFromPool ()
		{
			return modulesPool.Dequeue ();
		}

		void ReleaseModule (IModule module)
		{
			modulesPool.Enqueue (module);
		}

		#endregion

		void OnScroll (Vector2 velocity)
		{
		
		}

		void HandleOnInsert (int index, object obj)
		{
			if (vmsPool.Count > 0) {
				ViewModelBehaviour vm = GetVMFromPool ();
				IModule module = GetModuleFromPool ();
				listVMs.Insert (index, vm);
				listModules.Insert (index, module);
				module.OriginalData = obj;
			}
			RecalculatePosition (index);
		}

		void HandleOnClear ()
		{
			for (int i = 0; i < listVMs.Count; i++) {
				ReleaseVM (listVMs [i]);
				ReleaseModule (listModules [i]);
				listVMs [i].Recttransform.localPosition = hidePosition;
			}

			listVMs.Clear ();
			listModules.Clear ();
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
				ReleaseVM (listVMs [indexToRemove]);
				ReleaseModule (listModules [indexToRemove]);
				listVMs.RemoveAt (indexToRemove);
				listModules.RemoveAt (indexToRemove);
				RecalculatePosition (indexToRemove);
			}
		}

		void HandleOnAdd (object obj)
		{
			if (vmsPool.Count > 0) {
				ViewModelBehaviour vm = GetVMFromPool ();
				IModule module = GetModuleFromPool ();
				listVMs.Add (vm);
				listModules.Add (module);
				module.OriginalData = obj;
			}
			RecalculatePosition (listVMs.Count);
		}

		void HandleOnChange (int index, object obj)
		{
			listModules [index].OriginalData = obj;
		}

		void RecalculatePosition (int startIndex = 0)
		{
			for (int i = startIndex - 1; i < dataList.Count; i++) {
				Vector2 position = Vector2.zero;
				if (scrollRect.horizontal) {
					position.x = i * contentWidth + (i + 1) * contentSpacing;
				} else if (scrollRect.vertical) {
					position.y = -i * contentHeight - (i + 1) * contentSpacing;
				}
				listVMs [i].Recttransform.localPosition = position;
			}

			if (scrollRect.horizontal) {
				contentRect.sizeDelta = new Vector2 (contentWidth * dataList.Count + contentSpacing * (dataList.Count + 1), contentRect.sizeDelta.y);
			} else if (scrollRect.vertical) {
				contentRect.sizeDelta = new Vector2 (contentRect.sizeDelta.x, contentHeight * dataList.Count + contentSpacing * (dataList.Count + 1));
			}
		}

		public override void OnDisable ()
		{
			if (dataList != null) {
				dataList.OnAddObject -= HandleOnAdd;
				dataList.OnRemoveObject -= HandleOnRemove;
				dataList.OnInsertObject -= HandleOnInsert;
				dataList.OnClearObjects -= HandleOnClear;
				dataList.OnChangeObject -= HandleOnChange;
			}
		}

		public ViewModelBehaviour GetIndex (int index)
		{
			ViewModelBehaviour result = null;

			if (listVMs != null && listVMs.Count < index) {
				result = listVMs [index];
			}

			return result;
		}
	}
}