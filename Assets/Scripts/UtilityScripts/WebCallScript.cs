using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System;

namespace Fallon {
	public class WebCallScript : MonoBehaviour {

		public static WebCallScript instance;

		private void Awake() {
			if (instance == null)
				instance = this;

			// if (UserPrefs.isNewDay()) {
			// 	CachingScript.deleteAllTextRequestCache();
			// }
		}

		public void callUrl(Request request, Action<JSONNode, JSONNode> callback) {
			enumCallUrlForText(request, callback, false);
		}

		public void callUrl(string url, Action<JSONNode, JSONNode> callback) {
			Request request = new Request(url, GameApiScript.bodyEmpty(), RequestType.GET);
			enumCallUrlForText(request, callback, false);
		}

		public void callTextCachedUrl(string url, Action<JSONNode, JSONNode> callback) {
			Request request = new Request(url, GameApiScript.bodyEmpty(), RequestType.GET);
			if (!request.isCacheble()) {
				callUrl(request, callback);
				return;
			}
			// string response = CachingScript.checkTextRequestCache(request);
			string response=null;
			if (response == null) {
				enumCallUrlForText(request, callback, true);
			} else {
				callback(null, JSON.Parse(response));
			}
		}

		public void callTextCachedUrl(Request request, Action<JSONNode, JSONNode> callback) {
			if(!request.isCacheble()){
				callUrl(request, callback);
				return;
			}
			// string response = CachingScript.checkTextRequestCache(request);
			string response = null;
			if (response == null) {
				enumCallUrlForText(request, callback, true);
			} else {
				callback(null, JSON.Parse(response));
			}
		}

		public void downloadImage(string relUrl, Action<JSONNode, Sprite> callback, bool shouldSeeCache = true) {
			Request request = new Request(relUrl, GameApiScript.bodyEmpty(), RequestType.GET);
			string url = request.getURL();

			// Sprite sprite = CachingScript.getSpriteFromCache(url);
			Sprite sprite = null;

			//if (Reaug.GameData.showLogs) Debug.Log("Image Cached URL  : " + url);
			if (shouldSeeCache && sprite != null) {
				callback(null, sprite);
			} else {
				enumDownloadImage(request, callback);
			}
		}
		public void downloadImage(Request request, Action<JSONNode, Sprite> callback, bool shouldSeeCache=true) {
			string url = request.getURL();

			// Sprite sprite = CachingScript.getSpriteFromCache(url);
			Sprite sprite = null;
			//if (Reaug.GameData.showLogs) Debug.Log("Image Cached URL  : " + url);
			if(shouldSeeCache && sprite != null) {
				callback(null, sprite);
			} else {
				enumDownloadImage(request, callback);
			}
		}
		public void downloadBinary(Request request, Action<JSONNode, byte[]> callback) {
			string url = request.getURL();
			enumDownloadBinary(request, callback);
		}


