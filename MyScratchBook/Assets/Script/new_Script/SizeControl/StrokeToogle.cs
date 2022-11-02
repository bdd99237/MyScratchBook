using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StrokeToogle : MonoBehaviour {

	public GameObject toggleBG;
	public GameObject text;

	// Use this for initialization
	void Start () {
	
		transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width / 2, Screen.height / 27);
		transform.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, Screen.height / 20);
		toggleBG.GetComponent<RectTransform>().sizeDelta = new Vector2 (Screen.width / 15, Screen.height / 25);
		toggleBG.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2 (Screen.width / 15, Screen.height / 25);
		text.GetComponent<Text> ().fontSize = Screen.width / 21;
		text.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (Screen.width / 14, 0);
		text.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width / 2 - Screen.width / 14, Screen.height / 27);
	}
	
	void Update()
	{
		ScratchPainter.Instance ().stroke.enabled = transform.GetComponentInChildren<Toggle> ().isOn;
	}
}
