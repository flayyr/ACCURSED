using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class NavigationHelper : EditorWindow
{
    // Only colliders on these layers get a Navigation Modifier added automatically.
    private LayerMask colliderLayerMask = ~0;

    [MenuItem("Window/Navigation Helper")]
    static void Init()
    {
        GetWindow<NavigationHelper>("Navigation Helper");
    }

    void OnGUI()
    {
        GUILayout.Label("1. Mark Colliders As Obstacles", EditorStyles.boldLabel);
        GUILayout.Label("Adds a Navigation Modifier (Not Walkable) to every non-trigger collider on the chosen layers that doesn't already have one, so the bake carves out that collider's actual shape rather than its sprite's bounds.", EditorStyles.wordWrappedMiniLabel);
        colliderLayerMask = LayerMaskField("Collider Layers", colliderLayerMask);
        if (GUILayout.Button("Add Navigation Modifiers To Colliders"))
        {
            AddModifiersToColliders();
        }

        GUILayout.Space(15);

        GUILayout.Label("2. Bake", EditorStyles.boldLabel);
        GUILayout.Label("Rebuilds every Navigation Surface in the open scene(s) using each surface's own geometry setting.", EditorStyles.wordWrappedMiniLabel);
        if (GUILayout.Button("Build Nav Mesh!"))
        {
            BuildAllSurfaces();
        }
    }

    private void AddModifiersToColliders()
    {
        int notWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
        if (notWalkableArea < 0)
        {
            Debug.LogError("[NavigationHelper] Could not find a NavMesh area called \"Not Walkable\". Check Navigation > Areas.");
            return;
        }

        int added = 0;
        int skipped = 0;
        int relocated = 0;

        foreach (Collider2D collider in FindAll<Collider2D>())
        {
            if (collider.isTrigger)
                continue;

            // Pieces merged into a CompositeCollider2D are represented by the composite itself.
            if (collider is not CompositeCollider2D && collider.composite != null)
                continue;

            if (((1 << collider.gameObject.layer) & colliderLayerMask.value) == 0)
                continue;

            GameObject go = collider.gameObject;

            if (go.GetComponent<NavMeshModifier>() != null)
            {
                skipped++;
                continue;
            }

            // A previous version of this tool placed the modifier on a parent that only carries the
            // SpriteRenderer instead of on the collider itself. Baking off the sprite there carves the
            // wrong (visual, not physical) shape, so clean up any stray left behind by that.
            for (Transform t = go.transform.parent; t != null; t = t.parent)
            {
                if (t.GetComponent<Collider2D>() != null)
                    continue;
                if (t.GetComponent<SpriteRenderer>() == null && t.GetComponent<Tilemap>() == null)
                    continue;

                NavMeshModifier stray = t.GetComponent<NavMeshModifier>();
                if (stray != null)
                {
                    Undo.DestroyObjectImmediate(stray);
                    relocated++;
                }
            }

            NavMeshModifier modifier = Undo.AddComponent<NavMeshModifier>(go);
            modifier.overrideArea = true;
            modifier.area = notWalkableArea;
            EditorUtility.SetDirty(go);
            added++;
        }

        Debug.Log($"[NavigationHelper] Added Navigation Modifier to {added} collider object(s) ({relocated} stray modifier(s) removed from a sprite-only parent), skipped {skipped} that already had one.");
    }

    private void BuildAllSurfaces()
    {
        NavMeshSurface[] surfaces = FindAll<NavMeshSurface>();
        if (surfaces.Length == 0)
        {
            Debug.LogWarning("[NavigationHelper] No Navigation Surface component found in the open scene(s).");
            return;
        }

        foreach (NavMeshSurface surface in surfaces)
        {
            surface.BuildNavMesh();
            EditorUtility.SetDirty(surface);
        }

        Debug.Log($"[NavigationHelper] Baked {surfaces.Length} navigation surface(s).");
    }

    private static LayerMask LayerMaskField(string label, LayerMask layerMask)
    {
        List<string> layerNames = new();
        List<int> layerNumbers = new();
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (!string.IsNullOrEmpty(layerName))
            {
                layerNames.Add(layerName);
                layerNumbers.Add(i);
            }
        }

        int shownMask = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if ((layerMask.value & (1 << layerNumbers[i])) != 0)
                shownMask |= 1 << i;
        }

        shownMask = EditorGUILayout.MaskField(label, shownMask, layerNames.ToArray());

        int realMask = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if ((shownMask & (1 << i)) != 0)
                realMask |= 1 << layerNumbers[i];
        }

        layerMask.value = realMask;
        return layerMask;
    }

    private static T[] FindAll<T>() where T : Object
    {
        return FindObjectsByType<T>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    }
}
