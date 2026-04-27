using UnityEngine;

[CreateAssetMenu(fileName = "CellPaletteSettings", menuName = "Game/Cell Palette")]
public class CellPaletteSettings : ScriptableObject
{
    public Material selectedMaterial;
    public Material canMoveMaterial;
    public Material canAttackMaterial;
}