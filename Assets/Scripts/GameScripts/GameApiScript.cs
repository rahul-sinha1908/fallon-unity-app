using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;

namespace Fallon {
	public static class GameApiScript {
		//public const string baseURL = "http://192.168.0.54:3001";
		//public const string baseURL = "http://13.234.253.224:3001";
		public const string baseURL = "http://192.168.29.55:3001";

		//http://13.234.253.224:3001/
		//Unity Specific URLs ------------------------------------
		public static string urlLogin() {
			return ("/api/open");
		}


		public static string urlScene() {
			return ("/api/open/scenes");
		}

		public static string urlLogout() {
			return ("/api/open");
		}

		public static Dictionary<string, string> bodyGetScene() {
			Dictionary<String, String> dict = new Dictionary<string, string>() {};
			return dict;
		}

		
		public static Dictionary<string, string> bodyEmpty() {
			Dictionary<String, String> dict = new Dictionary<string, string>() {};
			return dict;
		}


		//----------------------------------------------------------------------------
		public static string formAbsoluteURL(string relUrl) {
			//TODO Make the relUrl prechecks
			return new Uri(new Uri(baseURL), relUrl).ToString();
		}
	}
}
