using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct VertexCircleInfo
{
    public float Radius;
    public Gradient Color;
}

[Serializable]
public class VertexCircle : VertexShapeBase
{
    public Vector2 Center;
    public float StartAngle;
    public float EndAngle;
    [Range(1,360)]
    public int Step;

    public List<VertexCircleInfo> CircleInfos = new List<VertexCircleInfo>();

    public override void PopulateMesh(VertexHelper vh, Rect rect)
    {
        if (Step <= 0)
            return;

        if (CircleInfos.Count <= 1)
            return;

        int index = vh.currentVertCount;

        for(int i = 0; i <= Step; i++)
        {
            float t = i / (float)Step;
            float angle = Mathf.Deg2Rad * Mathf.Lerp(StartAngle, EndAngle, t);
            var dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            for (int j = 0; j < CircleInfos.Count; j++)
            {
                VertexCircleInfo info = CircleInfos[j];
                Vector2 pos = Center + dir * info.Radius;
                Color color = info.Color.Evaluate(t);
                vh.AddVert(pos, color, Vector4.zero);
                if(i > 0)
                {
                    int currentIndex = index + i * CircleInfos.Count + j;
                    if(j > 0)
                    {
                        vh.AddTriangle(currentIndex - 1, currentIndex, currentIndex - CircleInfos.Count);
                    }
                    if(j < CircleInfos.Count - 1)
                    {
                        vh.AddTriangle(currentIndex - CircleInfos.Count + 1, currentIndex - CircleInfos.Count, currentIndex);
                    }
                }
            }
        }
    }
}

