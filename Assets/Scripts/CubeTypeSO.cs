using UnityEngine;

[CreateAssetMenu(fileName = "CubeType", menuName = "Grid/Cube Type")]
public class CubeTypeSO : ScriptableObject
{
    public string displayName;
    public Material material;
}