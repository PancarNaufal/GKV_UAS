using UnityEngine;
using UnityEngine.Tilemaps;

public class BoxColliderGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject colliderPrefab; // Empty GameObject with BoxCollider2D

    void Start()
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                Vector3 worldPos = tilemap.CellToWorld(pos) + tilemap.tileAnchor;
                Instantiate(colliderPrefab, worldPos + new Vector3(0.5f, 0.5f), Quaternion.identity, transform);
            }
        }
    }
}
