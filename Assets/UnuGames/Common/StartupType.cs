using System;

namespace UnuGames {
	public enum StartupType {
		NORMAL,
		PREFAB
	}

	public class StartupAttribute : Attribute
	{
		public StartupType Type { get; set; }
		public Type ParentType { get; set; }
		public string PrefabURL { get; set; }

		public StartupAttribute (StartupType type = StartupType.NORMAL)
		{
			Type = type;
			ParentType = null;
			PrefabURL = "";
		}

		public StartupAttribute (StartupType type, Type parentType, string prefabURL = "")
		{
			Type = type;
			ParentType = parentType;
			PrefabURL = prefabURL;
			if (PrefabURL == null)
				prefabURL = "";
		}
	}
}