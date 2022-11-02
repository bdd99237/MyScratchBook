using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum Samples
{
    None,
    Samples2,
    Samples4,
    Samples8,
    Samples16,
    Samples32,
    RotatedDisc
}

public class Drawing
{
    static public Samples numSamples = Samples.Samples4;

    static Vector2[] sample_Save;

    static public Texture2D DrawLine(Vector2 from, Vector2 to, float w, Color col, Texture2D tex)
    {
        return DrawLine(from, to, w, col, tex, false, Color.black, 0); //외곽선 false, 컬러는 검정, 외곽선의 굵기는 0을 추가하여 다음 함수 호출
    }

    static public Texture2D DrawLine(Vector2 from, Vector2 to, float w, Color col, Texture2D tex, bool stroke, Color strokeCol, float strokeWidth)
    {
        w = Mathf.Round(w); //선의 굵기 반올림
        strokeWidth = Mathf.Round(strokeWidth); //외곽선 굵기 반올림

        float extent = w + strokeWidth; // 그려야할 선의 최종 굵기.
        float stY = Mathf.Clamp(Mathf.Min(from.y, to.y) - extent, 0, tex.height);//가장 작은 y값에서 굵기를 뺀 값이 0~배경높이 사이일것.
        float stX = Mathf.Clamp(Mathf.Min(from.x, to.x) - extent, 0, tex.width);
        float endY = Mathf.Clamp(Mathf.Max(from.y, to.y) + extent, 0, tex.height); //가장 큰 y값에서 굵기를 더한 값이 0~배경높이 사이일것.
        float endX = Mathf.Clamp(Mathf.Max(from.x, to.x) + extent, 0, tex.width);

        strokeWidth = strokeWidth / 2; //인라인에 한번, 아웃라인에 한번 들어가서 2로 나눈다.
        float strokeInner = (w - strokeWidth) * (w - strokeWidth);
        float strokeOuter = (w + strokeWidth) * (w + strokeWidth);
        float strokeOuter2 = (w + strokeWidth + 1) * (w + strokeWidth + 1);
        float sqrW = w * w;//It is much faster to calculate with squared values

        float lengthX = endX - stX; //두 x간 거리
        float lengthY = endY - stY;
        Vector2 start = new Vector2(stX, stY);
        Color[] pixels = tex.GetPixels((int)stX, (int)stY, (int)lengthX, (int)lengthY, 0);//Get all pixels

        for (int y = 0; y < lengthY; y++)
        {
            for (int x = 0; x < lengthX; x++)
            {//Loop through the pixels
                Vector2 p = new Vector2(x, y) + start;
                Vector2 center = p + new Vector2(0.5f, 0.5f);
                float dist = (center - Mathfx.NearestPointStrict(from, to, center)).sqrMagnitude;//The squared distance from the center of the pixels to the nearest point on the line
                if (dist <= strokeOuter2)
                {
                    Vector2[] samples = Sample(p);
                    Color c = Color.black;
                    Color pc = pixels[(int)(y * lengthX + x)];
                    for (int i = 0; i < samples.Length; i++)
                    {//Loop through the samples
                        dist = (samples[i] - Mathfx.NearestPointStrict(from, to, samples[i])).sqrMagnitude;//The squared distance from the sample to the line
                        if (stroke)
                        {
                            if (dist <= strokeOuter && dist >= strokeInner) //거리차가 외곽선의 인라인과 아웃라인 사이일떄.
                            {
                                c += strokeCol;
                            }
                            else if (dist < sqrW) //거리차가 선의 보다 작을때
                            {
                                c += col;
                            }
                            else //나머지
                            {
                                c += pc;
                            }
                        }
                        else
                        {
                            if (dist < sqrW)
                            {//Is the distance smaller than the width of the line
                                c += col;
                            }
                            else
                            {
                                c += pc;//No it wasn't, set it to be the original colour
                            }
                        }
                    }
                    c /= samples.Length;//Get the avarage colour
                    pixels[(int)(y * lengthX + x)] = c;
                }
            }
        }
        tex.SetPixels((int)stX, (int)stY, (int)lengthX, (int)lengthY, pixels, 0);
        tex.Apply();
        return tex;
    }

