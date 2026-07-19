using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MajorRegions", menuName = "Scriptable Objects/Major Regions", order = 1)]
public class MajorRegion : ScriptableObject
{
    public string regionName;
    public List<Aspect> locations = new List<Aspect>();
    public Sprite regionImage;
}
