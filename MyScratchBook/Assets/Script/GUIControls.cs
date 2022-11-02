using UnityEngine;
using System.Collections;

public class GUIControls {
	/*
    //색상 슬라이더 부분.
    static Color RGBSlider(Color c, string label)
    {
        GUI.color = c;
        GUILayout.Label(label);
        GUI.color = Color.red;
        c.r = GUILayout.HorizontalSlider(c.r, 0, 1);
        GUI.color = Color.green;
        c.g = GUILayout.HorizontalSlider(c.g, 0, 1);
        GUI.color = Color.blue;
        c.b = GUILayout.HorizontalSlider(c.b, 0, 1);
        GUI.color = Color.white;
        return c;
    }

    //색상판 제어부분.
	static public Color RGBCircle(Color c, string label, Texture2D colorCircle)
    {
		//컬러사이클 공간.
		Rect r = GUILayoutUtility.GetAspectRect(1);//종횡비 1:1의 레이아웃공간을 가져옴.
		r.height = r.width -= 15;//높이를 너비-15로 조정.
		Rect r2 = new Rect(r.x + r.width + 5, r.y, 10, r.height);//명도조절 실린더 공간.
		//인자로 넘어온 색상을 HSBColor형식으로 변환. 
        HSBColor hsb = new HSBColor(c);//It is much easier to work with HSB colours in this case

        Vector2 cp = new Vector2(r.x + r.width / 2, r.y + r.height / 2); //r의 중심점.

        if (Input.GetMouseButton(0))
        {
            //입력이된 위치를 알아내는 부분.
            Vector2 InputVector = Vector2.zero;
            InputVector.x = cp.x - Event.current.mousePosition.x; //GUI.matrix를 반영한 포인트 값
            InputVector.y = cp.y - Event.current.mousePosition.y;

            float hyp = Mathf.Sqrt((InputVector.x * InputVector.x) + (InputVector.y * InputVector.y));
            if (hyp <= r.width / 2 + 5)
            {
                hyp = Mathf.Clamp(hyp, 0, r.width / 2);
                float a = Vector3.Angle(new Vector3(-1, 0, 0), InputVector);

                if (InputVector.y < 0)
                {
                    a = 360 - a;
                }

                hsb.h = a / 360;
                hsb.s = hyp / (r.width / 2);
            }
        }

        //조도?를 조절하는 부분.
        HSBColor hsb2 = new HSBColor(c);
        hsb2.b = 1;
        Color c2 = hsb2.ToColor();
        GUI.color = c2;
        hsb.b = GUI.VerticalSlider(r2, hsb.b, 1.0f, 0.0f, "BWSlider", "verticalsliderthumb");

        //색상판이미지의 색상조절부분.(조도의 조절에 따라 색상판이미지의 어둠기 변화)
        GUI.color = Color.white * hsb.b;
        GUI.color = new Color() { b = GUI.color.b, g = GUI.color.g, r = GUI.color.r , a = 1f }; //알파값도 감소하므로 알파값은 최대로 고정.
        GUI.Box(r, colorCircle, GUIStyle.none); //지정한 위치의 box안에 색상판이미지 적용. GUI스타일은 없음.

        Vector2 pos = (new Vector2(Mathf.Cos(hsb.h * 360 * Mathf.Deg2Rad), -Mathf.Sin(hsb.h * 360 * Mathf.Deg2Rad)) * r.width * hsb.s / 2);

        GUI.color = c;
        GUI.Box(new Rect(pos.x - 5 + cp.x, pos.y - 5 + cp.y, 10, 10), "", "ColorcirclePicker");
        GUI.color = Color.white;

        c = hsb.ToColor();
        return c;
    }
	*/
}
