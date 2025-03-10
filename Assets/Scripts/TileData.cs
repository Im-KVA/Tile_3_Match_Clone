using UnityEngine;

[CreateAssetMenu(fileName = "New Tile", menuName = "Tile/TileData")]
public class TileData : ScriptableObject
{
    [Header("Tile Info")]
    public int tileID;

    [Header("Tile Sprites")]
    public Sprite tileSprite;
    public Sprite tileUnderSprite;

    [Header("Tile Type Sprite")]
    public Sprite tileTypeSprite;
}