    static Texture2D Paint(Vector2 pos, float rad, Color col, float hardness, Texture2D tex)
    {
        Vector2 start = new Vector2(Mathf.Clamp(pos.x - rad, 0, tex.width), Mathf.Clamp(pos.y - rad, 0, tex.height));
        float width = rad * 2;
        Vector2 end = new Vector2(Mathf.Clamp(pos.x + rad, 0, tex.width), Mathf.Clamp(pos.y + rad, 0, tex.height));
        float widthX = Mathf.Round(end.x - start.x);
        float widthY = Mathf.Round(end.y - start.y);
        float sqrRad = rad * rad;
        float sqrRad2 = (rad + 1) * (rad + 1);
        Color[] pixels = tex.GetPixels((int)start.x, (int)start.y, (int)widthX, (int)widthY, 0);
        Color c;

        for (int y = 0; y < widthY; y++)
        {
            for (int x = 0; x < widthX; x++)
            {
                Vector2 p = new Vector2(x, y) + start;
                Vector2 center = p + new Vector2(0.5f, 0.5f);
                float dist = (center - pos).sqrMagnitude;
                if (dist > sqrRad2)
                {
                    continue;
                }
                Vector2[] samples = Sample(p);
                c = Color.black;
                for (int i = 0; i < samples.Length; i++)
                {
                    dist = Mathfx.GaussFalloff(Vector2.Distance(samples[i], pos), rad) * hardness;
                    if (dist > 0)
                    {
                        c += Color.Lerp(pixels[(int)(y * widthX + x)], col, dist);
                    }
                    else
                    {
                        c += pixels[(int)(y * widthX + x)];
                    }
                }
                c /= samples.Length;

                pixels[(int)(y * widthX + x)] = c;
            }
        }

        tex.SetPixels((int)start.x, (int)start.y, (int)widthX, (int)widthY, pixels, 0);
        return tex;
    }

    static public void DrawBezier(List<BezierPoint> points, float rad, Color col, Texture2D tex)
    {
        rad = Mathf.Round(rad);//It is important to round the numbers otherwise it will mess up with the texture width
        //선의 두께를 반올림.

        //베지어 포인트가 1개이하인 경우
        if (points.Count <= 1)
        {
            return;
        }

        //값이 얼마이든 들어갈수 있도록 하기위해 초기화.
        Vector2 topleft = new Vector2(Mathf.Infinity, Mathf.Infinity); //무한한 양의 값의 대표.
        Vector2 bottomright = new Vector2(0, 0); //제로 위치

        for (int i = 0; i < points.Count - 1; i++)
        {
            BezierCurve curve = new BezierCurve(points[i].main, points[i].control_2, points[i + 1].control_1, points[i + 1].main); //3차원 베지어곡선 사용하여 선을 만듬.
            points[i].curve_2 = curve; //계산된 베지어곡선을 현재의 커브 2번에 저장.
            points[i + 1].curve_1 = curve; //계산된 베지어곡선을 다음의 커브1번에 저장.

            //가장 큰 사각인 경우를 찾는다.
            topleft.x = Mathf.Min(topleft.x, curve.rect.x);

            topleft.y = Mathf.Min(topleft.y, curve.rect.y);

            bottomright.x = Mathf.Max(bottomright.x, curve.rect.x + curve.rect.width);

            bottomright.y = Mathf.Max(bottomright.y, curve.rect.y + curve.rect.height);
        }

        topleft -= new Vector2(rad, rad);
        bottomright += new Vector2(rad, rad);

        Vector2 start = new Vector2(Mathf.Clamp(topleft.x, 0, tex.width), Mathf.Clamp(topleft.y, 0, tex.height));
        Vector2 width = new Vector2(Mathf.Clamp(bottomright.x - topleft.x, 0, tex.width - start.x), Mathf.Clamp(bottomright.y - topleft.y, 0, tex.height - start.y));

        Color[] pixels = tex.GetPixels((int)start.x, (int)start.y, (int)width.x, (int)width.y, 0);

        for (int y = 0; y < width.y; y++)
        {
            for (int x = 0; x < width.x; x++)
            {
                Vector2 p = new Vector2(x + start.x, y + start.y);
                if (!Mathfx.IsNearBeziers(p, points, rad + 2))
                {
                    continue;
                }

                Vector2[] samples = Sample(p);
                Color c = Color.black;
                Color pc = pixels[(int)(y * width.x + x)];//Previous pixel color
                for (int i = 0; i < samples.Length; i++)
                {
                    if (Mathfx.IsNearBeziers(samples[i], points, rad))
                    {
                        c += col;
                    }
                    else
                    {
                        c += pc;
                    }
                }

                c /= samples.Length;

                pixels[(int)(y * width.x + x)] = c;

            }
        }

        tex.SetPixels((int)start.x, (int)start.y, (int)width.x, (int)width.y, pixels, 0);
        tex.Apply();
    }

