using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class FarmManager : MonoBehaviour
{
    public Tilemap fieldTilemap; // ���� ĥ���� Tilemap
    public Tilemap overlayTilemap; //���� ��ȭ �� ������ Tilemap
    public TileBase farmTile;  // ������ ������ Ÿ�� (FarmSoilTile.asset) // ���� �� Ÿ��
    public TileBase wetSoilTile; // ���� �� Ÿ��
    public Tilemap seedOverlayTilemap;   // ���� Ÿ�� ����
    public TileBase seedTile;           // ���� ��������Ʈ Ÿ�� (ex: seedTile.asset)

    private HashSet<Vector3Int> farmPositions = new HashSet<Vector3Int>();

    void Start()
    {
        RegisterFarmTiles();
    }

    // 1. Ÿ�ϸʿ��� �� ���� �ڵ� ���
    void RegisterFarmTiles()
    {
        farmPositions.Clear();

        // ������ ��ĵ (��ȿ�� ���� ��������)
        BoundsInt bounds = fieldTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = fieldTilemap.GetTile(pos);

                if (tile == farmTile)
                {
                    farmPositions.Add(pos);
                }
            }
        }

        Debug.Log($"�� ��ġ {farmPositions.Count}�� ��� �Ϸ�");
    }

    // 2. �� ��ġ�� ���ΰ�?
    public bool IsFarmTile(Vector3 worldPos)
    {
        Vector3Int cellPos = fieldTilemap.WorldToCell(worldPos);
        return farmPositions.Contains(cellPos);
    }

    // 3. ���� �� ���� Ȯ�� (��: ������)
    public void AddFarmTile(Vector3Int cellPos)
    {
        farmPositions.Add(cellPos);
        fieldTilemap.SetTile(cellPos, farmTile);
    }

    //�翡 �� �ѷ��� �� ��ȭ
    public void WaterSoil(Vector3 worldPos)
    {
        Vector3Int cellPos = fieldTilemap.WorldToCell(worldPos);

        if (IsFarmTile(worldPos))
        {
            overlayTilemap.SetTile(cellPos, wetSoilTile);
        }
    }

    public void PlantSeed(Vector3 worldPos)
    {
        Vector3Int cellPos = fieldTilemap.WorldToCell(worldPos);

        if (IsFarmTile(worldPos))
        {
            seedOverlayTilemap.SetTile(cellPos, seedTile);
        }
    }
}
