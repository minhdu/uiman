using UnityEngine;
using System.Collections;
using System;

public class AsyncResourcesLoader : MonoBehaviour
{

	string mPath;

	void Awake ()
	{
		DontDestroyOnLoad (gameObject);
	}

	public void Load<T> (string path, Action<T> onLoaded) where T : UnityEngine.Object
	{
		mPath = path;
		StartCoroutine (LoadAsync<T> (onLoaded));
	}

	IEnumerator LoadAsync<T> (Action<T> onLoaded) where T : UnityEngine.Object
	{
		ResourceRequest request = Resources.LoadAsync<T> (mPath);
		yield return request;
	
		if (request.asset != null)
			ResourceFactory.Cache (mPath, (T)request.asset);

		onLoaded ((T)request.asset);

		yield return new WaitForEndOfFrame ();

		if(Application.isPlaying)
			Destroy (gameObject);
		else
			DestroyImmediate (gameObject);
	}
}
