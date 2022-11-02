using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ColorCircle : MonoBehaviour {

	public bool colorSelect; //true는 선색상, false는 외곽선 색상.
	public Color color; //색상을 받는 부분.
	public GameObject circle;
	public GameObject slider;
	public GameObject Picker;

	void Start()
	{
		//슬라이더를 초기화.
		slider.GetComponent<Slider>().value = 1;
	}

	void Update()
	{
		//색상 전달.
		if (colorSelect) {
			color = ScratchPainter.Instance ().col;
		} else {
			color = ScratchPainter.Instance ().col2;
		}

		RGBChange ();

		if (colorSelect) {
			ScratchPainter.Instance ().col = color;
		} else {
			ScratchPainter.Instance ().col2 = color;
		}
	}

	public void RGBChange()
	{
		Vector2 InputVector = Vector2.zero;
		Vector3 r = circle.GetComponent<BoxCollider>().size; //사이클의 콜리더 크기를 받음.

		HSBColor hsb = new HSBColor(color); //인자로 넘어온 색상을 HSBColor형식으로 변환. *

		//픽커 이동 및 색상 변경.
		if (Input.GetMouseButton(0))
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if(Physics.Raycast(ray, out hit))
				{
				//충돌한 물체가 사이클일때. 해당 충돌위치(!)로 픽커를 이동.
				if (hit.transform.gameObject == circle) {
					Picker.transform.position = hit.point;
				}
				}

			InputVector = Picker.GetComponent<RectTransform> ().anchoredPosition; //픽커의 RectTransform기준 좌표를 받음.


			float hyp = Mathf.Sqrt((InputVector.x * InputVector.x) + (InputVector.y * InputVector.y));

			if (hyp <= r.x / 2 + 5)
			{
				hyp = Mathf.Clamp(hyp, 0, r.x / 2);
				float a = Vector3.Angle(new Vector3(1, 0, 0), InputVector); //색상판 앵글조정부.

				if (InputVector.y < 0)
				{
					a = 360 - a;
				}

				hsb.h = a / 360;
				hsb.s = hyp / (r.x / 2);
			}
		}

		//사이클의 조도에 따른 색상변경.
		Color circleColor = circle.GetComponent<RawImage> ().color;
		circleColor = Color.white * hsb.b;
		circleColor.a = 1f;
		circle.GetComponent<RawImage> ().color = circleColor;

		//슬라이더의 배경 조도 제거
		HSBColor hsb2 = new HSBColor(color);
		hsb2.b = 1; //b가 0면 검정, 1이 흰색.

		//슬라이더 배경 색상 변경.
		slider.transform.FindChild("Background").GetComponent<Image>().color = hsb2.ToColor();

		//픽커의 색상변경.
		Picker.GetComponent<RawImage> ().color = hsb.ToColor ();

		//조도?를 조절하는 부분.
		hsb.b = slider.GetComponent<Slider> ().value; //b가 0이되면 기존의 색상이 날아가고 흰색이 됨.

		color = hsb.ToColor (); //변경된 색상 적용.
}
}
