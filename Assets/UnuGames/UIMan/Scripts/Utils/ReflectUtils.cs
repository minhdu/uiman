using System;
using System.Collections.Generic;
using System.Reflection;
using UnuGames.MVVM;

namespace UnuGames
{
	static public class ReflectUtils
	{

#if !UNITY_EDITOR
		static Dictionary<object, string[]> cachedMembersName = new Dictionary<object, string[]> ();
		static Dictionary<object, Type> cachedTypes = new Dictionary<object, Type> ();
#endif

		static List<string> cachedAssembly = new List<string> ();
		static List<Type> allTypes = new List<Type> ();
		static Dictionary<Type, object> cachedInstance = new Dictionary<Type, object> ();
		static Dictionary<string, Type> cachedTypeName = new Dictionary<string, Type> ();

		/// <summary>
		/// Get all member with suitable type of current object
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="memberTypes"></param>
		/// <returns></returns>

		static public string[] GetAllMembers (this IObservable type, params MemberTypes[] memberTypes)
		{
			if (type == null)
				return  null;

			MemberInfo[] members = null;

			members = type.GetCachedType ().GetMembers ();

			bool all = false;
			if (memberTypes == null || (memberTypes != null && memberTypes.Length == 0))
				all = true;

			List<string> results = new List<string> ();
			for (int i = 0; i < members.Length; i++) {
				if (all) {
					results.Add (members [i].Name);
				} else {
					for (int j = 0; j < memberTypes.Length; j++) {
						if (all || members [i].MemberType == memberTypes [j]) {
							results.Add (members [i].Name);
							break;
						}
					}
				}
			}

			return results.ToArray ();
		}

		static public MemberInfo[] GetAllMembersInfo (this IObservable type, params MemberTypes[] memberTypes)
		{
			if (type == null)
				return  null;
		
			MemberInfo[] members = null;
		
			members = type.GetCachedType ().GetMembers ();
		
			bool all = false;
			if (memberTypes == null || (memberTypes != null && memberTypes.Length == 0))
				all = true;
		
			List<MemberInfo> results = new List<MemberInfo> ();
			for (int i = 0; i < members.Length; i++) {
				if (all) {
					results.Add (members [i]);
				} else {
					for (int j = 0; j < memberTypes.Length; j++) {
						if (members [i].MemberType == memberTypes [j]) {
							results.Add (members [i]);
							break;
						}
					}
				}
			}
		
			return results.ToArray ();
		}

		static public string[] GetAllMembers (this PropertyInfo proInfo, params MemberTypes[] memberTypes)
		{
			if (proInfo == null)
				return  null;
		
			MemberInfo[] members = null;
		
			members = proInfo.PropertyType.GetMembers ();
		
			bool all = false;
			if (memberTypes == null || (memberTypes != null && memberTypes.Length == 0))
				all = true;
		
			List<string> results = new List<string> ();
			for (int i = 0; i < members.Length; i++) {
				if (all) {
					results.Add (members [i].Name);
				} else {
					for (int j = 0; j < memberTypes.Length; j++) {
						if (members [i].MemberType == memberTypes [j]) {
							results.Add (members [i].Name);
							break;
						}
					}
				}
			}
		
			return results.ToArray ();
		}

		static public MemberInfo[] GetAllMembersInfo (this PropertyInfo proInfo, params MemberTypes[] memberTypes)
		{
			if (proInfo == null)
				return  null;
		
			MemberInfo[] members = null;
		
			members = proInfo.PropertyType.GetMembers ();
		
			bool all = false;
			if (memberTypes == null || (memberTypes != null && memberTypes.Length == 0))
				all = true;
		
			List<MemberInfo> results = new List<MemberInfo> ();
			for (int i = 0; i < members.Length; i++) {
				for (int j = 0; j < memberTypes.Length; j++) {
					if (all || members [i].MemberType == memberTypes [j]) {
						results.Add (members [i]);
						break;
					}
				}
			}
		
			return results.ToArray ();
		}

		static public MemberInfo GetMemberInfo (this IObservable type, string memberName, params MemberTypes[] memberTypes)
		{
			MemberInfo[] infos = type.GetAllMembersInfo (memberTypes);
			MemberInfo result = null;

			for (int i = 0; i < infos.Length; i++) {
				if (infos [i].Name == memberName) {
					result = infos [i];
					break;
				}
			}

			return result;
		}

		static public FieldInfo ToField (this MemberInfo member)
		{
			return (member as FieldInfo);
		}

		static public PropertyInfo ToProperty (this MemberInfo member)
		{
			return (member as PropertyInfo);
		}

		static public MethodInfo ToMethod (this MemberInfo member)
		{
			return (member as MethodInfo);
		}

		static public Type GetCachedType (this object obj)
		{

			Type type = null;
#if UNITY_EDITOR
			if (obj != null)
				type = obj.GetType ();
			else
				return null;
#else
			if (!cachedTypes.TryGetValue (obj, out type)) {
				type = obj.GetType ();
				cachedTypes.Add (obj, type);
			}
#endif
			return type;
		}

		static public List<string> GetAllAssembly ()
		{
			return cachedAssembly;
		}

		static public List<Type> GetAllTypes ()
		{
			return allTypes;
		}

