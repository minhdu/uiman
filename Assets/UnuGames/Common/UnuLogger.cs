using UnityEngine;
using System.Collections;

public class UnuLogger
{

	static private bool _enableAllLog = true;

	static public bool EnableAllLog {
		get { return _enableAllLog; }
		set {
			_enableAllLog = value;
		}
	}

	static private bool _enableLogInfo = true;

	static public bool EnableLogInfo {
		get { return _enableLogInfo; }
		set {
			_enableLogInfo = value;
		}
	}

	static private bool _enableLogWarning = true;

	static public bool EnableLogWarning {
		get { return _enableLogWarning; }
		set {
			_enableLogWarning = value;
		}
	}

	static private bool _enableLogError = true;

	static public bool EnableLogError {
		get { return _enableLogError; }
		set {
			_enableLogError = value;
		}
	}

	static public void Log (object message)
	{
#if UNITY_DEBUG || UNITY_EDITOR
		if (EnableAllLog || EnableLogInfo)
			Debug.Log (message);
#endif
	}

	static public void LogFormat (string message, params object[] args)
	{
#if UNITY_DEBUG || UNITY_EDITOR
		if (EnableAllLog || EnableLogInfo)
			Debug.LogFormat (message, args);
#endif
	}

	static public void LogError (object message)
	{
#if UNITY_DEBUG || UNITY_EDITOR
		if (EnableAllLog || EnableLogError)
			Debug.LogError (message);
#endif
	}

	static public void LogErrorFormat (string message, params object[] args)
	{
#if UNITY_DEBUG || UNITY_EDITOR
		if (EnableAllLog || EnableLogError)
			Debug.LogErrorFormat (message, args);
#endif
	}

	static public void LogWarning (object message)
	{
#if UNITY_DEBUG || UNITY_EDITOR
		if (EnableAllLog || EnableLogWarning)
			Debug.LogWarning (message);
#endif
	}

	static public void LogFormatWarning (string message, params object[] args)
	{
#if UNITY_DEBUG || UNITY_EDITOR
		if (EnableAllLog || EnableLogWarning)
			Debug.LogWarningFormat (message, args);
#endif
	}
}
