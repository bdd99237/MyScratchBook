using UnityEngine;
using System.Collections;

public class PickerSize : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width / 30, Screen.height / 50);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
