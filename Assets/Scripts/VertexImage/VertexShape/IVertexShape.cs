using System;
using UnityEngine;
using UnityEngine.UI;

public interface IVertexShape
{
    bool Enable { get; set; }
    void PopulateMesh(VertexHelper vh, Rect rect);
}

[Serializable]
public abstract class VertexShapeBase : IVertexShape
{
    [SerializeField]
    protected bool enable = true;
    public bool Enable { get => enable; set => enable = value; }

    public abstract void PopulateMesh(VertexHelper vh, Rect rect);
}

