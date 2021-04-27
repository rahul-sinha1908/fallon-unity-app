using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fallon {
	public class MainMenuScript : MonoBehaviour {

		[SerializeField]
		private GameObject container;

		[SerializeField]
		private SceneButtonScript prefab;

		// Start is called before the first frame update
		void Start() {
			Request req = new Request(GameApiScript.urlScene(), GameApiScript.bodyGetScene(), RequestType.GET);
			WebCallScript.instance.callUrl(req, (errNode, sucNode) => {
				if (errNode != null) {
					Debug.Log(errNode);
				} else {
					Debug.Log(sucNode);
					instantiateButtons(sucNode["scenes"]);
				}
			});
		}

		public void instantiateButtons(JSONNode node) {
			
		}

		// Update is called once per frame
		void Update() {

		}

	}
}