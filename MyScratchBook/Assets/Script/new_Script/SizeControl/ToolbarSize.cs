using UnityEngine;
using System.Collections;

public class ToolbarSize : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width, Screen.height/11);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
