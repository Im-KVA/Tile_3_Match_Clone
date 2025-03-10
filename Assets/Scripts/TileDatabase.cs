using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TileDatabase", menuName = "Tile/TileDatabase")]
public class TileDatabase : ScriptableObject
{
    public List<TileData> allTiles;

    public TileData GetRandomTile()
    {
        if (allTiles.Count == 0) return null;
        return allTiles[Random.Range(0, allTiles.Count)];
    }

    public TileData GetTileByID(int id)
    {
        return allTiles.Find(tile => tile.tileID == id);
    }
}
