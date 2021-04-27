using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Fallon {
	public enum CallType {
		General, Login, Token
	}
	public enum RequestType {
		POST, GET, PUT, DELETE
	}
	public class Request {
		string url;
		Dictionary<string, string> body, header;
		JSONNode jsonBody;
		RequestType requestType;

		private CallType callType;
		private bool isJsonType=false;

		public Request(string url, Dictionary<string, string> body, RequestType requestType, CallType callType = CallType.General, bool isAbsolute = false) {
			if(!isAbsolute)
				this.url = GameApiScript.formAbsoluteURL(url);
			else
				this.url = url;
			this.body = body;

			this.requestType = requestType;
			this.callType=callType;

			isJsonType=false;
			refreshHeaders();
		}
		public Request(string url, JSONNode body, RequestType requestType, CallType callType = CallType.General, bool isAbsolute = false) {
			if (!isAbsolute)
				this.url = GameApiScript.formAbsoluteURL(url);
			else
				this.url = url;

			this.jsonBody = body;
			this.body = null;

			this.requestType = requestType;
			this.callType=callType;

			isJsonType=true;
			refreshHeaders();
		}
		private void refreshHeaders(){
			header = new Dictionary<string, string>();
			//if (callType == CallType.General)
			//	header = UserPrefs.getGenericHeader();
			//else if (callType == CallType.Login)
			//	header = UserPrefs.getLoginHeader();
			//else
			//	header = UserPrefs.getTokenHeader();
			if(isJsonType)
				header.Add("Content-Type", "application/json");
		}
		public void addBody(string key, string value) {
			body.Add(key, value);
		}
		public UnityWebRequest getWWW(bool isImage=false) {
			refreshHeaders();
			UnityWebRequest www=null;
			if (requestType == RequestType.GET) {
				//Uri uri = new Uri(url);
				//TODO Add the restauranyt Parameters
				string murl = url;
				bool first = true;
				foreach (var key in body.Keys) {
					if (first) {
						murl += "?";
						first = false;
					} else
						murl += "&";
					murl += key + "=" + body[key];
				}

				if (requestType == RequestType.GET) {
					if (isImage) {
						www = UnityWebRequestTexture.GetTexture(murl);
					} else {
						www = UnityWebRequest.Get(murl);
					}
				} else if (requestType == RequestType.DELETE) {
					www = UnityWebRequest.Delete(murl);
				}
				
			} else {
				if (body == null) {
					if (requestType == RequestType.POST) {
						www = UnityWebRequest.Post(url, jsonBody.ToString().Trim());
					} else if (requestType == RequestType.PUT) {
						www = UnityWebRequest.Put(url, jsonBody.ToString());
					}
					www.SetRequestHeader("content-type", "application/json");
				} else {
					
					if (requestType == RequestType.POST) {
						var form = new WWWForm();
						foreach (var key in body.Keys) {
							form.AddField(key, body[key]);
						}
						www = UnityWebRequest.Post(url, form);
					} else if (requestType == RequestType.PUT) {
						var json = JSON.Parse("{}");
						foreach (var key in body.Keys) {
							json[key] = body[key];
						}
						www = UnityWebRequest.Put(url, json.ToString());
						www.SetRequestHeader("content-type", "application/json");
					}
				}
				
			}
			foreach (string key in header.Keys) {
				www.SetRequestHeader(key, header[key]);
			}

			return www;
		}

		public void getDownloadableSprite(Action<JSONNode, Sprite> callback) {
			WebCallScript.instance.downloadImage(this, callback);
		}

		public void getStringCachedResponse(Action<JSONNode, JSONNode> callback) {
			WebCallScript.instance.callTextCachedUrl(this, callback);
		}

		public void getDirectResponse(Action<JSONNode, JSONNode> callback) {
			WebCallScript.instance.callUrl(this, callback);
		}

		public string getURL() {
			return url;
		}

		public bool isCacheble() {
			if(requestType==RequestType.GET)
				return true;
			else
				return false;
		}
		public int getRequestType() {
			return (int)requestType;
		}
		public string getPREFString() {
			//TODO Think over the preference string. tricky part
			string str=url;
			if (requestType == RequestType.GET) {
				foreach (var key in body.Keys) {
					str += "~!@" + key + "=" + body[key];
				}
			}
			return str;
		}

	}
}
