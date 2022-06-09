using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class VertexBezierLineInfo
{
    public Gradient Color;
    public Vector2 StartPoint;
    public Vector2 EndPoint;
    public List<Vector2> ControlPoints;

    public Vector2 GetPoint(float t)
    {
        t = Mathf.Clamp01(t);
        return GetPoint(t, -1, ControlPoints.Count);
    }

    Vector2 GetPoint(float t, int index1, int index2)
    {
        if(index2 - index1 <= 1)
        {
            Vector2 p1 = Vector2.zero;
            Vector2 p2 = Vector2.zero;
            if(index1 < -1)
            {
                Debug.LogError("bezier index1 out of range " + index1);
            }
            else if(index1 == -1)
            {
                p1 = StartPoint;
            }
            else
            {
                p1 = ControlPoints[index1];
            }

            if(index2 > ControlPoints.Count)
            {
                Debug.LogError("bezier index2 out of range " + index1);
            }
            else if(index2 == ControlPoints.Count)
            {
                p2 = EndPoint;
            }
            else
            {
                p2 = ControlPoints[index2];
            }

            return (1 - t) * p1 + t * p2;
        }
        return (1 - t) * GetPoint(t, index1, index2 - 1) + t * GetPoint(t, index1 + 1, index2);
    }
}


[Serializable]
public class VertexBezierLine : VertexShapeBase
{
    public int Step;

    public List<VertexBezierLineInfo> LineInfos = new List<VertexBezierLineInfo>();

    public override void PopulateMesh(VertexHelper vh, Rect rect)
    {
        Debug.Log(rect);
        if (Step <= 0)
            return;

        if (LineInfos.Count <= 1)
            return;

        int index = vh.currentVertCount;

        for (int i = 0; i <= Step; i++)
        {
            float t = i / (float)Step;
            for (int j = 0; j < LineInfos.Count; j++)
            {
                var info = LineInfos[j];
                Vector2 pos = info.GetPoint(t);
                Color color = info.Color.Evaluate(t);
                vh.AddVert(pos, color, Vector4.zero);
                if (i > 0)
                {
                    int currentIndex = index + i * LineInfos.Count + j;
                    if (j > 0)
                    {
                        vh.AddTriangle(currentIndex - 1, currentIndex, currentIndex - LineInfos.Count);
                    }
                    if (j < LineInfos.Count - 1)
                    {
                        vh.AddTriangle(currentIndex - LineInfos.Count + 1, currentIndex - LineInfos.Count, currentIndex);
                    }
                }
            }
        }
    }
}

