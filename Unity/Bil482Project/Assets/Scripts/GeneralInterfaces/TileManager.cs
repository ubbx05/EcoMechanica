using UnityEngine;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    // Ka� birimlik karelerle hizaland���n� belirler (�rne�in 1f = 1x1 kareler)
    public float tileSize = 1f;

    // Hangi tile�lar dolu tutulur
    private HashSet<Vector2Int> occupiedTiles = new HashSet<Vector2Int>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // D�nya koordinat�n� tile koordinat�na �evirir
    public Vector2Int WorldToTile(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x / tileSize),
            Mathf.FloorToInt(worldPos.y / tileSize)
        );
    }

    // Tile koordinat�n� d�nya pozisyonuna �evirir (merkeze hizalan�r)
    public Vector3 TileToWorld(Vector2Int tilePos)
    {
        return new Vector3(tilePos.x * tileSize + tileSize / 2f, 
                           tilePos.y * tileSize + tileSize / 2f, 
                           0f);
    }

    public bool IsOccupied(Vector2Int tilePos)
    {
        return occupiedTiles.Contains(tilePos);
    }

    public void MarkOccupied(Vector2Int tilePos)
    {
        occupiedTiles.Add(tilePos);
    }

    public void MarkFree(Vector2Int tilePos)
    {
        occupiedTiles.Remove(tilePos);
    }
}