		static public void RefreshAssembly (bool force)
		{
			if (!force && cachedAssembly.Count > 0)
				return;
			cachedAssembly.Clear ();
			allTypes.Clear ();

			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies ();
			for (int i = 0; i < assemblies.Length; i++) {
				Assembly asem = assemblies [i];
				if (!asem.Location.Contains ("Editor"))
					cachedAssembly.Add (asem.FullName);
			}

			for (int i = 0; i < assemblies.Length; i++) {
				Type[] types = assemblies [i].GetTypes ();
				for (int j = 0; j < types.Length; j++) {
					if (types [j].IsPublic) {
						allTypes.Add (types [j]);
					}
				}
			}
		}

		static public string[] GetAllUIManType ()
		{
			List<string> uiManTypes = new List<string> ();
			List<Type> types = GetAllTypes ();
			for (int i = 0; i < types.Count; i++) {
				string typeName = types [i].Name;
				Type type = Type.GetType (typeName);
				if (type != null) {
					if (type.BaseType == typeof(UIManScreen) || type.BaseType == typeof(UIManDialog) || type.BaseType == typeof(ObservableModel)) {
						if (!uiManTypes.Contains (typeName)) {
							uiManTypes.Add (typeName);
						}
					}
				}
			}

			return uiManTypes.ToArray ();
		}

		static public string[] GetAllRefType (Type baseType)
		{
			List<string> refTypes = new List<string> ();
			List<Type> types = GetAllTypes ();
			for (int i = 0; i < types.Count; i++) {
				string typeName = types [i].Name;
				Type type = Type.GetType (typeName);
				if (type != null) {
					if (type.BaseType == baseType) {
						if (!refTypes.Contains (typeName)) {
							refTypes.Add (typeName);
						}
					}
				}
			}

			return refTypes.ToArray ();
		}

		static public Type GetTypeByName (string name)
		{
			if (cachedTypeName.ContainsKey (name))
				return cachedTypeName [name];

			List<Type> types = GetAllTypes ();
			for (int i = 0; i < types.Count; i++) {
				if (types [i].Name == name || types [i].GetAllias () == name) {
					cachedTypeName.Add (name, types [i]);
					return types [i];
				}
			}

			return null;
		}

		static public Type GetUIManTypeByName (string typeName)
		{
			Type uiManType = null;
			Type type = Type.GetType (typeName);
			if (type != null) {
				if (type.BaseType == typeof(UIManScreen) || type.BaseType == typeof(UIManDialog) || type.BaseType == typeof(ObservableModel)) {
					if (type.Name == typeName) {
						uiManType = type;
					}
				}
			}

			return uiManType;
		}

#if UNITY_EDITOR
		static public CustomPropertyInfo[] GetUIManProperties (this Type uiManType)
		{
			PropertyInfo[] properties = uiManType.GetProperties ();
			List<CustomPropertyInfo> customProperties = new List<CustomPropertyInfo> ();
			foreach (PropertyInfo property in properties) {
				if (property.IsDefined (typeof(UIManProperty), true)) {
					object instance = GetCachedTypeInstance (uiManType);
					customProperties.Add (new CustomPropertyInfo (property.Name, property.PropertyType, property.GetValue (instance, null)));
				}
			}

			return customProperties.ToArray ();
		}
#endif

		static public string GetAllias (this Type type)
		{
			if (type == null)
				return null;
			Dictionary<string, string> dict = new Dictionary<string, string> ();
			dict.Add ("String", "string");
			dict.Add ("Boolean", "bool");
			dict.Add ("Int32", "int");
			dict.Add ("Int64", "long");
			dict.Add ("Single", "float");
			dict.Add ("Double", "double");

			if (dict.ContainsKey (type.Name))
				return dict [type.Name];
			else
				return type.Name;

			/*object : System.Object
			string : System.String
			bool : System.Boolean
			byte : System.Byte
			char : System.Char
			decimal : System.Decimal
			double : System.Double
			short : System.Int16
			int : System.Int32
			long : System.Int64
			sbyte : System.SByte
			float : System.Single
			ushort : System.UInt16
			uint : System.UInt32
			ulong : System.UInt64
			void : System.Void*/
		}

		static public string[] GetAllObservableType (Type excludeType = null)
		{
			List<Type> types = GetAllTypes ();
			List<string> observableTypes = new List<string> ();
			for (int i = 0; i < types.Count; i++) {
				if ((types [i].BaseType == typeof(ObservableModel) || types [i].IsAllias () || types [i].IsSupportType ()) && types [i] != excludeType) {
					observableTypes.Add (types [i].GetAllias ());
				}
			}

			return observableTypes.ToArray ();
		}

		static public bool IsAllias (this System.Type type)
		{
			if (type.GetAllias () == type.Name)
				return false;
			else
				return true;
		}

		static public bool IsSupportType (this System.Type type)
		{
			if (type == null)
				return false;
			List<string> listType = new List<string> ();
			listType.Add ("Color");
			listType.Add ("Vector3");

			if (listType.Contains (type.Name))
				return true;

			return false;
		}

		static public object GetCachedTypeInstance (Type type)
		{
			object instance = null;
			if (!cachedInstance.TryGetValue (type, out instance)) {
				instance = GetDefaultValue (type);
				cachedInstance.Add (type, instance);
			} else {
				if (instance == null) {
					cachedInstance.Remove (type);
					instance = GetCachedTypeInstance (type);
				}
			}

			return instance;
		}

		static public object GetDefaultValue (Type type)
		{
			try {
				return Activator.CreateInstance (type);
			} catch {
				UnuLogger.LogError ("Cannot get default value of target type!");
				return null;
			}
		}
	}
}