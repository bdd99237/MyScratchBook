using UnityEngine;
using System.Collections;

public class SliderHandleSize : MonoBehaviour {

	public enum SliderDirection
	{
		Width,
		Height
	}

	public SliderDirection slider = SliderDirection.Width;

	// Use this for initialization
	void Start () {
		if (slider == SliderDirection.Width) {
			transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width / 15, 0);
		} else {
			transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, Screen.height / 25);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
