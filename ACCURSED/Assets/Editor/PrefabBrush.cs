using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Scatters random prefabs from a list at random positions inside a brush shape.
/// Open via Tools > Prefab Brush, add prefabs, press Start Painting, then click in the Scene view.
/// </summary>
public class PrefabBrush : EditorWindow
{
    private enum BrushShape
    {
        Circle,
        Square,
        Diamond
    }

    private const string UndoName = "Prefab Brush Stamp";
    private static readonly Color BrushColor = new Color(0.3f, 0.9f, 1f, 1f);

    [SerializeField] private List<GameObject> prefabs = new List<GameObject>();
    [SerializeField] private BrushShape shape = BrushShape.Circle;
    [SerializeField] private float brushSize = 5f;
    [SerializeField] private int instanceCount = 10;
    [SerializeField] private float zPosition;
    [SerializeField] private Transform parent;

    private SerializedObject serializedWindow;
    private SerializedProperty prefabsProperty;
    private Vector2 scrollPosition;
    private bool painting;

    [MenuItem("Tools/Prefab Brush")]
    private static void Open()
    {
        GetWindow<PrefabBrush>("Prefab Brush");
    }

    private void OnEnable()
    {
        serializedWindow = new SerializedObject(this);
        prefabsProperty = serializedWindow.FindProperty(nameof(prefabs));
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        SetPainting(false);
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawDropArea();

        serializedWindow.Update();
        EditorGUILayout.PropertyField(prefabsProperty, new GUIContent("Prefabs"), true);
        serializedWindow.ApplyModifiedProperties();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Brush", EditorStyles.boldLabel);
        shape = (BrushShape)EditorGUILayout.EnumPopup("Shape", shape);
        brushSize = EditorGUILayout.Slider(
            new GUIContent("Size", "Width of the brush in world units."), brushSize, 0.1f, 100f);
        instanceCount = EditorGUILayout.IntSlider(
            new GUIContent("Instances", "How many prefabs are created per click."), instanceCount, 1, 200);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Placement", EditorStyles.boldLabel);
        zPosition = EditorGUILayout.FloatField(
            new GUIContent("Z Position", "The Z value instances are placed at."), zPosition);
        parent = (Transform)EditorGUILayout.ObjectField(
            new GUIContent("Parent", "Optional scene object to put instances under."), parent, typeof(Transform), true);
        if (parent != null && EditorUtility.IsPersistent(parent))
        {
            parent = null;
        }

        EditorGUILayout.Space();
        bool hasPrefabs = GetValidPrefabs().Count > 0;
        if (!hasPrefabs)
        {
            SetPainting(false);
            EditorGUILayout.HelpBox("Add at least one prefab asset to start painting.", MessageType.Info);
        }

        using (new EditorGUI.DisabledScope(!hasPrefabs))
        {
            string label = painting ? "Stop Painting (Esc)" : "Start Painting";
            SetPainting(GUILayout.Toggle(painting, label, "Button", GUILayout.Height(30f)));
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawDropArea()
    {
        Rect dropArea = GUILayoutUtility.GetRect(0f, 50f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag prefabs here", EditorStyles.helpBox);

        Event e = Event.current;
        if ((e.type != EventType.DragUpdated && e.type != EventType.DragPerform) || !dropArea.Contains(e.mousePosition))
        {
            return;
        }

        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        if (e.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();
            foreach (Object dragged in DragAndDrop.objectReferences)
            {
                if (dragged is GameObject prefab && PrefabUtility.IsPartOfPrefabAsset(prefab))
                {
                    prefabs.Add(prefab);
                }
            }
        }

        e.Use();
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!painting)
        {
            return;
        }

        Event e = Event.current;
        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        // Claim the Scene view's default control so clicks stamp instead of changing the selection.
        if (e.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(controlId);
        }

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            SetPainting(false);
            Repaint();
            e.Use();
            return;
        }

        if (!TryGetBrushCenter(e.mousePosition, out Vector3 center))
        {
            return;
        }

        switch (e.type)
        {
            case EventType.Repaint:
                DrawBrush(center);
                break;

            case EventType.MouseMove:
                sceneView.Repaint();
                break;

            // Alt is left alone so Scene view navigation keeps working while painting.
            case EventType.MouseDown when e.button == 0 && !e.alt:
                Stamp(center);
                e.Use();
                break;
        }
    }

    private void SetPainting(bool value)
    {
        if (painting == value)
        {
            return;
        }

        painting = value;
        Tools.hidden = value; // Hide the move/rotate gizmo so it can't intercept clicks.
        SceneView.RepaintAll();
    }

    private bool TryGetBrushCenter(Vector2 guiPosition, out Vector3 center)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
        Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, zPosition));

        if (plane.Raycast(ray, out float distance))
        {
            center = ray.GetPoint(distance);
            return true;
        }

        center = default;
        return false;
    }

    private void DrawBrush(Vector3 center)
    {
        float radius = brushSize * 0.5f;
        Handles.color = BrushColor;

        switch (shape)
        {
            case BrushShape.Circle:
                Handles.DrawWireDisc(center, Vector3.forward, radius, 2f);
                break;

            case BrushShape.Square:
                DrawOutline(
                    center + new Vector3(-radius, -radius),
                    center + new Vector3(-radius, radius),
                    center + new Vector3(radius, radius),
                    center + new Vector3(radius, -radius));
                break;

            case BrushShape.Diamond:
                DrawOutline(
                    center + new Vector3(-radius, 0f),
                    center + new Vector3(0f, radius),
                    center + new Vector3(radius, 0f),
                    center + new Vector3(0f, -radius));
                break;
        }
    }

    private static void DrawOutline(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Handles.DrawAAPolyLine(2f, a, b, c, d, a);
    }

    private void Stamp(Vector3 center)
    {
        List<GameObject> validPrefabs = GetValidPrefabs();
        if (validPrefabs.Count == 0)
        {
            return;
        }

        // In Prefab Mode the active scene isn't visible, so instances go under the prefab being edited.
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        Transform instanceParent = prefabStage != null ? prefabStage.prefabContentsRoot.transform : parent;

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName(UndoName);
        int undoGroup = Undo.GetCurrentGroup();

        for (int i = 0; i < instanceCount; i++)
        {
            GameObject prefab = validPrefabs[Random.Range(0, validPrefabs.Count)];
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, instanceParent);
            instance.transform.position = center + (Vector3)RandomOffset();
            Undo.RegisterCreatedObjectUndo(instance, UndoName);
        }

        Undo.CollapseUndoOperations(undoGroup);
    }

    private Vector2 RandomOffset()
    {
        float radius = brushSize * 0.5f;

        switch (shape)
        {
            case BrushShape.Square:
                return new Vector2(Random.Range(-radius, radius), Random.Range(-radius, radius));

            case BrushShape.Diamond:
                // A square rotated 45 degrees keeps the distribution uniform.
                float u = Random.Range(-radius, radius);
                float v = Random.Range(-radius, radius);
                return new Vector2((u + v) * 0.5f, (u - v) * 0.5f);

            default:
                return Random.insideUnitCircle * radius;
        }
    }

    private List<GameObject> GetValidPrefabs()
    {
        // The list field also accepts scene objects and empty slots, so filter down to real prefab assets.
        return prefabs.FindAll(prefab => prefab != null && PrefabUtility.IsPartOfPrefabAsset(prefab));
    }
}
