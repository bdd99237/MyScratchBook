using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToolberButton : MonoBehaviour {

    public bool circle_IsActive;

	public GameObject[] colorWindow_Obj;

	public GameObject[] layers;

    public GameObject layerChangeWindow;

    int beforTool; //이전에 사용하던 툴의 넘버 저장.(사이클의 창변경에 사용)

    void Start()
    {
        beforTool = ScratchPainter.Instance().tool2;
    }

    void Update()
    {
            CircleUpdate();
    }

	//Tool기능 부분.
	public void Tool_Line()
	{
        beforTool = ScratchPainter.Instance().tool2;

		ScratchPainter.Instance ().tool = Tool.Line;
        ScratchPainter.Instance().tool2 = (int)Tool.Line;
	}

	public void Tool_Brush()
	{
        beforTool = ScratchPainter.Instance().tool2;

		ScratchPainter.Instance ().tool = Tool.Brush;
        ScratchPainter.Instance().tool2 = (int)Tool.Brush;
	}

	public void Tool_Eraser()
	{
        beforTool = ScratchPainter.Instance().tool2;

		ScratchPainter.Instance ().tool = Tool.Eraser;
        ScratchPainter.Instance().tool2 = (int)Tool.Eraser;
	}

	//레이어 전환 창 오픈 버튼 부분.
	public void WindowOpen()
	{
        //색상창 모두 끈다.
        circle_IsActive = false;
        for (int i = 0; i < colorWindow_Obj.Length; i++) {
				colorWindow_Obj [i].SetActive (false); //켜져있는걸 끈다.
			}

        if (layerChangeWindow.active == true)
        {
            layerChangeWindow.SetActive(false);
            if (ScratchPainter.Instance().tool2 != 0 && ScratchPainter.Instance().tool2 != 4)
            {
                ScratchPainter.Instance().tool = (Tool)ScratchPainter.Instance().tool2;
            }
			
		}
		else
		{
            layerChangeWindow.SetActive(true);
            if (ScratchPainter.Instance().tool != (Tool) 0 && ScratchPainter.Instance().tool != (Tool) 4)
            {
                ScratchPainter.Instance().tool2 = (int)ScratchPainter.Instance().tool;
            }
			ScratchPainter.Instance ().tool = Tool.Layer;

            SelectLayerButton();
		}
	}

	//레이어 전환 버튼.
	public void SelectLayer(int layerNum)
	{
		ScratchPainter.Instance ().mode = (Mode)layerNum;
        SelectLayerButton();
	}

	//선택한 레이어의 색상을 바꿔서 표시
	void SelectLayerButton()
	{
		for (int i = 0; i < layers.Length; i++) {
			layers [i].GetComponent<Image> ().color = Color.white;
		}
		layers[(int)ScratchPainter.Instance ().mode].GetComponent<Image> ().color = Color.gray;
	}

	//컬러선택창 부분.
	public void ColorWindowOpen()
	{
        layerChangeWindow.SetActive(false); //레이어창 끔.

        /*
		for (int i = 0; i < colorWindow_Obj.Length; i++) {
			//창이 열려있을때.
			if (colorWindow_Obj [i].active) {
				colorWindow_Obj [i].SetActive (false); //켜져있는걸 끈다.
				ScratchPainter.Instance ().tool = (Tool)ScratchPainter.Instance ().tool2; //이전에 사용중이던 툴로 돌아간다.
				break;
			}
			//열수 있는 창을 다확인했을때 모두 닫혀있다면.
			if (colorWindow_Obj.Length - 1 == i) {
				//창여는 부분.
				switch(ScratchPainter.Instance ().tool)
				{
				case Tool.Line:
					colorWindow_Obj [0].SetActive (true);
					break;

				case Tool.Brush:
					colorWindow_Obj [1].SetActive (true);
					break;

				case Tool.Eraser:
					colorWindow_Obj [2].SetActive (true);
					break;

                    default :
                    ScratchPainter.Instance().tool = Tool.Line;
                    colorWindow_Obj[0].SetActive(true);
                    break;
				}
				//ScratchPainter.Instance ().tool2 = (int)ScratchPainter.Instance ().tool; //사용중이던 툴저장.
				ScratchPainter.Instance ().tool = Tool.Circle; //상태를 사이클로 전환.
			}
		}
         */

        if (circle_IsActive)
        {
            circle_IsActive = false;
            ScratchPainter.Instance().tool = (Tool)ScratchPainter.Instance().tool2; //이전에 사용중이던 툴로 돌아간다.
        }
        else
        {
            circle_IsActive = true;
            ScratchPainter.Instance().tool = Tool.Circle; //상태를 사이클로 전환.
        }
	}

    void CircleUpdate()
    {
        if (circle_IsActive)
        {
            if (beforTool != ScratchPainter.Instance().tool2)
            {
                colorWindow_Obj[beforTool - 1].SetActive(false);
            }
            colorWindow_Obj[ScratchPainter.Instance().tool2 - 1].SetActive(true);
        }
        else
        {
            //창모두 끔.
            for (int i = 0; i < colorWindow_Obj.Length; i++)
            {
                colorWindow_Obj[i].SetActive(false);
            }
        }
    }
}
