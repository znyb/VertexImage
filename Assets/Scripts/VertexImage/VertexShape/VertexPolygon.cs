using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class VertexPolygonInfo
{
    public List<VertexInfo> VertexInfos = new List<VertexInfo>();
}

[Serializable]
public class VertexPolygon : VertexShapeBase
{
    public List<VertexPolygonInfo> Polygons = new List<VertexPolygonInfo>();

    public override void PopulateMesh(VertexHelper vh, Rect rect)
    {
        if (Polygons.Count <= 1)
            return;

        int index = vh.currentVertCount;

        int step = 0;
        foreach(var info in Polygons)
        {
            if (step < info.VertexInfos.Count)
                step = info.VertexInfos.Count;
        }

        if (step == 0)
            return;

        for (int i = 0; i < step; i++)
        {
            for (int j = 0; j < Polygons.Count; j++)
            {
                var info = Polygons[j];
                if(step >= info.VertexInfos.Count)
                {
                    vh.AddVert(Vector3.zero, Color.clear, Vector4.zero);
                }
                else
                {
                    vh.AddVert(info.VertexInfos[step].Position, info.VertexInfos[step].Color, Vector4.zero);
                }

                if (i > 0)
                {
                    int currentIndex = index + i * Polygons.Count + j;
                    if (j > 0)
                    {
                        vh.AddTriangle(currentIndex - 1, currentIndex, currentIndex - Polygons.Count);
                    }
                    if (j < Polygons.Count - 1)
                    {
                        vh.AddTriangle(currentIndex - Polygons.Count + 1, currentIndex - Polygons.Count, currentIndex);
                    }
                }
            }
        }
    }
}

