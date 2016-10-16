using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ImageFactory : MonoBehaviour {

	static Dictionary<string, WWW> inprogressWWW = new Dictionary<string, WWW>();
	static Dictionary<string, Action<Texture>> loadTextureCallbacks = new Dictionary<string, Action<Texture>>();
	static Dictionary<string, Action<Sprite>> loadSpriteCallbacks = new Dictionary<string, Action<Sprite>>();
	static Dictionary<string, Texture> cache = new Dictionary<string, Texture>();
	Vector2 centerPivot = new Vector2(0.5f, 0.5f);

	static public void LoadSprite (string url, Action<Sprite> onLoadComplete) {
		LoadImage(url, null);
		if(loadSpriteCallbacks.ContainsKey(url))
			loadSpriteCallbacks[url] += onLoadComplete;
		else
			loadSpriteCallbacks.Add(url, onLoadComplete);
	}

	static public void LoadImage (string url, Action<Texture> onLoadComplete) {
		Texture texture = null;
		if(!cache.TryGetValue(url, out texture)) {
			WWW w = new WWW(url);
			inprogressWWW.Add(url, w);
			if(loadTextureCallbacks.ContainsKey(url))
				loadTextureCallbacks[url] += onLoadComplete;
			else
				loadTextureCallbacks.Add(url, onLoadComplete);
		}
		else {
			if(onLoadComplete != null)
				onLoadComplete(texture);
		}
	}

	void Update () {
		List<string> doneWWW = new List<string>();
		foreach(KeyValuePair<string, WWW> www in inprogressWWW) {
			string url = www.Key;
			WWW wVal = www.Value;
			if(wVal.isDone) {
				doneWWW.Add(url);
				loadTextureCallbacks[url](wVal.texture);
				if(loadSpriteCallbacks.ContainsKey(url)) {
					loadSpriteCallbacks[www.Key](Sprite.Create(wVal.texture, GetTextureRect(wVal.texture), centerPivot));
				}
			}
		}

		for(int i=0; i<doneWWW.Count; i++) {
			string url = doneWWW[i];
			inprogressWWW.Remove(url);
			loadTextureCallbacks.Remove(url);
			if(loadSpriteCallbacks.ContainsKey(url))
				loadSpriteCallbacks.Remove(url);
		}
	}

	Rect GetTextureRect (Texture texture) {
		return new Rect(0, 0, texture.width, texture.height);
	}
}
