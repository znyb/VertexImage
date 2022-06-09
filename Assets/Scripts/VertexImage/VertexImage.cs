using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(CanvasRenderer))]
public class VertexImage : MaskableGraphic
{
    [SerializeReference]
    public List<IVertexShape> VertexShapes = new List<IVertexShape>();
    
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        foreach(var vs in VertexShapes)
        {
            if(vs == null)
            {
                Debug.LogError("Vertex Shape is null");
                continue;
            }

            if (!vs.Enable)
                continue;

            vs.PopulateMesh(vh, rectTransform.rect);
        }
        Debug.Log(name +  " ��������" + vh.currentVertCount, gameObject);
        Debug.Log(name + " ������" + vh.currentIndexCount / 3, gameObject);
    }
}
