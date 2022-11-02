using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WindowSize : MonoBehaviour {

	public enum Window
	{
		LayerChange,
		Line,
		Brush,
		Eraser
	}

	public Window window;

	// Use this for initialization
	void Start () {

		if (window == Window.LayerChange) {
			transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width/2, Screen.height/11);
			transform.GetComponent<GridLayoutGroup> ().cellSize = new Vector2 (Screen.height / 14, Screen.height / 14); //높이 기준으로 셀크기정함.
			transform.GetComponent<GridLayoutGroup> ().spacing = new Vector2 (Screen.width / 32, Screen.height / 64);
		}
		else if (window == Window.Line) {
			transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width/2, Screen.height/1.4f);
			transform.GetComponent<GridLayoutGroup> ().padding = new RectOffset (0, 0, 10, 0);
			transform.GetComponent<GridLayoutGroup> ().cellSize = new Vector2 (Screen.height / 4, Screen.height / 4);
			transform.GetComponent<GridLayoutGroup> ().spacing = new Vector2 (Screen.width / 32, Screen.height / 8);
		}
		else if (window == Window.Brush) {
			transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width/2, Screen.height/3);
			transform.GetComponent<GridLayoutGroup> ().cellSize = new Vector2 (Screen.height / 4, Screen.height / 4);
			transform.GetComponent<GridLayoutGroup> ().spacing = new Vector2 (Screen.width / 32, Screen.height / 24);
		}
		else if (window == Window.Eraser) {
			transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width/2, Screen.height/6);
			transform.GetComponent<GridLayoutGroup> ().cellSize = new Vector2 (Screen.height / 4, Screen.height / 8);
			transform.GetComponent<GridLayoutGroup> ().spacing = new Vector2 (Screen.width / 32, Screen.height / 24);
		}

	}
}
