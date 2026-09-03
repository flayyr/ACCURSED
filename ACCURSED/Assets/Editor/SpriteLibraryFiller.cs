using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class SpriteLibraryFiller : EditorWindow
{
    private string _spriteLibPath;
    private UnityEngine.Object _pickerObj;
    private string[] _categoryNames = System.Array.Empty<string>();
    private int _selectedCategory;
    private string _lastCategoryName;
    private readonly List<Sprite> _sprites = new();
    private ReorderableList _reorderableList;
    private Vector2 _scroll;

    [MenuItem("Tools/Sprite Library Filler")]
    static void Open() => GetWindow<SpriteLibraryFiller>("Sprite Library Filler");

    void OnEnable() => RebuildList();

    void OnFocus() { if (!string.IsNullOrEmpty(_spriteLibPath)) RefreshCategories(); }

    void RebuildList()
    {
        _reorderableList = new ReorderableList(_sprites, typeof(Sprite), true, true, true, true);
        _reorderableList.drawHeaderCallback = r =>
            EditorGUI.LabelField(r, $"Sprites in order  ({_sprites.Count} frames)");
        _reorderableList.drawElementCallback = (r, i, active, focused) =>
        {
            _sprites[i] = (Sprite)EditorGUI.ObjectField(
                new Rect(r.x, r.y + 2, r.width, EditorGUIUtility.singleLineHeight),
                _sprites[i], typeof(Sprite), false);
        };
        _reorderableList.onAddCallback = _ => _sprites.Add(null);
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Sprite Library Filler", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        // Asset picker — accept any Object, validate by extension
        EditorGUI.BeginChangeCheck();
        var picked = EditorGUILayout.ObjectField(
            "Sprite Library (.spriteLib)", _pickerObj, typeof(UnityEngine.Object), false);
        if (EditorGUI.EndChangeCheck())
        {
            string path = picked != null ? AssetDatabase.GetAssetPath(picked) : null;
            bool valid = picked == null || (path != null && path.EndsWith(".spriteLib", System.StringComparison.OrdinalIgnoreCase));
            if (valid)
            {
                _pickerObj = picked;
                _spriteLibPath = path;
                RefreshCategories();
            }
            else
            {
                EditorUtility.DisplayDialog("Wrong asset type", "Please select a .spriteLib asset.", "OK");
            }
        }

        if (string.IsNullOrEmpty(_spriteLibPath))
        {
            EditorGUILayout.HelpBox("Drag a Sprite Library (.spriteLib) asset into the field above.", MessageType.Info);
            return;
        }

        if (_categoryNames.Length == 0)
        {
            EditorGUILayout.HelpBox("No categories found. Make sure the library has categories set up.", MessageType.Warning);
            if (GUILayout.Button("Retry"))
                RefreshCategories();
            return;
        }

        _selectedCategory = Mathf.Clamp(_selectedCategory, 0, _categoryNames.Length - 1);
        _selectedCategory = EditorGUILayout.Popup("Category to fill", _selectedCategory, _categoryNames);
        _lastCategoryName = _categoryNames[_selectedCategory];
        EditorGUILayout.Space(6);

        DrawDropZone();
        EditorGUILayout.Space(4);

        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        _reorderableList.DoLayoutList();
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(4);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear List"))
        {
            _sprites.Clear();
            Repaint();
        }

        int valid_count = _sprites.Count(s => s != null);
        GUI.enabled = valid_count > 0;
        if (GUILayout.Button($"Fill \"{_categoryNames[_selectedCategory]}\"  ({valid_count} sprites)", GUILayout.Height(28)))
            FillCategory();
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
    }

    void DrawDropZone()
    {
        Rect dropRect = GUILayoutUtility.GetRect(0, 44, GUILayout.ExpandWidth(true));
        GUI.Box(dropRect, "Drop sprites here to add them to the list (auto-sorted by name)");

        Event evt = Event.current;
        if (!dropRect.Contains(evt.mousePosition)) return;

        if (evt.type == EventType.DragUpdated)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            evt.Use();
        }
        else if (evt.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();
            var added = new List<Sprite>();

            foreach (var obj in DragAndDrop.objectReferences)
            {
                if (obj is Sprite s)
                {
                    added.Add(s);
                }
                else if (obj is Texture2D tex)
                {
                    string texPath = AssetDatabase.GetAssetPath(tex);
                    var subs = AssetDatabase.LoadAllAssetsAtPath(texPath)
                        .OfType<Sprite>()
                        .OrderBy(sp => TrailingNumber(sp.name))
                        .ThenBy(sp => sp.name, System.StringComparer.OrdinalIgnoreCase)
                        .ToList();
                    added.AddRange(subs.Count > 0 ? subs : new List<Sprite> { null });
                }
            }

            added.Sort((a, b) =>
            {
                int diff = TrailingNumber(a?.name).CompareTo(TrailingNumber(b?.name));
                return diff != 0 ? diff : System.StringComparer.OrdinalIgnoreCase.Compare(a?.name, b?.name);
            });

            _sprites.AddRange(added);
            RebuildList();
            Repaint();
            evt.Use();
        }
    }

    // Extracts the last contiguous digit sequence from a name for numeric sorting.
    // "Knight_Walk_10" → 10, so "1, 2, 10" sorts correctly instead of "1, 10, 2".
    static int TrailingNumber(string name)
    {
        if (string.IsNullOrEmpty(name)) return int.MaxValue;
        var m = System.Text.RegularExpressions.Regex.Match(name, @"(\d+)\D*$");
        return m.Success ? int.Parse(m.Groups[1].Value) : int.MaxValue;
    }

    // .spriteLib files are ScriptedImporter assets — SerializedObject on the imported
    // runtime asset won't expose m_Library. Load the source YAML file directly instead.
    bool TryLoadLibrary(out UnityEngine.Object[] objects, out SerializedObject so, out SerializedProperty lib)
    {
        objects = null; so = null; lib = null;
        if (string.IsNullOrEmpty(_spriteLibPath)) return false;

        objects = InternalEditorUtility.LoadSerializedFileAndForget(_spriteLibPath);
        if (objects == null || objects.Length == 0 || objects[0] == null)
        {
            Debug.LogError($"[SpriteLibFiller] Could not load '{_spriteLibPath}'");
            return false;
        }

        so = new SerializedObject(objects[0]);
        lib = so.FindProperty("m_Library");

        if (lib == null || !lib.isArray)
        {
            Debug.LogError("[SpriteLibFiller] m_Library property not found on loaded object.");
            return false;
        }
        return true;
    }

    void RefreshCategories()
    {
        _categoryNames = System.Array.Empty<string>();
        _selectedCategory = 0;

        if (string.IsNullOrEmpty(_spriteLibPath)) return;

        // Use the runtime asset so inherited categories are included.
        // Invoke GetCategoryNames() via reflection to stay package-version agnostic.
        var runtimeAsset = AssetDatabase.LoadMainAssetAtPath(_spriteLibPath);
        if (runtimeAsset != null)
        {
            var getNames = runtimeAsset.GetType().GetMethod(
                "GetCategoryNames",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (getNames != null)
            {
                var names = (getNames.Invoke(runtimeAsset, null) as System.Collections.IEnumerable)
                    ?.Cast<string>().ToArray();
                if (names != null && names.Length > 0)
                {
                    _categoryNames = names;
                    RestoreSelectedCategory();
                    return;
                }
            }
        }

        // Fallback: read YAML directly (local categories only)
        if (!TryLoadLibrary(out _, out _, out var lib)) return;
        _categoryNames = new string[lib.arraySize];
        for (int i = 0; i < lib.arraySize; i++)
            _categoryNames[i] = lib.GetArrayElementAtIndex(i)
                .FindPropertyRelative("m_Name").stringValue;

        RestoreSelectedCategory();
    }

    void RestoreSelectedCategory()
    {
        if (!string.IsNullOrEmpty(_lastCategoryName))
        {
            int idx = System.Array.IndexOf(_categoryNames, _lastCategoryName);
            _selectedCategory = idx >= 0 ? idx : 0;
        }
    }

    void FillCategory()
    {
        if (!TryLoadLibrary(out var objects, out var so, out var lib)) return;

        string targetCat = _categoryNames[_selectedCategory];
        SerializedProperty category = null;

        for (int i = 0; i < lib.arraySize; i++)
        {
            var cat = lib.GetArrayElementAtIndex(i);
            if (cat.FindPropertyRelative("m_Name").stringValue == targetCat)
            {
                category = cat;
                break;
            }
        }

        if (category == null)
        {
            // Inherited category — insert a local override entry so we can write to it
            int newIdx = lib.arraySize;
            lib.arraySize = newIdx + 1;
            category = lib.GetArrayElementAtIndex(newIdx);
            category.FindPropertyRelative("m_Name").stringValue = targetCat;
            var initEntries = category.FindPropertyRelative("m_OverrideEntries");
            if (initEntries != null) initEntries.arraySize = 0;
            var initCount = category.FindPropertyRelative("m_EntryOverrideCount");
            if (initCount != null) initCount.intValue = 0;
        }

        var validSprites = _sprites.Where(s => s != null).ToList();
        var entries = category.FindPropertyRelative("m_OverrideEntries");

        entries.arraySize = validSprites.Count;
        for (int i = 0; i < validSprites.Count; i++)
        {
            var entry = entries.GetArrayElementAtIndex(i);
            string label = i.ToString();
            entry.FindPropertyRelative("m_Name").stringValue = label;
            entry.FindPropertyRelative("m_Hash").intValue = Animator.StringToHash(label);
            entry.FindPropertyRelative("m_Sprite").objectReferenceValue = validSprites[i];
            entry.FindPropertyRelative("m_FromMain").intValue = 0;
            entry.FindPropertyRelative("m_SpriteOverride").objectReferenceValue = validSprites[i];
        }
        category.FindPropertyRelative("m_EntryOverrideCount").intValue = validSprites.Count;

        so.ApplyModifiedPropertiesWithoutUndo();
        InternalEditorUtility.SaveToSerializedFileAndForget(objects, _spriteLibPath, true);
        AssetDatabase.ImportAsset(_spriteLibPath, ImportAssetOptions.ForceUpdate);

        Debug.Log($"[SpriteLibFiller] Filled '{targetCat}' with {validSprites.Count} sprite(s).");
        RefreshCategories();
        _sprites.Clear();
        RebuildList();
        Repaint();
    }
}
