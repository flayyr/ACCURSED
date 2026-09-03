using System.IO;
using UnityEditor;
using UnityEngine;

public static class FixAnimationClipNames
{
    [MenuItem("Tools/Fix Animation Clip Names")]
    static void FixAll()
    {
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip");
        int fixed_count = 0;
        int skipped = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // Skip clips embedded inside other assets (FBX, etc.)
            if (!path.EndsWith(".anim", System.StringComparison.OrdinalIgnoreCase))
                continue;

            string expectedName = Path.GetFileNameWithoutExtension(path);
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

            if (clip == null)
            {
                skipped++;
                continue;
            }

            if (clip.name == expectedName)
                continue;

            Debug.Log($"[FixAnimNames] '{clip.name}'  ->  '{expectedName}'  ({path})");

            var so = new SerializedObject(clip);
            so.FindProperty("m_Name").stringValue = expectedName;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(clip);
            fixed_count++;
        }

        if (fixed_count > 0)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        Debug.Log($"[FixAnimNames] Done — {fixed_count} clip(s) renamed, {skipped} skipped.");
        EditorUtility.DisplayDialog(
            "Fix Animation Clip Names",
            $"Done!\n\n{fixed_count} clip(s) renamed to match their filename.\n{skipped} skipped (null assets).",
            "OK");
    }
}