    static Vector2[] Sample(Vector2 p)
    {
        
        switch (numSamples)
        {
            case Samples.None:
                sample_Save = new Vector2[]{ p + new Vector2(0.5f, 0.5f)};
                break;

            case Samples.Samples2:
                sample_Save = new Vector2[] { p + new Vector2(0.25f, 0.5f), p + new Vector2(0.75f, 0.5f) };
                break;

            case Samples.Samples4:
                sample_Save = new Vector2[] {
                p + new Vector2(0.25f, 0.5f),
                p + new Vector2(0.75f, 0.5f),
                p + new Vector2(0.5f, 0.25f),
                p + new Vector2(0.5f, 0.75f)
                };
                break;

            case Samples.Samples8:
                sample_Save = new Vector2[] {
                p + new Vector2(0.25f, 0.5f),
                p + new Vector2(0.75f, 0.5f),
                p + new Vector2(0.5f, 0.25f),
                p + new Vector2(0.5f, 0.75f),

                p + new Vector2(0.25f, 0.25f),
                p + new Vector2(0.75f, 0.25f),
                p + new Vector2(0.25f, 0.75f),
                p + new Vector2(0.75f, 0.75f)
                };
                break;

            case Samples.Samples16:
                sample_Save = new Vector2[] {
                p + new Vector2(0, 0),
                p + new Vector2(0.3f, 0),
                p + new Vector2(0.7f, 0),
                p + new Vector2(1, 0),

                p + new Vector2(0, 0.3f),
                p + new Vector2(0.3f, 0.3f),
                p + new Vector2(0.7f, 0.3f),
                p + new Vector2(1, 0.3f),

                p + new Vector2(0, 0.7f),
                p + new Vector2(0.3f, 0.7f),
                p + new Vector2(0.7f, 0.7f),
                p + new Vector2(1, 0.7f),

                p + new Vector2(0, 1),
                p + new Vector2(0.3f, 1),
                p + new Vector2(0.7f, 1),
                p + new Vector2(1, 1)
                };
                break;

            case Samples.Samples32:
                sample_Save = new Vector2[] {

                p + new Vector2(0, 0),
                p + new Vector2(1, 0),
                p + new Vector2(0, 1),
                p + new Vector2(1, 1),

                p + new Vector2(0.2f, 0.2f),
                p + new Vector2(0.4f, 0.2f),
                p + new Vector2(0.6f, 0.2f),
                p + new Vector2(0.8f, 0.2f),

                p + new Vector2(0.2f, 0.4f),
                p + new Vector2(0.4f, 0.4f),
                p + new Vector2(0.6f, 0.4f),
                p + new Vector2(0.8f, 0.4f),

                p + new Vector2(0.2f, 0.6f),
                p + new Vector2(0.4f, 0.6f),
                p + new Vector2(0.6f, 0.6f),
                p + new Vector2(0.8f, 0.6f),

                p + new Vector2(0.2f, 0.8f),
                p + new Vector2(0.4f, 0.8f),
                p + new Vector2(0.6f, 0.8f),
                p + new Vector2(0.8f, 0.8f),

                p + new Vector2(0.5f, 0),
                p + new Vector2(0.5f, 1),
                p + new Vector2(0, 0.5f),
                p + new Vector2(1, 0.5f),

                p + new Vector2(0.5f, 0.5f)

                };
                break;

            case Samples.RotatedDisc:
                sample_Save = new Vector2[] {
                p + new Vector2(0, 0),
                p + new Vector2(1, 0),
                p + new Vector2(0, 1),
                p + new Vector2(1, 1),

                p + new Vector2(0.5f, 0.5f) + new Vector2(0.258f, 0.965f),//Sin (75°) && Cos (75°)
                p + new Vector2(0.5f, 0.5f) + new Vector2(-0.965f, -0.258f),
                p + new Vector2(0.5f, 0.5f) + new Vector2(0.965f, 0.258f),
                p + new Vector2(0.5f, 0.5f) + new Vector2(0.258f, -0.965f)
                };

                break;
        }

        return sample_Save;
    }

