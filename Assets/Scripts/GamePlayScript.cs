using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Fallon {
	public class GamePlayScript : MonoBehaviour {
		public Image bg, body, expression, hair, outfit, accessories;

		public TextMeshProUGUI dialogText;
		public Button nextButton;

		private JSONNode mNode;
		private int timelineNumber, dialogNumber;

		// Start is called before the first frame update
		void Start() {
			nextButton.onClick.AddListener(() => {
				loadNextSequence();
			});
		}

		private void loadNextSequence() {
			if (timelineNumber >= mNode.Count) {
				returnToScene();
				return;
			}
			var dialogs = mNode[timelineNumber]["dialogs"];
			if (dialogNumber >= dialogs.Count-1) {
				timelineNumber++;
				dialogNumber = -1;
				loadCharacter();
				loadNextSequence();
				return;
			}
			dialogNumber++;

			dialogText.text = dialogs[dialogNumber];
		}
		public void loadCharacter() {
			if (timelineNumber >= mNode.Count) {
				return;
			}
			var tl = mNode[timelineNumber];

			body.gameObject.SetActive(false);
			expression.gameObject.SetActive(false);
			hair.gameObject.SetActive(false);
			outfit.gameObject.SetActive(false);
			accessories.gameObject.SetActive(false);

			Debug.Log("Body : "+tl["body"]["file"].Value);
			if (!string.IsNullOrEmpty(tl["body"]["file"].Value))
				WebCallScript.instance.downloadImage(tl["body"]["file"].Value, (errNode, sprite) => {
					if (errNode != null) {
						Debug.Log(errNode);
					} else {
						body.sprite = sprite;
						body.gameObject.SetActive(true);
					}
				});
			if (!string.IsNullOrEmpty(tl["expression"]["file"].Value))
				WebCallScript.instance.downloadImage(tl["expression"]["file"].Value, (errNode, sprite) => {
					if (errNode != null) {
						Debug.Log(errNode);
					} else {
						expression.sprite = sprite;
						expression.gameObject.SetActive(true);
					}
				});
			if (!string.IsNullOrEmpty(tl["hair"]["file"].Value))
				WebCallScript.instance.downloadImage(tl["hair"]["file"].Value, (errNode, sprite) => {
					if (errNode != null) {
						Debug.Log(errNode);
					} else {
						hair.sprite = sprite;
						hair.gameObject.SetActive(true);
					}
				});
			if (!string.IsNullOrEmpty(tl["outfit"]["file"].Value))
				WebCallScript.instance.downloadImage(tl["outfit"]["file"].Value, (errNode, sprite) => {
					if (errNode != null) {
						Debug.Log(errNode);
					} else {
						outfit.sprite = sprite;
						outfit.gameObject.SetActive(true);
					}
				});
			for(int i=0;i< tl["accessories"].Count; i++) {
				var acc = tl["accessories"][i];
				if (!string.IsNullOrEmpty(acc["file"].Value))
					WebCallScript.instance.downloadImage(acc["file"].Value, (errNode, sprite) => {
						if (errNode != null) {
							Debug.Log(errNode);
						} else {
							var ins = Instantiate(accessories, accessories.transform.parent);
							ins.sprite = sprite;
							ins.gameObject.SetActive(true);
						}
					});
			}
		}
		public void returnToScene() {
			GameScript.instance.returnToScene();
		}

		public void loadGame(JSONNode node) {
			this.mNode = node["timeline"];

			Debug.Log(node["background"]["file"].Value);
			timelineNumber = 0;
			dialogNumber = -1;
			loadCharacter();
			WebCallScript.instance.downloadImage(node["background"]["file"].Value, (errNode, sprite) => {
				if (errNode != null) {
					Debug.Log(errNode);
				} else {
					bg.sprite = sprite;
				}
			});
			loadNextSequence();
		}

		// Update is called once per frame
		void Update() {

		}
	}
}