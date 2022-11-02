using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum Tool
{
	Circle,
	Line,
	Brush,
	Eraser,
	Layer,
	Vector
}

public enum Mode
{
	Layer_1,
	Layer_2,
	Scratch
}

public class ScratchPainter : MonoBehaviour {

	//싱글톤
	private static ScratchPainter instance;

	private ScratchPainter(){}

	public static ScratchPainter Instance()
	{
		if (instance == null) {
			instance = new ScratchPainter ();
		}
		return instance;
	}

	public RawImage backGround;
	public RawImage layer_1; //첫번째 도화지
	public RawImage layer_2; //두번째 도화지

	Texture base1; //텍스쳐 저장.
	Texture base2;

	Texture2D testA; //2d텍스쳐 저장.
	Texture2D testB;

	Vector2 dragStart; //드래그 시작위치
	Vector2 dragEnd; //드래그 끝위치

	public Tool tool = Tool.Line; //툴의 상태를 의미하는듯.
	public int tool2 = 1;
	public Mode mode = Mode.Layer_1;
	int mode2 = 0;
	public Samples AntiAlias = Samples.Samples4; //Drawing.cs에 있음. 이미지가 깨지는걸 관리하는듯??
	//public float lineWidth = 1; //라인의 크기
	//public float strokeWidth = 1; //스트로크 크기(라인그을때 사용)
	public Color col = Color.white; //색상.(본 색상)
	public Color col2 = Color.white; //색상(라인을 그릴때 외곽선 색상)
	public GameObject[] tools;
	public LineTool lineTool;
	public BrushTool brush;
	public EraserTool eraser;
	public Stroke stroke;
	public List<BezierPoint> bezierPoints = new List<BezierPoint>();
	Vector2 preDrag;
	public float w_zoom; //넓이 줌.
	public float h_zoom; //높이 줌.

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		//텍스쳐 추출.
		base1 = layer_1.GetComponent<RawImage> ().texture;
		base2 = layer_2.GetComponent<RawImage> ().texture;

		//2d 텍스쳐로 변환.
		testA = (Texture2D)base1;
		testB = (Texture2D)base2;

		//화면 크기 대비 텍스쳐를 확대.
		w_zoom = (float) (Screen.width - 20) / testA.width;
		h_zoom = (float) (Screen.height - (Screen.height/11+20)) / testB.height;

