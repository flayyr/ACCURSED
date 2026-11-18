using UnityEngine;

/// <summary>
/// Put this on any walkable prefab (bridge, platform, puddle...) to give it its own footstep VFX.
/// The prefab needs a trigger Collider2D on one of CharacterAnimator's surfaceLayers
/// (the collider can be on this object or on a child).
/// Prefab surfaces are checked before tiles; if several overlap, the one closest to the camera (lowest z) wins.
/// </summary>
public class SurfaceData : MonoBehaviour
{
    [Tooltip("Particle system prefab played when a character steps here. Should be a non-looping burst.")]
    [SerializeField] private ParticleSystem footstepVFX;

    public ParticleSystem FootstepVFX => footstepVFX;
}
