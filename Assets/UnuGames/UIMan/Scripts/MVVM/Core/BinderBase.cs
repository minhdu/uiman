using UnuGames.MVVM;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnuGames {

	/// <summary>
	/// Base class for all binders.
	/// </summary>
	[Serializable]
    [ExecuteInEditMode]
    public abstract class BinderBase : MonoBehaviour {

        protected bool isInit = false;

        Type mType;
        public Type MyType {
            get {
                if(mType == null)
                    mType = GetType();
                return mType;
            }
        }

		[HideInInspector]
		public DataContext mDataContext;

        /// <summary>
        /// Find the Data Context for this binder
        /// </summary>
        /// <returns></returns>
        public DataContext FindDataContext () {
            if(mDataContext == null) {
                mDataContext = GetComponent<DataContext>();
                if(mDataContext == null)
                    mDataContext = GetComponentInParent<DataContext>();
            }

            return mDataContext;
        }

        /// <summary>
        /// Get the view model attached to current data context
        /// </summary>
        /// <returns></returns>
        public IObservable GetViewModel () {
            if(mDataContext == null)
                FindDataContext();

			return mDataContext.viewModel;
        }

		public string[] GetMembers (params MemberTypes[] memberTypes) {
			if(mDataContext == null)
				FindDataContext();
			
			if (mDataContext.type == ContextType.MONO_BEHAVIOR) {
				return mDataContext.viewModel.GetAllMembers(memberTypes);
			} else if(mDataContext.type == ContextType.PROPERTY) {
				return mDataContext.GetPropertyInfo().GetAllMembers(memberTypes);
			}

			return new string[0];
		}

		public MemberInfo GetMemberInfo (string memberName, params MemberTypes[] memberTypes) {
			if(mDataContext == null)
				FindDataContext();

			MemberInfo[] infos = null;
			if (mDataContext.type == ContextType.MONO_BEHAVIOR) {
				infos = mDataContext.viewModel.GetAllMembersInfo(memberTypes);
			} else if(mDataContext.type == ContextType.PROPERTY) {
				infos = mDataContext.GetPropertyInfo().GetAllMembersInfo(memberTypes);
			}

			for (int i=0; i<infos.Length; i++) {
				if(infos[i].Name == memberName)
					return infos[i];
			}

			return null;
		}

		/// <summary>
		/// All binder must implement this method to initialize the binder's instance.
		/// </summary>
        public abstract void Init ();

        /// <summary>
        /// All binder must implement this method to unsubsribe onchanged event when object is disable
        /// </summary>
		public  abstract void OnDisable ();

		/// <summary>
		/// Subscribe the on changed event
		/// </summary>
		/// <param name="_field">_binding info.</param>
		/// <param name="onChanged">On changed.</param>
		protected void SubscribeOnChangedEvent (BindingField _field, Action<object> onChanged) {
			_field.UpdateAction += onChanged;
			RegisterViewModel(_field.member, onChanged);
		}

		/// <summary>
		/// Unsubscribe the on changed event
		/// </summary>
		/// <param name="_field"></param>
		/// <param name="onChanged"></param>
		protected void UnSubscribeOnChangedEvent (BindingField _field, Action<object> onChanged) {
			_field.UpdateAction -= onChanged;
			UnRegisterViewModel(_field.member, onChanged);
		}

		/// <summary>
		/// Register the view model.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		void RegisterViewModel (string propertyName, Action<object> updateAction) {
			if(mDataContext != null && !string.IsNullOrEmpty(propertyName)) {
                mDataContext.viewModel.SubscribeAction(propertyName, updateAction);
			}
		}

        /// <summary>
        /// Un register the view model.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        void UnRegisterViewModel (string propertyName, Action<object> updateAction) {
            if(mDataContext != null && !string.IsNullOrEmpty(propertyName) && mDataContext.viewModel != null) {
                mDataContext.viewModel.UnSubscribeAction(propertyName, updateAction);
            }
        }

        protected BindingField[] fields;

        /// <summary>
        /// Get and cache all binding fields of this binder
        /// </summary>
        /// <returns></returns>
        public BindingField[] GetBindingFields () {

#if !UNITY_EDITOR
            if(fields != null)
                return  fields;
#endif

            List<BindingField> listField = new List<BindingField>();
            MemberInfo[] members = MyType.GetMembers();
            for (int i=0; i<members.Length; i++) {
                MemberInfo memberInfo = members[i];
                if(memberInfo.MemberType == MemberTypes.Field) {
                    FieldInfo fieldInfo = memberInfo as FieldInfo;
                    if(fieldInfo.FieldType == typeof(BindingField)) {
                        listField.Add(fieldInfo.GetValue(this) as BindingField);
                    }
                }
            }

            fields = listField.ToArray();
            return fields;
        }

		protected void CheckInit () {
			if(isInit)
				return;
			isInit = true;
		}

		protected void SetValue(string memberName, object value) {
			if (!enabled)
				return;

			if (string.IsNullOrEmpty(memberName))
				return;

			GetViewModel ().SetValue (memberName, value);
		}
	}
}