using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsyncResourcesLoader : MonoBehaviour
{

	string mPath;

	void Awake ()
	{
		DontDestroyOnLoad (gameObject);
	}

	public void Load<T> (string path, System.Action<T> onLoaded) where T : Object
	{
		mPath = path;
		StartCoroutine (LoadAsync<T> (onLoaded));
	}

	IEnumerator LoadAsync<T> (System.Action<T> onLoaded) where T : Object
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
