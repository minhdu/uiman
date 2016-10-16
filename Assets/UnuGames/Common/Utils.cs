using System;
using System.ComponentModel;
using UnityEngine;
using System.Collections.Generic;

namespace UnuGames {
    public class Utils {
        static public T ParseEnum<T>(string value)
        {
            try {
                return (T)System.Enum.Parse(typeof(T), value, true);
            }catch {
                return default(T);
            }
        }
    }

	static public class EnumExtensions {
#if !UNITY_EDITOR
		static Dictionary<Enum, string> caches = new Dictionary<Enum, string> ();
#endif

		// This extension method is broken out so you can use a similar pattern with 
		// other MetaData elements in the future. This is your base method for each.
		static public T GetAttribute<T>(this Enum value) where T : Attribute {
			var type = value.GetCachedType();
			var memberInfo = type.GetMember(value.ToString());
			var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
			return (T)attributes[0];
		}
		
		// This method creates a specific call to the above method, requesting the
		// Description MetaData attribute.
		static public string ToName(this Enum value) {
#if !UNITY_EDITOR
			string name = null;
			if (!caches.TryGetValue (value, out name)) {
				var attribute = value.GetAttribute<DescriptionAttribute> ();
				name = (attribute == null ? value.ToString () : attribute.Description);
				caches.Add(value, name);
			}

			return name;
#else
			var attribute = value.GetAttribute<DescriptionAttribute> ();
			return (attribute == null ? value.ToString () : attribute.Description);
#endif
		}
		
	}

	static public class AnimatorExtensions {
		static public void EnableAndPlay (this Animator animator, string stateName) {
			if (animator != null) {
				animator.enabled = true;
				animator.Play(Animator.StringToHash(stateName), 0, 0);
			}
		}
	}
}
