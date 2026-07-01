using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SpriteLibToAnimClip : EditorWindow
{
    private string _spriteLibPath;
    private UnityEngine.Object _pickerObj;
    private string[] _categoryNames = System.Array.Empty<string>();
    private int _selectedCategory;
    private AnimationClip _targetClip;
    private float _frameRate = 12f;
    private string _resolverPath = "Sprite"; // path to the object with SpriteResolver
    private bool _loop = true;

    [MenuItem("Tools/Sprite Library to Anim Clip")]
    static void Open() => GetWindow<SpriteLibToAnimClip>("Lib → Anim Clip");

    void OnGUI()
    {
        EditorGUILayout.LabelField("Sprite Library → Animation Clip (SpriteResolver)", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        // Asset picker
        EditorGUI.BeginChangeCheck();
        var picked = EditorGUILayout.ObjectField(
            "Sprite Library (.spriteLib)", _pickerObj, typeof(UnityEngine.Object), false);
        if (EditorGUI.EndChangeCheck())
        {
            string p = picked != null ? AssetDatabase.GetAssetPath(picked) : null;
            if (picked == null || (p != null && p.EndsWith(".spriteLib", System.StringComparison.OrdinalIgnoreCase)))
            {
                _pickerObj = picked;
                _spriteLibPath = p;
                RefreshCategories();
            }
            else
            {
                EditorUtility.DisplayDialog("Wrong asset type", "Please select a .spriteLib asset.", "OK");
            }
        }

        if (string.IsNullOrEmpty(_spriteLibPath))
        {
            EditorGUILayout.HelpBox("Drag a Sprite Library (.spriteLib) asset above.", MessageType.Info);
            return;
        }

        if (_categoryNames.Length == 0)
        {
            EditorGUILayout.HelpBox("No categories found in this library.", MessageType.Warning);
            if (GUILayout.Button("Retry")) RefreshCategories();
            return;
        }

        _selectedCategory = Mathf.Clamp(_selectedCategory, 0, _categoryNames.Length - 1);
        _selectedCategory = EditorGUILayout.Popup("Category", _selectedCategory, _categoryNames);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Clip Settings", EditorStyles.boldLabel);
        _frameRate    = EditorGUILayout.FloatField("Frame Rate (fps)", _frameRate);
        _loop         = EditorGUILayout.Toggle("Loop", _loop);
        _resolverPath = EditorGUILayout.TextField("SpriteResolver Path", _resolverPath);
        EditorGUILayout.HelpBox(
            "Relative path from the Animator root to the object with the SpriteResolver component. " +
            "\"Sprite\" matches existing project clips.",
            MessageType.None);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
        _targetClip = (AnimationClip)EditorGUILayout.ObjectField(
            "Target Clip (optional)", _targetClip, typeof(AnimationClip), false);
        EditorGUILayout.HelpBox(
            _targetClip == null
                ? "Leave empty to create a new .anim next to the sprite library, named after the category."
                : "Existing clip will be fully overwritten.",
            MessageType.None);

        EditorGUILayout.Space(8);
        if (GUILayout.Button("Generate Clip", GUILayout.Height(30)))
            GenerateClip();
    }

    void RefreshCategories()
    {
        _categoryNames = System.Array.Empty<string>();
        _selectedCategory = 0;
        if (!TryLoadLibrary(out _, out var lib)) return;

        _categoryNames = new string[lib.arraySize];
        for (int i = 0; i < lib.arraySize; i++)
            _categoryNames[i] = lib.GetArrayElementAtIndex(i)
                .FindPropertyRelative("m_Name").stringValue;
    }

    bool TryLoadLibrary(out UnityEngine.Object[] objects, out SerializedProperty lib)
    {
        objects = null; lib = null;
        if (string.IsNullOrEmpty(_spriteLibPath)) return false;

        objects = InternalEditorUtility.LoadSerializedFileAndForget(_spriteLibPath);
        if (objects == null || objects.Length == 0 || objects[0] == null) return false;

        var so = new SerializedObject(objects[0]);
        lib = so.FindProperty("m_Library");
        return lib != null && lib.isArray;
    }

    void GenerateClip()
    {
        if (!TryLoadLibrary(out _, out var lib)) return;

        string catName = _categoryNames[_selectedCategory];
        var labels = GetOrderedLabels(lib, catName);

        if (labels.Count == 0)
        {
            EditorUtility.DisplayDialog("No Labels",
                $"Category '{catName}' has no entries. Use Sprite Library Filler to populate it first.", "OK");
            return;
        }

        // Resolve or create the clip
        AnimationClip clip = _targetClip;
        if (clip == null)
        {
            string dir = Path.GetDirectoryName(_spriteLibPath)?.Replace('\\', '/') ?? "Assets";
            string clipPath = $"{dir}/{catName}.anim";
            clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath) ?? CreateNewClip(clipPath);
            if (clip == null) return;
        }

        clip.frameRate = 60f; // sample rate — matches existing project clips
        SetLooping(clip, _loop);

        // m_SpriteHash: single float curve combining category + label hash.
        // Hash = Animator.StringToHash("Category_Label") & 0x3FFFFFFF, stored as bit-cast float.
        var binding = EditorCurveBinding.FloatCurve(_resolverPath, typeof(SpriteResolver), "m_SpriteHash");
        AnimationUtility.SetEditorCurve(clip, binding, null); // clear first

        var curve = new AnimationCurve();
        float frameDuration = 1f / Mathf.Max(_frameRate, 0.01f);

        for (int i = 0; i < labels.Count; i++)
        {
            int hash = Animator.StringToHash($"{catName}_{labels[i]}") & 0x3FFFFFFF;
            float hashAsFloat = System.BitConverter.Int32BitsToSingle(hash);
            int idx = curve.AddKey(new Keyframe(i * frameDuration, hashAsFloat));
            AnimationUtility.SetKeyLeftTangentMode(curve, idx, AnimationUtility.TangentMode.Constant);
            AnimationUtility.SetKeyRightTangentMode(curve, idx, AnimationUtility.TangentMode.Constant);
        }

        AnimationUtility.SetEditorCurve(clip, binding, curve);

        EditorUtility.SetDirty(clip);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string savedPath = AssetDatabase.GetAssetPath(clip);
        Debug.Log($"[LibToAnim] '{savedPath}' — {labels.Count} frame(s) @ {_frameRate} fps  (category: {catName})");
        EditorGUIUtility.PingObject(clip);
        _targetClip = clip;
    }

    // Returns label names sorted by trailing number (e.g. "0","1","2",...,"10" not "0","1","10","2")
    List<string> GetOrderedLabels(SerializedProperty lib, string categoryName)
    {
        for (int i = 0; i < lib.arraySize; i++)
        {
            var cat = lib.GetArrayElementAtIndex(i);
            if (cat.FindPropertyRelative("m_Name").stringValue != categoryName) continue;

            var entries = cat.FindPropertyRelative("m_OverrideEntries");
            var labels = new List<string>(entries.arraySize);
            for (int j = 0; j < entries.arraySize; j++)
                labels.Add(entries.GetArrayElementAtIndex(j).FindPropertyRelative("m_Name").stringValue);

            labels.Sort((a, b) =>
            {
                int diff = TrailingNumber(a).CompareTo(TrailingNumber(b));
                return diff != 0 ? diff : System.StringComparer.OrdinalIgnoreCase.Compare(a, b);
            });

            return labels;
        }
        return new List<string>();
    }

    static int TrailingNumber(string name)
    {
        if (string.IsNullOrEmpty(name)) return int.MaxValue;
        var m = System.Text.RegularExpressions.Regex.Match(name, @"(\d+)\D*$");
        return m.Success ? int.Parse(m.Groups[1].Value) : int.MaxValue;
    }

    static AnimationClip CreateNewClip(string assetPath)
    {
        AssetDatabase.CreateAsset(new AnimationClip(), assetPath);
        return AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
    }

    static void SetLooping(AnimationClip clip, bool loop)
    {
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
    }
}
