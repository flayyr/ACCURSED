using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Maps tiles (Tile, RuleTile, AnimatedTile... anything TileBase) to a footstep VFX prefab.
/// One entry per surface type, drag all its tiles in.
/// </summary>
[CreateAssetMenu(fileName = "TileSurfaceDataBase", menuName = "Scriptable Objects/Footsteps/Tile Surface Database")]
public class TileSurfaceDataBaseSO : ScriptableObject
{
    [Serializable]
    public class SurfaceEntry
    {
        public string name; // inspector label only (e.g. Grass, Sand)
        public ParticleSystem footstepVFX;
        public List<TileBase> tiles = new List<TileBase>();
    }

    [SerializeField] private List<SurfaceEntry> surfaces = new List<SurfaceEntry>();

    // Flattened tile -> VFX map so each footstep is one lookup instead of looping every list
    private Dictionary<TileBase, ParticleSystem> lookup;

    public bool TryGetFootstepVFX(TileBase tile, out ParticleSystem vfx)
    {
        vfx = null;
        if (tile == null) return false;

        // Built once on first use
        if (lookup == null)
        {
            lookup = new Dictionary<TileBase, ParticleSystem>();
            foreach (SurfaceEntry entry in surfaces)
                foreach (TileBase t in entry.tiles)
                    if (t != null) lookup[t] = entry.footstepVFX;
        }

        return lookup.TryGetValue(tile, out vfx) && vfx != null;
    }

    // Rebuild after editing the asset in the inspector
    private void OnValidate() => lookup = null;
}
