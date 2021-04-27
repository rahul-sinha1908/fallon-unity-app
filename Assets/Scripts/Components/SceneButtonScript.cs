using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Fallon {
	public class SceneButtonScript : MonoBehaviour {

		private JSONNode mNode;

		[SerializeField]
		private TextMeshProUGUI sceneText;

		private Button button;
		public void initObject(JSONNode node) {
			this.mNode = node;
			sceneText.text = node["name"].Value;
		}

		// Start is called before the first frame update
		void Start() {
			button = GetComponent<Button>();
			button.onClick.AddListener(() => {
				loadScene();	
			});
		}

		private void loadScene() {
			GameScript.instance.loadGame(mNode);
		}

		// Update is called once per frame
		void Update() {

		}
	}
}