		//--------------   IENUMERATOR   ----------------------
		private void enumDownloadBinary(Request request, Action<JSONNode, byte[]> callback) {
			StartCoroutine(callForBinary(request, (errNode, bytes) => {
				if (errNode == null) {
					callback(null, bytes);
				} else {
					callback(errNode, null);
				}
			}));
		}
		private void enumDownloadImage(Request request, Action<JSONNode, Sprite> callback) {
			StartCoroutine(callForBinary(request, (errNode, texture) => {
				if (errNode == null) {
					// if (request.isCacheble())
					// 	CachingScript.addImageInCache(request.getURL(), texture.EncodeToPNG());
					Sprite sprite = Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), new Vector2(0.5f, 0.5f));
					callback(null, sprite);
				} else {
					callback(errNode, null);
				}
			}));
		}

		private void enumCallUrlForText(Request request, Action<JSONNode, JSONNode> callback, bool alreadyCached) {
			StartCoroutine(call(request, (errNode, succNode) => {
				if (errNode == null) {
					// if(request.isCacheble())
					// 	CachingScript.addTextRequestCache(request, succNode.ToString());
					callback(null, succNode);
				} else {
					callback(errNode, null);
				}
			}, alreadyCached));
		}

		IEnumerator call(Request req, Action<JSONNode, JSONNode> callback, bool alreadyCached=false) {
			UnityWebRequest www = req.getWWW();
			//if (Reaug.GameData.showLogs) Debug.Log("Pinging URL : " + www.url+" : "+www.method);
			yield return www.SendWebRequest();
			if (www.isNetworkError) {
				Debug.Log("Network Error : " + req.isCacheble()+" : "+alreadyCached);
				if (req.isCacheble() && !alreadyCached) {
					callTextCachedUrl(req, callback);
					yield break;
				}
				Debug.Log(www.error);
				var errNode = JSON.Parse("{}");
				errNode["message"] = www.error;
				errNode["netError"] = true;
				callback(errNode, null);
				// FirebaseLogging.networkError(www.error);
			} else if (www.isHttpError) {
				JSONNode node = JSON.Parse(www.downloadHandler.text);
				try {
					// FirebaseLogging.webError(req, www.responseCode, node.ToString());
					//if (www.responseCode == APIResponseCode.tokenError) {
					//	//TODO Create a token and send back a response
					//	// waitForToken(req, callback);
					//} else if (www.responseCode == APIResponseCode.sessionError) {
					//	//TODO Logout and display a message
					//	UtilityScript.logout();
					//} else if (www.responseCode == APIResponseCode.popUpError) {
					//	//Display a popup message
					//	UtilityScript.showDialogAlert(node["data"]["message"].Value, node["data"]["title"].Value);
					//} else if (www.responseCode == APIResponseCode.roomError) {
					//	//TODO Logout from the firebase chat room
					//	// if(GameData.getInstance().isAdmin)
					//	// 	AdminCanvasScript.instance.removeFromChatRoom();
					//	// else
					//	// 	MainCanvasScript.instance.removeFromChatRoom();
					//} else {
					//	Debug.Log("Stange Error : " + www.downloadHandler.text);
					//	if (node == null) {
					//		node = JSON.Parse("{message:\"\"}");
					//		node["message"] = www.downloadHandler.text;
					//	}
					//	callback(node, null);
					//}
				} catch (Exception e) {
					Debug.LogError("API Error: " + e);
					var errNode = JSON.Parse("{}");
					errNode["message"] = e.ToString();
					callback(errNode, null);
				}
			} else {
				callback(null, JSON.Parse(www.downloadHandler.text));
			}
		}
		IEnumerator callForBinary(Request req, Action<JSONNode, Texture2D> callback) {
			UnityWebRequest www = req.getWWW(true);

			yield return www.SendWebRequest();
			if (www.isNetworkError) {
				Debug.Log(www.error);
				var errNode = JSON.Parse("{}");
				errNode["message"] = www.error;
				errNode["netError"] = true;
				callback(errNode, null);
			} else if (www.isHttpError) {
				JSONNode node = JSON.Parse(www.downloadHandler.text);
				try {
					//if (www.responseCode == APIResponseCode.tokenError) {
					//	//TODO Create a token and send back a response
					//	// waitForToken(req, callback);
					//} else if (www.responseCode == APIResponseCode.sessionError) {
					//	//TODO Logout and display a message
					//	UtilityScript.logout();
					//} else if (www.responseCode == APIResponseCode.popUpError) {
					//	//Display a popup message
					//	UtilityScript.showDialogAlert(node["data"]["message"].Value, node["data"]["title"].Value);
					//} else {
					//	callback(node, null);
					//}
				} catch (Exception e) {
					Debug.LogError("API Error: " + e);
					callback(node, null);
				}
			} else {
				callback(null, DownloadHandlerTexture.GetContent(www));
			}
		}
		IEnumerator callForBinary(Request req, Action<JSONNode, byte[]> callback) {
			UnityWebRequest www = req.getWWW(true);

			yield return www.SendWebRequest();
			if (www.isNetworkError) {
				Debug.Log(www.error);
				var errNode = JSON.Parse("{}");
				errNode["message"] = www.error;
				errNode["netError"] = true;
				callback(errNode, null);
			} else if (www.isHttpError) {
				JSONNode node = JSON.Parse(www.downloadHandler.text);
				try {
					//if (www.responseCode == APIResponseCode.tokenError) {
					//	//TODO Create a token and send back a response
					//	// waitForToken(req, callback);
					//} else if (www.responseCode == APIResponseCode.sessionError) {
					//	//TODO Logout and display a message
					//	UtilityScript.logout();
					//} else if (www.responseCode == APIResponseCode.popUpError) {
					//	//Display a popup message
					//	UtilityScript.showDialogAlert(node["data"]["message"].Value, node["data"]["title"].Value);
					//} else {
					//	callback(node, null);
					//}
				} catch (Exception e) {
					Debug.LogError("API Error: " + e);
					callback(node, null);
				}
			} else {
				callback(null, www.downloadHandler.data);
			}
		}

		// private void waitForToken(Request req, Action<JSONNode, JSONNode> callback) {
		// 	StartCoroutine(enumCallForToken((errNode, sucNode) => {
		// 		if (errNode) {
		// 			callback(errNode, null);
		// 		} else {
		// 			StartCoroutine(call(req, callback));
		// 		}
		// 	}));
		// }
		// private void waitForToken(Request req, Action<JSONNode, Texture2D> callback) {
		// 	StartCoroutine(enumCallForToken((errNode, sucNode) => {
		// 		if (errNode) {
		// 			callback(errNode, null);
		// 		} else {
		// 			StartCoroutine(callForBinary(req, callback));
		// 		}
		// 	}));
		// }
		// private void waitForToken(Request req, Action<JSONNode, byte[]> callback) {
		// 	StartCoroutine(enumCallForToken((errNode, sucNode) => {
		// 		if (errNode) {
		// 			callback(errNode, null);
		// 		} else {
		// 			StartCoroutine(callForBinary(req, callback));
		// 		}
		// 	}));
		// }

		// public void getToken(Action<JSONNode, JSONNode> callback) {
		// 	StartCoroutine(enumCallForToken(callback));
		// }
		public void login(Dictionary<string, string> body, Action<JSONNode, JSONNode> callback) {
			Request req = new Request(GameApiScript.urlLogin(), body, RequestType.POST, CallType.Login);
			WebCallScript.instance.callUrl(req, (errNode, sucNode) => {
				if (errNode != null) {
					Debug.Log("Error : " + errNode.ToString());
				} else {
					var sessToken = sucNode["data"]["sessionToken"].Value;
					//UserPrefs.setSessionToken(sessToken);
				}
				callback(errNode, sucNode);
			});
		}

		// private IEnumerator enumCallForToken(Action<JSONNode, JSONNode> callback) {
		// 	UnityWebRequest www = Request.getTokenRequest();
		// 	yield return www.SendWebRequest();
		// 	if (www.isNetworkError) {
		// 		if (Reaug.GameData.showLogs) Debug.Log(www.error);
		// 		var errNode = JSON.Parse("{}");
		// 		errNode["message"] = www.error;
		// 		callback(errNode, null);
		// 	} else if (www.isHttpError) {
		// 		JSONNode node = JSON.Parse(www.downloadHandler.text);
		// 		try {
		// 			if (www.responseCode == APIResponseCode.tokenError) {
		// 				//TODO Create a token and send back a response
		// 			} else if (www.responseCode == APIResponseCode.sessionError) {
		// 				//TODO Logout and display a message

		// 			} else if (www.responseCode == APIResponseCode.popUpError) {
		// 				//Display a popup message
		// 				UtilityScript.showDialogAlert(node["data"]["message"].Value, node["data"]["title"].Value);
		// 			} else {
		// 				callback(node, null);
		// 			}
		// 		} catch (Exception e) {
		// 			Debug.LogError("API Error: " + e);
		// 			callback(node, null);
		// 		}
		// 	} else {
		// 		//if (Reaug.GameData.showLogs) Debug.Log("Token Output : " + www.downloadHandler.text);
		// 		JSONNode node = JSON.Parse(www.downloadHandler.text);
		// 		var accountToken = node["data"]["accountToken"].Value;
		// 		UserPrefs.setAccountToken(accountToken);
		// 		callback(null, node);
		// 	}
		// }
	}
}