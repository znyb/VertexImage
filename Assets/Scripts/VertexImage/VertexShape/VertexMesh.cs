using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[Serializable]
public struct VertexInfo
{
    public Vector2 Position;
    public Color32 Color;
}

[Serializable]
public class VertexMesh : VertexShapeBase
{
    public List<VertexInfo> Vertexs = new List<VertexInfo>();
    public List<Vector3Int> Triangles = new List<Vector3Int>();

    public override void PopulateMesh(VertexHelper vh, Rect rect)
    {
        int index = vh.currentVertCount;
        float xMin = rect.xMin;
        float xMax = rect.xMax;
        float yMin = rect.yMin;
        float yMax = rect.yMax;
        for (int i = 0; i < Vertexs.Count; i++)
        {
            float x = Mathf.LerpUnclamped(xMin, xMax, Vertexs[i].Position.x);
            float y = Mathf.LerpUnclamped(yMin, yMax, Vertexs[i].Position.y);
            vh.AddVert(new Vector3(x, y), Vertexs[i].Color, Vector4.zero);
        }

        foreach(var triangle in Triangles)
        {
            vh.AddTriangle(index + triangle.x, index + triangle.y, index + triangle.z);
        }
    }


#if UNITY_EDITOR

    public void Clip(Rect rect)
    {
        List<int> polygons = new List<int>();

        bool AddCrossPoint(VertexInfo v1, VertexInfo v2, int axis, float sepatateValue)
        {
            if ((v1.Position[axis] > sepatateValue && v2.Position[axis] < sepatateValue) 
                || (v1.Position[axis] < sepatateValue && v2.Position[axis] > sepatateValue))
            {
                float t = (v1.Position[axis] - sepatateValue) / (v1.Position[axis] - v2.Position[axis]);
                polygons.Add(Vertexs.Count);
                var pos = new Vector2();
                pos[axis] = sepatateValue;
                int otherAxis = axis == 0 ? 1 : 0;
                pos[otherAxis] = Mathf.Lerp(v1.Position[otherAxis], v2.Position[otherAxis], t);
                // todo gama校正?
                var color = Color32.Lerp(v1.Color, v2.Color, t);
                Vertexs.Add(new VertexInfo() { Position = pos, Color = color });
                return true;
            }
            return false;
        }

        List<Vector3Int> newTriangles = new List<Vector3Int>();

        void AddTriangles()
        {
            for(int i = 1; i < polygons.Count - 1; i++)
            {
                newTriangles.Add(new Vector3Int(polygons[0], polygons[i], polygons[i + 1]));
            }
        }

        List<Vector3Int> tris = Triangles;
        // 左
        foreach(var tri in tris)
        {
            polygons.Clear();
            VertexInfo v1 = Vertexs[tri.x];
            VertexInfo v2 = Vertexs[tri.y];
            VertexInfo v3 = Vertexs[tri.z];
            if(v1.Position.x >= 0)
            {
                polygons.Add(tri.x);
            }

            AddCrossPoint(v1, v2, 0, 0);

            if (v2.Position.x >= 0)
            {
                polygons.Add(tri.y);
            }

            AddCrossPoint(v2, v3, 0, 0);

            if(v3.Position.x >= 0)
            {
                polygons.Add(tri.z);
            }

            AddCrossPoint(v3, v1, 0, 0);

            AddTriangles();
        }


        Triangles = newTriangles;
        newTriangles = tris;
        tris = Triangles;
        newTriangles.Clear();
        // 下
        foreach (var tri in tris)
        {
            polygons.Clear();
            VertexInfo v1 = Vertexs[tri.x];
            VertexInfo v2 = Vertexs[tri.y];
            VertexInfo v3 = Vertexs[tri.z];
            if (v1.Position.y >= 0)
            {
                polygons.Add(tri.x);
            }

            AddCrossPoint(v1, v2, 1, 0);

            if (v2.Position.y >= 0)
            {
                polygons.Add(tri.y);
            }

            AddCrossPoint(v2, v3, 1, 0);

            if (v3.Position.y >= 0)
            {
                polygons.Add(tri.z);
            }

            AddCrossPoint(v3, v1, 1, 0);

            AddTriangles();
        }


        Triangles = newTriangles;
        newTriangles = tris;
        tris = Triangles;
        newTriangles.Clear();
        // 右
        foreach (var tri in tris)
        {
            polygons.Clear();
            VertexInfo v1 = Vertexs[tri.x];
            VertexInfo v2 = Vertexs[tri.y];
            VertexInfo v3 = Vertexs[tri.z];
            if (v1.Position.x <= 1)
            {
                polygons.Add(tri.x);
            }

            AddCrossPoint(v1, v2, 0, 1);

            if (v2.Position.x <= 1)
            {
                polygons.Add(tri.y);
            }

            AddCrossPoint(v2, v3, 0, 1);

            if (v3.Position.x <= 1)
            {
                polygons.Add(tri.z);
            }

            AddCrossPoint(v3, v1, 0, 1);

            AddTriangles();
        }


        Triangles = newTriangles;
        newTriangles = tris;
        tris = Triangles;
        newTriangles.Clear();
        // 上
        foreach (var tri in tris)
        {
            polygons.Clear();
            VertexInfo v1 = Vertexs[tri.x];
            VertexInfo v2 = Vertexs[tri.y];
            VertexInfo v3 = Vertexs[tri.z];
            if (v1.Position.y <= 1)
            {
                polygons.Add(tri.x);
            }

            AddCrossPoint(v1, v2, 1, 1);

            if (v2.Position.y <= 1)
            {
                polygons.Add(tri.y);
            }

            AddCrossPoint(v2, v3, 1, 1);

            if (v3.Position.y <= 1)
            {
                polygons.Add(tri.z);
            }

            AddCrossPoint(v3, v1, 1, 1);

            AddTriangles();
        }


        Triangles = newTriangles;
    }

    public void Normalized(Rect rect)
    {

    }

    public void Standardized(Rect rect)
    {

    }

#endif
}

