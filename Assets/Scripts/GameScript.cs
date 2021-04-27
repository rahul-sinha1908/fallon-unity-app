using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fallon {
	public class GameScript : MonoBehaviour {

		public MainMenuScript mainMenu;
		public GamePlayScript gameMenu;

		public static GameScript instance;

		private void Awake() {
			instance = this;
		}

		// Start is called before the first frame update
		void Start() {
			mainMenu.gameObject.SetActive(true);
			gameMenu.gameObject.SetActive(false);
		}

		public void loadGame(JSONNode node) {
			mainMenu.gameObject.SetActive(false);
			gameMenu.gameObject.SetActive(true);
			gameMenu.loadGame(node);
		}
		public void returnToScene() {
			mainMenu.gameObject.SetActive(true);
			gameMenu.gameObject.SetActive(false);
		}
		// Update is called once per frame
		void Update() {

		}
	}
}