    static public Texture2D PaintLine(Vector2 from, Vector2 to, float rad, Color col, float hardness, Texture2D tex)
    {
        float width = rad * 2; //본 굵기의 2배한다.

        float extent = rad; //본굵기
        float stY = Mathf.Clamp(Mathf.Min(from.y, to.y) - extent, 0, tex.height); //두 개의 좌표중 작은 값을 구하고, 선굵기만큼 빼준다.(범위제한)
        float stX = Mathf.Clamp(Mathf.Min(from.x, to.x) - extent, 0, tex.width);
        float endY = Mathf.Clamp(Mathf.Max(from.y, to.y) + extent, 0, tex.height); //두 개의 좌표중 큰 값을 구하고, 선굵기만큼 더해준다.(범위제한)
        float endX = Mathf.Clamp(Mathf.Max(from.x, to.x) + extent, 0, tex.width);

        //두 위치 사이의 거리를 구함.
        float lengthX = endX - stX;
        float lengthY = endY - stY;



        float sqrRad = rad * rad;
        float sqrRad2 = (rad + 1) * (rad + 1);
        Color[] pixels = tex.GetPixels((int)stX, (int)stY, (int)lengthX, (int)lengthY, 0); //픽셀의 색상을 구한다.
        Vector2 start = new Vector2(stX, stY); //그릴 범위의 시작점.

        Color c;

        //Debug.Log (widthX + "   "+ widthY + "   "+ widthX*widthY);
        for (int y = 0; y < lengthY; y++)
        {
            for (int x = 0; x < lengthX; x++)
            {
                Vector2 p = new Vector2(x, y) + start; //각 픽셀의 시작점.(LeftUp)
                Vector2 center = p + new Vector2(0.5f, 0.5f); //픽셀의 센터.
                float dist = (center - Mathfx.NearestPointStrict(from, to, center)).sqrMagnitude; //sqrMagnitude는 두 점간의 거리의 제곱에 루트를 한 값을 구해준다.
                if (dist > sqrRad2)
                {
                    continue;
                }
                dist = Mathfx.GaussFalloff(Mathf.Sqrt(dist), rad) * hardness;
                if (dist > 0)
                {
                    c = Color.Lerp(pixels[(int)(y * lengthX + x)], col, dist); //거리차에 따라 색상을 선형보간
                }
                else
                {
                    c = pixels[(int)(y * lengthX + x)]; //거리차가 없을시 본색상 그대로 사용.
                }

                pixels[(int)(y * lengthX + x)] = c; //해당 픽셀 위치에 색상 저장.
            }
        }
        tex.SetPixels((int)start.x, (int)start.y, (int)lengthX, (int)lengthY, pixels, 0); //texture2d의 픽셀에 적용.
        return tex;
    }
}

