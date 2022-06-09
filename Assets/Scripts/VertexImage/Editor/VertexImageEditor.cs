using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEditorInternal;
using UnityEngine.UI;

[CustomEditor(typeof(VertexImage))]
public class VertexImageEditor : GraphicEditor
{
    ReorderableList reorderableList;
    SerializedProperty VertexShapes;

    protected override void OnEnable()
    {
        base.OnEnable();
        var image = target as VertexImage;
        VertexShapes = serializedObject.FindProperty("VertexShapes");
        reorderableList = new ReorderableList(image.VertexShapes, typeof(IVertexShape));
        reorderableList.onAddDropdownCallback += OnAddDropdown;
        reorderableList.onRemoveCallback += OnRemove;
        reorderableList.drawHeaderCallback += DrawHeader;
        reorderableList.drawElementCallback += DrawElement;
        reorderableList.elementHeightCallback += GetElementHeight;
        reorderableList.onChangedCallback += OnChange;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_Script);
        AppearanceControlsGUI();
        RaycastControlsGUI();
        reorderableList.DoLayoutList();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Enable All"))
        {
            foreach (var shape in (target as VertexImage).VertexShapes)
                shape.Enable = true;
            EditorUtility.SetDirty(target);
        }
        if (GUILayout.Button("Disable All"))
        {
            foreach (var shape in (target as VertexImage).VertexShapes)
                shape.Enable = false;
            EditorUtility.SetDirty(target);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Shape To Mesh"))
        {
            CoverToMesh();
        }
        if(GUILayout.Button("Clip Mesh"))
        {
            ClipMesh();
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    void OnAddDropdown(Rect rect,ReorderableList list)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Polygon"), false, OnAdd,"Polygon");
        menu.AddItem(new GUIContent("Bezier Line"), false, OnAdd,"BezierLine");
        menu.AddItem(new GUIContent("Circle"), false, OnAdd, "Circle");
        menu.AddItem(new GUIContent("Raw Mesh"), false, OnAdd, "Mesh");
        menu.DropDown(rect);
    }

    void OnChange(ReorderableList list)
    {
        (target as VertexImage).SetAllDirty();
        EditorUtility.SetDirty(target);
    }

    void OnRemove(ReorderableList list)
    {
        Undo.RecordObject(target, "Remove Vertext Shape");
        ReorderableList.defaultBehaviours.DoRemoveButton(reorderableList);
        EditorUtility.SetDirty(target);
    }

    void DrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "VertexShapes");
    }

    float GetElementHeight(int index)
    {
        if (index >= VertexShapes.arraySize)
            return 0;
        return EditorGUI.GetPropertyHeight(VertexShapes.GetArrayElementAtIndex(index), true);
    }

    void DrawElement(Rect rect,int index,bool isActive,bool focus)
    {
        var element = VertexShapes.GetArrayElementAtIndex(index);
        if (element == null)
            return;

        string typeName = element.managedReferenceFullTypename;
        int i = typeName.IndexOf("Vertex");
        if(i > 0 && i + 6 < typeName.Length)
        {
            typeName = typeName.Substring(i + 6);
        }
        if(string.IsNullOrEmpty(typeName))
        {
            typeName = "null";
        }
        EditorGUI.indentLevel++;
        EditorGUI.PropertyField(rect, element, new GUIContent(typeName), true);
        EditorGUI.indentLevel--;
    }

    void OnAdd(object type)
    {
        Undo.RecordObject(target, "Add Vertex Shape " + type);
        string typeName = type as string;
        var image = target as VertexImage;
        if(typeName == "Polygon")
        {
            image.VertexShapes.Add(new VertexPolygon());
        }
        else if(typeName == "BezierLine")
        {
            image.VertexShapes.Add(new VertexBezierLine());
        }
        else if(typeName == "Circle")
        {
            image.VertexShapes.Add(new VertexCircle());
        }
        else if(typeName == "Mesh")
        {
            image.VertexShapes.Add(new VertexMesh());
        }
        EditorUtility.SetDirty(target);
    }


    void CoverToMesh()
    {
        Undo.RecordObject(target, "Vertex Shape To Mesh");
        var image = target as VertexImage;
        Rect rect = image.rectTransform.rect;
        List<IVertexShape> meshs = new List<IVertexShape>();
        using (VertexHelper vh = new VertexHelper())
        {
            
            Mesh mesh = new Mesh();
            foreach (var shape in image.VertexShapes)
            {
                if (shape is VertexMesh)
                {
                    meshs.Add(shape);
                }
                else
                {
                    var vertexMesh = new VertexMesh();
                    vertexMesh.Enable = shape.Enable;

                    vh.Clear();
                    shape.PopulateMesh(vh, rect);
                    mesh.Clear();
                    vh.FillMesh(mesh);

                    Vector3[] positions = mesh.vertices;
                    Color[] colors = mesh.colors;
                    for (int i = 0; i < mesh.vertexCount; i++)
                    {
                        Vector2 pos = positions[i];
                        pos.x = (pos.x - rect.xMin) / rect.width;
                        pos.y = (pos.y - rect.yMin) / rect.height;
                        vertexMesh.Vertexs.Add(new VertexInfo() { Position = pos, Color = colors[i] });
                    }

                    var triangles = mesh.triangles;
                    for (int i = 0; i < triangles.Length; i += 3)
                    {
                        vertexMesh.Triangles.Add(new Vector3Int(triangles[i], triangles[i + 1], triangles[i + 2]));
                    }
                    meshs.Add(vertexMesh);
                }
            }
            DestroyImmediate(mesh);
        }
        image.VertexShapes = meshs;

        EditorUtility.SetDirty(target);
    }

    void ClipMesh()
    {
        Undo.RecordObject(target, "Clip Vertex Mesh");
        var image = target as VertexImage;
        Rect rect = image.rectTransform.rect;

        foreach (var shape in image.VertexShapes)
        {
            if (shape is VertexMesh mesh)
            {
                mesh.Clip(rect);
            }
        }

        EditorUtility.SetDirty(target);
    }
}

