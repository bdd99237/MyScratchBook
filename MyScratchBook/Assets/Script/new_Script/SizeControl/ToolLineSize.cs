using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToolLineSize : MonoBehaviour {

	public enum ToolCheck
	{
		Line,
		Stroke,
		Brush,
		Erasor
	}

	public ToolCheck tool;

	public GameObject slider;

	public Text text;

	// Use this for initialization
	void Start () {
		text.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width/3, Screen.height/27);
		text.fontSize = Screen.width / 21;

		slider.GetComponent<Slider> ().value = 1;
	}
	
	// Update is called once per frame
	void Update () {
	
		switch(tool)
		{
		case ToolCheck.Line:
			ScratchPainter.Instance ().lineTool.width = slider.GetComponent<Slider> ().value;
			text.text = "Width : " + slider.GetComponent<Slider> ().value;
			break;

		case ToolCheck.Stroke:
			ScratchPainter.Instance ().stroke.width = slider.GetComponent<Slider> ().value;
			text.text = "Stroke : " + slider.GetComponent<Slider> ().value;
			break;

		case ToolCheck.Brush:
			ScratchPainter.Instance ().brush.width = slider.GetComponent<Slider> ().value;
			ScratchPainter.Instance ().brush.hardness = slider.GetComponent<Slider> ().value;
			text.text = "Brush : " + slider.GetComponent<Slider> ().value;
			break;

		case ToolCheck.Erasor:
			ScratchPainter.Instance ().eraser.width = slider.GetComponent<Slider> ().value;
			ScratchPainter.Instance ().eraser.hardness = slider.GetComponent<Slider> ().value;
			text.text = "Eraser : " + slider.GetComponent<Slider> ().value;
			break;
		}

	}
}