[System.Serializable]
public class BezierPoint
{
    public Vector2 main;
    public Vector2 control_1; //왼쪽
    public Vector2 control_2; //오른쪽

    public BezierCurve curve_1;
    public BezierCurve curve_2;

    public BezierPoint(Vector2 m, Vector2 l, Vector2 r)
    {
        main = m;
        control_1 = l;
        control_2 = r;
    }
}

public class BezierCurve
{
    List<Vector2> points = new List<Vector2>();
    public float aproxLength;
    public Rect rect;

    public Vector2 Get(float t)
    {
        int t2 = (int)Mathf.Round(t * (points.Count - 1));
        return points[t2];
    }

    void Init(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        Vector2 topleft = new Vector2(Mathf.Infinity, Mathf.Infinity); //양의 무한대 값이 들어가고 있다.
        Vector2 bottomright = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity); //음의 무한대 값이 들어가고 있다.

        //매개변수로 받은 값들중 가장작은값을 topleft의 지점으로저장.
        topleft.x = Mathf.Min(topleft.x, p0.x); //둘중 작은 값이 들어감
        topleft.x = Mathf.Min(topleft.x, p1.x);
        topleft.x = Mathf.Min(topleft.x, p2.x);
        topleft.x = Mathf.Min(topleft.x, p3.x);

        topleft.y = Mathf.Min(topleft.y, p0.y);
        topleft.y = Mathf.Min(topleft.y, p1.y);
        topleft.y = Mathf.Min(topleft.y, p2.y);
        topleft.y = Mathf.Min(topleft.y, p3.y);

        //매개변수로 받은 값들중 가장 큰값을 bottomright의 지점으로 저장
        bottomright.x = Mathf.Max(bottomright.x, p0.x);
        bottomright.x = Mathf.Max(bottomright.x, p1.x);
        bottomright.x = Mathf.Max(bottomright.x, p2.x);
        bottomright.x = Mathf.Max(bottomright.x, p3.x);

        bottomright.y = Mathf.Max(bottomright.y, p0.y);
        bottomright.y = Mathf.Max(bottomright.y, p1.y);
        bottomright.y = Mathf.Max(bottomright.y, p2.y);
        bottomright.y = Mathf.Max(bottomright.y, p3.y);

        rect = new Rect(topleft.x, topleft.y, bottomright.x - topleft.x, bottomright.y - topleft.y); //사각 영역생성


        List<Vector2> ps = new List<Vector2>();

        Vector2 point1 = Mathfx.CubicBezier(0f, p0, p1, p2, p3);
        Vector2 point2 = Mathfx.CubicBezier(0.05f, p0, p1, p2, p3);
        Vector2 point3 = Mathfx.CubicBezier(0.1f, p0, p1, p2, p3);
        Vector2 point4 = Mathfx.CubicBezier(0.15f, p0, p1, p2, p3);

        Vector2 point5 = Mathfx.CubicBezier(0.5f, p0, p1, p2, p3);
        Vector2 point6 = Mathfx.CubicBezier(0.55f, p0, p1, p2, p3);
        Vector2 point7 = Mathfx.CubicBezier(0.6f, p0, p1, p2, p3);

        aproxLength = Vector2.Distance(point1, point2) + Vector2.Distance(point2, point3) + Vector2.Distance(point3, point4) + Vector2.Distance(point5, point6) + Vector2.Distance(point6, point7);

        Debug.Log(Vector2.Distance(point1, point2) + "     " + Vector2.Distance(point3, point4) + "   " + Vector2.Distance(point6, point7));
        aproxLength *= 4;

        float a2 = 0.5f / aproxLength;//Double the amount of points since the aproximation is quite bad
        for (float i = 0; i < 1; i += a2)
        {
            ps.Add(Mathfx.CubicBezier(i, p0, p1, p2, p3));
        }

        points.AddRange(ps);
    }

    public BezierCurve(Vector2 main, Vector2 control1, Vector2 control2, Vector2 end)
    {
        Init(main, control1, control2, end);
    }
}