		//도화지 사이즈.
		backGround.transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width - 20, Screen.height - (Screen.height / 11 + 20));
		layer_1.transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width - 20, Screen.height - (Screen.height / 11 + 20));
		layer_2.transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width - 20, Screen.height - (Screen.height / 11 + 20));

		//값 초기화.
		lineTool.width = 1;
		brush.width = 1;
		brush.hardness = 1;
		eraser.width = 1;
		eraser.hardness = 10;
		stroke.width = 1;
	}

	// Update is called once per frame
	void Update () {

		//사용중인 툴에 따라 색상변화.
		for (int i = 0; i < tools.Length; i++) {
			tools [i].GetComponent<Image> ().color = Color.white;
		}
		tools[(int)tool].GetComponent<Image> ().color = Color.gray;

		//모드에 따른 이미지 순서결정.
		if (mode == Mode.Layer_1) {
			layer_1.enabled = true;
			layer_2.enabled = false;

		} else if (mode == Mode.Layer_2) {
			layer_1.enabled = false;
			layer_2.enabled = true;
		} else if (mode == Mode.Scratch) {
			layer_1.enabled = true;
			layer_2.enabled = true;
		}

			//모드에 따른 알파값.
			if (mode == Mode.Scratch) {
				col = new Color(1f,1f,1f,0f);
                col2 = new Color(1f, 1f, 1f, 0f);
			} else {
				col.a = 1f;
				col2.a = 1f;
			}

			Rect imgRect = new Rect (10, 10, testA.width * w_zoom, testA.height * h_zoom); //이미지범위
			Vector2 mouse = Input.mousePosition; //마우스 위치 저장.
			mouse.y = Screen.height - mouse.y; //마우스와 스크린의 기준점 방향이 다르다.

			if (Input.GetKeyDown ("mouse 0")) {
				if(imgRect.Contains(mouse))
				{
					if (tool == Tool.Vector) {
						Vector2 m2 = mouse - new Vector2 (imgRect.x, imgRect.y); //그림범위 안에서의 위치
						m2.y = imgRect.height - m2.y; //마우스기준의 실제 위치.
						List<BezierPoint> bz = new List<BezierPoint>();
						bz.AddRange(bezierPoints); //기존에 저장된 베지어 포인트를 불러와 저장.
						bz.Add(new BezierPoint(m2, m2 - new Vector2(50, 10), m2 + new Vector2(50, 10))); //베지어 포인트 추가
						bezierPoints.Clear(); //기존의 데이터를 제거
						bezierPoints.AddRange(bz); //bz의 리스트를 전부 추가
						if (mode == Mode.Layer_1) {
							Drawing.DrawBezier (bezierPoints, lineTool.width, col, testA); //베지어를 그림.
						} else if (mode == Mode.Layer_2 || mode == Mode.Scratch) {
							Drawing.DrawBezier (bezierPoints, lineTool.width, col, testB); //베지어를 그림.
						}
					}

					dragStart = mouse - new Vector2(imgRect.x, imgRect.y); //그림범위에서의 드래그 시작점.
					dragStart.y = imgRect.height - dragStart.y; //마우스 기준 실제 위치.
					dragStart.x = Mathf.Round(dragStart.x / w_zoom); //줌이된 경우 좌표가 배가 되므로 나누어 x위치 파악.(소수점 .5초과 반올림)
					dragStart.y = Mathf.Round(dragStart.y / h_zoom);

					dragEnd = mouse - new Vector2(imgRect.x, imgRect.y); //드래그 끝지점.
					dragEnd.x = Mathf.Clamp(dragEnd.x, 0, imgRect.width); //그림범위를 벗어나지 않도록 값조절.
					dragEnd.y = imgRect.height - Mathf.Clamp(dragEnd.y, 0, imgRect.height);
					dragEnd.x = Mathf.Round(dragEnd.x / w_zoom);
					dragEnd.y = Mathf.Round(dragEnd.y / h_zoom);
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
				dragEnd.x = Mathf.Round(dragEnd.x / w_zoom);
				dragEnd.y = Mathf.Round(dragEnd.y / h_zoom);

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
					dragEnd.x = Mathf.Round(dragEnd.x / w_zoom);
					dragEnd.y = Mathf.Round(dragEnd.y / h_zoom);
					Debug.Log("Draw Line");
					Drawing.numSamples = AntiAlias;
					if (mode == Mode.Layer_1) {
						if (stroke.enabled) { //스트로크있을때.
							testA = Drawing.DrawLine (dragStart, dragEnd, lineTool.width, col, testA, true, col2, stroke.width); //외곽선 있게 그리기.
						} else {
							testA = Drawing.DrawLine (dragStart, dragEnd, lineTool.width, col, testA); //외곽선 없게 그리기.
						}
					} else {
						if (stroke.enabled) { //스트로크있을때.
							testB = Drawing.DrawLine (dragStart, dragEnd, lineTool.width, col, testB, true, col2, stroke.width); //외곽선 있게 그리기.
						} else {
							testB = Drawing.DrawLine (dragStart, dragEnd, lineTool.width, col, testB); //외곽선 없게 그리기.
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
			Drawing.PaintLine (p1, p2, brush.width, col, brush.hardness, testA); //그리는 부분.
			testA.Apply(); //textur2d저장.
	} else {
			Drawing.PaintLine (p1, p2, brush.width, col, brush.hardness, testB); //그리는 부분.
			testB.Apply (); //textur2d저장.
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
			Drawing.PaintLine (p1, p2, eraser.width, Color.white, eraser.hardness, testA);
			testA.Apply ();
		} else{
			Drawing.PaintLine (p1, p2, eraser.width, Color.white, eraser.hardness, testB);
			testB.Apply ();
		}
}
}

[System.Serializable]
public class LineTool
{
	public float width = 1; //두께
}

[System.Serializable]
public class EraserTool
{
	public float width = 1;
	public float hardness = 10; //단단하기?? 진하게인듯?
}

[System.Serializable]
public class BrushTool
{
	public float width = 1;
	public float hardness = 10;
	public float spacing = 10;
}

[System.Serializable]
public class Stroke
{
	public bool enabled = false; //외곽선의 유무
	public float width = 1;
}

//하던중인 이미지 종료시 저장여부 묻고 승낙시 저장.
//외부 이미지 불러와서 넣을 수 있도록 하기(1, 2레이어)
//이미지를 불러오게되면 사진의 설정에 차이가 있을테니 수정하도록 스크립트짜기
//이미지의 원본을 회손하는건 문제가 있으니 정보를 받아오거나 복사하게 할것.
//스샷기능을 통해 최종작업물을 이미지로 저장할수 있도록 하기

//확대, 축소, 이동.

//광고 상단배치(그림 그리는 중에 상단바를 여는 문제가 있으므로)
//유니티 에드를 찾아보고 스크립트는 구해높음. 추후 검색을 통해 스토어에 올리고 광고등록할것.

//레이어3번기능 중 버그
//브러시의 경우 알파값에 영향은 없지만 색에 따라 외곽선이 나타나는 문제.
//라인의 경우 해당 부분의 알파값을 낮추지만 라인의 색상남는현상이 발생.


//컬러 스포이드 기능.
//종료버튼 만들것.