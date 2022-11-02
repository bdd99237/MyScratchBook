using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Painter : MonoBehaviour
{
	/*
    public Texture2D baseTex; //도화지의 역할.
    public Texture2D baseTex_2; //두번째 도화지

    Vector2 dragStart; //드래그 시작위치 저장
    Vector2 dragEnd; //드래그 끝위치 저장


    //툴의 상태.
    public enum Tool
    {
        None,
        Line,
        Brush,
        Eraser,
        Vector
    }

	public enum Mode
	{
		Layer_1,
		Layer_2,
		Scratch
	}


    public Tool tool = Tool.Line; //툴의 상태를 의미하는듯.
    int tool2 = 1;
	public Mode mode = Mode.Layer_1;
	int mode2 = 0;
    public Samples AntiAlias = Samples.Samples4; //Drawing.cs에 있음. 이미지가 깨지는걸 관리하는듯??
    public Texture[] tool_Imgs; //화면에 나타낼 툴들의 이미지.
	public Texture[] mode_Imgs; //화면에 나타낼 툴들의 이미지.
    public Texture2D colorCircle; //색상판
    public float lineWidth = 1; //라인의 크기
    public float strokeWidth = 1; //스트로크 크기(라인그을때 사용)
    public Color col = Color.white; //색상.(본 색상)
    public Color col2 = Color.white; //색상(라인을 그릴때 외곽선 색상)
    public GUISkin g_Skin; //GUI스킨
    public LineTool lineTool;
    public BrushTool brush;
    public EraserTool eraser;
    public Stroke stroke;
    public int zoom = 1; //줌. 이미지가 확대됨.(현재 이미지 이동기능이 없음)
    public List<BezierPoint> bezierPoints = new List<BezierPoint>();
    Vector2 preDrag;

    void OnGUI()
    {
        GUI.skin = g_Skin; //GUI스킨적용
        GUILayout.BeginArea(new Rect(5, 5, 100 + baseTex.width * zoom, baseTex.height * zoom), "", "Box"); //GUILayout시작(범위생성 및 스킨적용)
        GUILayout.BeginArea(new Rect(0, 0, 100, baseTex.height * zoom)); //툴부분 범위 생성.
        tool2 = GUILayout.Toolbar(tool2, tool_Imgs, "Tool"); //툴바를 만든다. 선택된 이미지의 번호저장.
        //tool = (Tool)System.Enum.Parse(typeof(Tool), tool2.ToString()); //수를 글형식으로 바꾸고 그것을 열거형tool형식으로 변환.
        GUILayout.Label("Drawing Options"); //글자출력
        GUILayout.Space(10); //10만큼 띄움
        switch (tool)
        {
            case Tool.Line:
                GUILayout.Label("Size " + Mathf.Round(lineTool.width * 10) / 10); //사이즈 표시
                lineTool.width = GUILayout.HorizontalSlider(lineTool.width, 0, 40); //브러쉬 사이즈 조절 바
			col = GUIControls.RGBCircle(col, "", colorCircle); //색상표 출력. 색상설정.
                break;
            case Tool.Brush:
                GUILayout.Label("Size " + Mathf.Round(brush.width * 10) / 10); //화면에 글출력
                brush.width = GUILayout.HorizontalSlider(brush.width, 0, 40); //화면에 슬라이더를 만들고 그 값을 받는다.
                GUILayout.Label("Hardness " + Mathf.Round(brush.hardness * 10) / 10);
                brush.hardness = GUILayout.HorizontalSlider(brush.hardness, 0.1f, 50);
			col = GUIControls.RGBCircle(col, "", colorCircle);
                break;
            case Tool.Eraser:
                GUILayout.Label("Size " + Mathf.Round(eraser.width * 10) / 10);
                eraser.width = GUILayout.HorizontalSlider(eraser.width, 0, 50);
                GUILayout.Label("Hardness " + Mathf.Round(eraser.hardness * 10) / 10);
                eraser.hardness = GUILayout.HorizontalSlider(eraser.hardness, 1, 50);
                break;
        }
        if (tool == Tool.Line) 
        {
            stroke.enabled = GUILayout.Toggle(stroke.enabled, "Stroke"); //스트로크 토글 생성.
            GUILayout.Label("Stroke Width " + Mathf.Round(stroke.width * 10) / 10);
            stroke.width = GUILayout.HorizontalSlider(stroke.width, 0, lineWidth);
            GUILayout.Label("Secondary Color");
			col2 = GUIControls.RGBCircle(col2, "", colorCircle);
        }

		GUILayout.Space (10);
		mode2 = GUILayout.Toolbar(mode2, mode_Imgs, "Tool"); //툴바를 만든다. 선택된 이미지의 번호저장.
		mode = (Mode)System.Enum.Parse(typeof(Mode), mode2.ToString()); //수를 글형식으로 바꾸고 그것을 열거형tool형식으로 변환.


        GUILayout.EndArea(); //GUI레이아웃 공간설정 종료.

		if (mode == Mode.Layer_1) {
			GUI.DrawTexture (new Rect (100, 0, baseTex.width * zoom, baseTex.height * zoom), baseTex); //그릴부분생성.
		} else if (mode == Mode.Layer_2) {
			GUI.DrawTexture (new Rect (100, 0, baseTex.width * zoom, baseTex.height * zoom), baseTex_2);
		} else if (mode == Mode.Scratch) {
			GUI.DrawTexture(new Rect(100, 0, baseTex.width * zoom, baseTex.height * zoom), baseTex); //그릴부분생성.
			GUI.DrawTexture(new Rect(100, 0, baseTex.width * zoom, baseTex.height * zoom), baseTex_2); //그릴부분생성.
		}
		GUI.enabled = true;
        GUILayout.EndArea();
    }

    void Update()
    {
		if (mode == Mode.Scratch) {
			col.a = 0f;
			col2.a = 0f;
		} else {
			col.a = 1f;
			col2.a = 1f;
		}

        Rect imgRect = new Rect(5 + 100, 5, baseTex.width * zoom, baseTex.height * zoom); //이미지 범위(5,5띄우고 툴바100을 뺀 부분부터가 시작위치)
        Vector2 mouse = Input.mousePosition;
        mouse.y = Screen.height - mouse.y; //마우스와 스크린의 기준점 방향이 다르다. 스크린은 위에서 아래., 마우스는 아래에서 위, x는 좌에서 우로 동일.

        if (Input.GetKeyDown("t"))
        {
            test();
        }
        //마우스 클릭된 순간. 드래그 스타트, 엔드 동시에 초기화함.
        if (Input.GetKeyDown("mouse 0"))
        {
            if (imgRect.Contains(mouse)) //마우스의 클릭위치가 범위안에 포함되는가
            {
                if (tool == Tool.Vector)
                {
                    Vector2 m2 = mouse - new Vector2(imgRect.x, imgRect.y); //그림범위에서의 그려지는 위치를 저장하고 있게됨.
                    m2.y = imgRect.height - m2.y; //이럴경우 마우스 기준의 실제 위치가 됨.
                    List<BezierPoint> bz = new List<BezierPoint>();
                    bz.AddRange(bezierPoints); //기존에 저장된 베지어 포인트를 불러와 저장.
                    bz.Add(new BezierPoint(m2, m2 - new Vector2(50, 10), m2 + new Vector2(50, 10))); //베지어 포인트 추가
                    bezierPoints.Clear(); //기존의 데이터를 제거
                    bezierPoints.AddRange(bz); //bz의 리스트를 전부 추가
					if (mode == Mode.Layer_1) {
						Drawing.DrawBezier (bezierPoints, lineTool.width, col, baseTex); //베지어를 그림.
					} else if (mode == Mode.Layer_2 || mode == Mode.Scratch) {
						Drawing.DrawBezier (bezierPoints, lineTool.width, col, baseTex_2); //베지어를 그림.
					}
                }

                dragStart = mouse - new Vector2(imgRect.x, imgRect.y); //그림범위에서의 드래그 시작점.
                dragStart.y = imgRect.height - dragStart.y; //마우스 기준 실제 위치.
                dragStart.x = Mathf.Round(dragStart.x / zoom); //줌이된 경우 좌표가 배가 되므로 나누어 x위치 파악.(소수점 .5초과 반올림)
                dragStart.y = Mathf.Round(dragStart.y / zoom);

                dragEnd = mouse - new Vector2(imgRect.x, imgRect.y); //드래그 끝지점.
                dragEnd.x = Mathf.Clamp(dragEnd.x, 0, imgRect.width); //그림범위를 벗어나지 않도록 값조절.
                dragEnd.y = imgRect.height - Mathf.Clamp(dragEnd.y, 0, imgRect.height);
                dragEnd.x = Mathf.Round(dragEnd.x / zoom);
                dragEnd.y = Mathf.Round(dragEnd.y / zoom);
            }
            else
            {
                dragStart = Vector3.zero; //범위 밖이면 제로 위치가 시작점.
            }

        }
        //누른상태로 지속될떄. 엔드를 갱신
        if (Input.GetKey("mouse 0"))
        {
            if (dragStart == Vector2.zero)
            {
                return;
            }
            dragEnd = mouse - new Vector2(imgRect.x, imgRect.y);
            dragEnd.x = Mathf.Clamp(dragEnd.x, 0, imgRect.width);
            dragEnd.y = imgRect.height - Mathf.Clamp(dragEnd.y, 0, imgRect.height);
            dragEnd.x = Mathf.Round(dragEnd.x / zoom);
            dragEnd.y = Mathf.Round(dragEnd.y / zoom);

            if (tool == Tool.Brush)
            {
                Brush(dragEnd, preDrag);
            }
            if (tool == Tool.Eraser)
            {
                Eraser(dragEnd, preDrag);
            }
        }
        if (Input.GetKeyUp("mouse 0") && dragStart != Vector2.zero)
        {
            if (tool == Tool.Line)
            {
                dragEnd = mouse - new Vector2(imgRect.x, imgRect.y);
                dragEnd.x = Mathf.Clamp(dragEnd.x, 0, imgRect.width);
                dragEnd.y = imgRect.height - Mathf.Clamp(dragEnd.y, 0, imgRect.height);
                dragEnd.x = Mathf.Round(dragEnd.x / zoom);
                dragEnd.y = Mathf.Round(dragEnd.y / zoom);
                Debug.Log("Draw Line");
                Drawing.numSamples = AntiAlias;
				if (mode == Mode.Layer_1) {
					if (stroke.enabled) { //스트로크있을때.
						baseTex = Drawing.DrawLine (dragStart, dragEnd, lineTool.width, col, baseTex, true, col2, stroke.width); //외곽선 있게 그리기.
					} else {
						baseTex = Drawing.DrawLine (dragStart, dragEnd, lineTool.width, col, baseTex); //외곽선 없게 그리기.
					}
				} else {
					if (stroke.enabled) { //스트로크있을때.
						baseTex_2 = Drawing.DrawLine (dragStart, dragEnd, lineTool.width, col, baseTex_2, true, col2, stroke.width); //외곽선 있게 그리기.
					} else {
						baseTex_2 = Drawing.DrawLine (dragStart, dragEnd, lineTool.width, col, baseTex_2); //외곽선 없게 그리기.
					}
				}
            }
            //드로우 상태 초기화
            dragStart = Vector2.zero;
            dragEnd = Vector2.zero;
        }
        preDrag = dragEnd; //초기화
    }

    void Brush(Vector2 p1, Vector2 p2)
    {
        Drawing.numSamples = AntiAlias;
        if (p2 == Vector2.zero)
        {
            //프리드래그가 제로라면 현재 드래그엔드 값을 넘겨줌.
            p2 = p1;
        }
		if (mode == Mode.Layer_1) {
			Drawing.PaintLine (p1, p2, brush.width, col, brush.hardness, baseTex); //그리는 부분.
			baseTex.Apply (); //textur2d저장.
		} else {
			Drawing.PaintLine (p1, p2, brush.width, col, brush.hardness, baseTex_2); //그리는 부분.
			baseTex_2.Apply (); //textur2d저장.
		}
    }

    void Eraser(Vector2 p1, Vector2 p2)
    {
        Drawing.numSamples = AntiAlias;
        if (p2 == Vector2.zero)
        {
            p2 = p1;
        }
		if (mode == Mode.Layer_1) {
			Drawing.PaintLine (p1, p2, eraser.width, Color.white, eraser.hardness, baseTex);
			baseTex.Apply ();
		} else {
			Drawing.PaintLine (p1, p2, eraser.width, Color.white, eraser.hardness, baseTex_2);
			baseTex_2.Apply ();
		}
    }

    void test()
    {
        float startTime = Time.realtimeSinceStartup;
        int w = 100;
        int h = 100;
        BezierPoint p1 = new BezierPoint(new Vector2(10, 0), new Vector2(5, 20), new Vector2(20, 0));
        BezierPoint p2 = new BezierPoint(new Vector2(50, 10), new Vector2(40, 20), new Vector2(60, -10));
        BezierCurve c = new BezierCurve(p1.main, p1.control_2, p2.control_1, p2.main);
        p1.curve_2 = c;
        p2.curve_1 = c;
        Vector2 elapsedTime = new Vector2((Time.realtimeSinceStartup - startTime) * 10, 0);
        float startTime2 = Time.realtimeSinceStartup;
        for (int i = 0; i < w * h; i++)
        {
            Mathfx.IsNearBezier(new Vector2(Random.value * 80, Random.value * 30), p1, p2, 10);
        }

        Vector2 elapsedTime2 = new Vector2((Time.realtimeSinceStartup - startTime2) * 10, 0);
        Debug.Log("Drawing took " + elapsedTime.ToString() + "  " + elapsedTime2.ToString());

    }
    */
}

/*
[System.Serializable]
public class LineTool
{
    public float width = 1; //두께
}

[System.Serializable]
public class EraserTool
{
        public float width = 1;
        public float hardness = 1; //단단하기?? 진하게인듯?
}

[System.Serializable]
public class BrushTool
{
        public float width = 1;
        public float hardness = 0;
        public float spacing = 10;
}

[System.Serializable]
public class Stroke
{
        public bool enabled = false; //외곽선의 유무
        public float width = 1;
}
*/