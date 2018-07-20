using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnuGames;

public class ImageFactory : SingletonBehaviour<ImageFactory> {

	static Dictionary<string, WWW> inprogressWWW = new Dictionary<string, WWW>();
	static Dictionary<string, Action<Texture>> loadTextureCallbacks = new Dictionary<string, Action<Texture>>();
	static Dictionary<string, Action<Sprite>> loadSpriteCallbacks = new Dictionary<string, Action<Sprite>>();
	static Dictionary<string, Texture> cache = new Dictionary<string, Texture>();
	Vector2 centerPivot = new Vector2(0.5f, 0.5f);

	public void LoadSprite (string url, Action<Sprite> onLoadComplete) {
		if(loadSpriteCallbacks.ContainsKey(url))
			loadSpriteCallbacks[url] += onLoadComplete;
		else
			loadSpriteCallbacks.Add(url, onLoadComplete);
		LoadTexture(url, null);
	}

	public void LoadTexture (string url, Action<Texture> onLoadComplete) {
		Texture texture = null;
		if(!cache.TryGetValue(url, out texture)) {
			if(loadTextureCallbacks.ContainsKey(url))
				loadTextureCallbacks[url] += onLoadComplete;
			else
				loadTextureCallbacks.Add(url, onLoadComplete);
			if (!inprogressWWW.ContainsKey (url)) {
				WWW w = new WWW (url);
				inprogressWWW.Add (url, w);
			}
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
			if(wVal != null && wVal.isDone) {
				doneWWW.Add(url);
				if(loadTextureCallbacks[url] != null)
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
